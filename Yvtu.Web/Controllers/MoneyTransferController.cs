using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NToastNotify;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;
using Yvtu.Web.Reports;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class MoneyTransferController : Controller
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;
        private readonly IPartnerActivityRepo _partnerActivity;
        private readonly IConverter converter;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<MoneyTransferController> logger;
        private readonly IToastNotification _toastNotification;

        public MoneyTransferController(IAppDbContext db, IPartnerManager partnerManager
            , IPartnerActivityRepo partnerActivity, IConverter converter, IWebHostEnvironment environment,
            ILogger<MoneyTransferController> logger,  IToastNotification toastNotification)
        {
            this._db = db;
            this._partnerManager = partnerManager;
            this._partnerActivity = partnerActivity;
            this.converter = converter;
            this.environment = environment;
            this.logger = logger;
            _toastNotification = toastNotification;
        }
        //[HttpGet]
        public IActionResult CreatePDF(int id)
        {
            var model = new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).GetSingleOrDefault(id);
            if (model == null) return Ok("غير موجود");
            var roleId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.GivenName).Value;
            var permission = _partnerActivity.GetPartAct("MoneyTransfer.Print", int.Parse(roleId));
            var currUserId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.MobilePhone).Value;
            if (permission == null) return LocalRedirect("/Account/AccessDenied");
            if (permission.Scope.Id != "Everyone" && model.CreatedBy.Id != currUserId) return LocalRedirect("/Account/AccessDenied");


            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize =  PaperKind.A4,
                Margins = new MarginSettings { Top = 10},
                DocumentTitle = "Money Transfer"
                
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = new MoneyTransferTemplate(_db, _partnerManager, environment, _partnerActivity).GetHTMLString(id),
                WebSettings =
                {
                    DefaultEncoding = "utf-8",UserStyleSheet=Path.Combine(environment.WebRootPath, "css","Reports","rptMoneyTransfer.css")
                },
                //HeaderSettings = {FontName = "Arial", FontSize = 9, Right = "page [page] of [topage]",Line=true},
                FooterSettings = {FontName = "Arial", FontSize = 9, Right = "page [page] of [topage]",Line=true, Center="Y Company"},
                
                
            };

            var pdf = new HtmlToPdfDocument {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            
            var file = converter.Convert(pdf);

            return File(file, "application/pdf");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("MoneyTransfer.Create", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreateMoneyTransferDto();
            var currentPartAccount = _partnerManager.GetCurrentUserAccount(this.HttpContext);
            var currentPart = _partnerManager.GetPartnerByAccount(currentPartAccount);
            model.CreatorBalance = currentPart.Balance - currentPart.Reserved;
            var payTypes = new CommonCodeRepo(_db).GetCodesByType("pay.type");
            model.PayType = payTypes;
            model.PayDate = DateTime.Today;
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateMoneyTransferDto model)
        {
            if (ModelState.IsValid)
            {
                if (!Utility.ValidYMobileNo(model.PartnerId))
                {
                    model.Error = "رقم موبايل خاطئ";
                    model.PayType = new CommonCodeRepo(_db).GetCodesByType("pay.type");
                    return View(model);
                }
                if (model.Amount <= 0)
                {
                    model.Error = "المبلغ غير صحيح";
                    model.PayType = new CommonCodeRepo(_db).GetCodesByType("pay.type");
                    return View(model);
                }
                var result = GetBasicInfo(model.PartnerId, model.Amount);
                if (result.Error != "N/A")
                {
                    model.Error =result.Error;
                    model.PayType = new CommonCodeRepo(_db).GetCodesByType("pay.type");
                    return View(model);
                }
                result.AmountName = new MonyToString().NumToStr(result.Amount);
                result.PayTypeName = new CommonCodeRepo(_db).GetCodesById(model.PayTypeId, "pay.type").Name;
                result.PayTypeId = model.PayTypeId;
                result.PayNo = model.PayNo;
                result.PayDate = model.PayDate;
                result.PayBank = model.PayBank;
                result.Note = model.Note;
                result.BillNo = model.BillNo;
                result.RequestNo = model.RequestNo;
                result.RequestAmount = model.RequestAmount;
                result.PayType = new CommonCodeRepo(_db).GetCodesByType("pay.type");
                return View("Confirmation", result);
            }
            var payTypes = new CommonCodeRepo(_db).GetCodesByType("pay.type");
            model.PayType = payTypes;
            return View(model);
        }

        [HttpPost]
        public IActionResult Confirmation(CreateMoneyTransferDto model)
        {
            if (model.Id > 0)
            {
                model.Error = "تم التحويل مسبقا";
                return View(model);
            }
            var moneyTransfer = new MoneyTransfer();
            moneyTransfer.Partner = _partnerManager.GetPartnerById(model.PartnerId);
            moneyTransfer.PayType.Id = model.PayTypeId;
            moneyTransfer.PayNo = model.PayNo;
            moneyTransfer.PayDate = model.PayDate;
            moneyTransfer.PayBank = model.PayBank;
            moneyTransfer.CreatedBy = _partnerManager.GetPartnerById(_partnerManager.GetCurrentUserId(this.HttpContext));
            moneyTransfer.AccessChannel.Id = "web";
            moneyTransfer.Amount = model.Amount;
            moneyTransfer.BillNo = model.BillNo;
            moneyTransfer.RequestNo = model.RequestNo;
            moneyTransfer.RequestAmount = model.RequestAmount;
            moneyTransfer.Note = model.Note;
            moneyTransfer.NetAmount = model.NetAmount;
            moneyTransfer.TaxPercent = model.TaxPercent;
            moneyTransfer.TaxAmount =  model.TaxAmount;
            moneyTransfer.BonusPercent = model.BonusPercent;
            moneyTransfer.BounsAmount = model.BounsAmount;
            moneyTransfer.BounsTaxPercent = model.BounsTaxPercent;
            moneyTransfer.BounsTaxAmount = model.BounsTaxAmount;
            moneyTransfer.ReceivedAmount = model.ReceivedAmount;

            var result = new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).Create(moneyTransfer);
            if (result.Success)
            {
                model.Id = result.AffectedCount;
                ModelState.SetModelValue("Id", new ValueProviderResult("" + result.AffectedCount + "", CultureInfo.InvariantCulture));
                //CreatePDF(model.Id);
                moneyTransfer.NetAmount = Math.Round(moneyTransfer.NetAmount, 2);
                moneyTransfer.Partner.Balance = _partnerManager.GetBalance(moneyTransfer.Partner.Account);
                moneyTransfer.CreatedBy.Balance = _partnerManager.GetBalance(moneyTransfer.CreatedBy.Account);
                new NotificationRepo(_db, _partnerManager).SendNotification<MoneyTransfer>("MoneyTransfer.Create", result.AffectedCount, moneyTransfer);
                return View(model);
            }
            else
            {
                if (result.AffectedCount == -500)
                {
                    model.Error = "لم يتم تعريف هذا الاجراء او ليس لديك الصلاحية الكافية";
                } else if (result.AffectedCount == -501)
                {
                    model.Error =  $"رصيدك غير كافي { model.CreatorBalance.ToString("N0") } ";
                }
                else if (result.AffectedCount == -502)
                {
                    model.Error = $"المبلغ اقل من الاحد الادنى المسموح به";
                }
                if (result.AffectedCount == -503)
                {
                    model.Error = $"المبلغ اكبر من الاحد الاعلى المسموح به";
                }
            }
            return View(model);
        }

        public CreateMoneyTransferDto GetBasicInfo(string pId, double amount = 0)
        {
            if (pId.Length != 9) return new CreateMoneyTransferDto { Error = "رقم خاطئ" };
            var validateResult =  _partnerManager.Validate(pId);
            if (validateResult.Success)
            {
                var currentId = _partnerManager.GetCurrentUserId(this.HttpContext);
                var partner = validateResult.Partner;
                var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = _partnerActivity.GetPartAct("MoneyTransfer.Create", currentRoleId);
                if (permission == null)
                {
                    return new CreateMoneyTransferDto { Error = "ليس لديك الصلاحية الكافية" };
                }

                if (permission.Details == null || permission.Details?.Count == 0)
                {
                    return new CreateMoneyTransferDto { Error = "لم يتم تعريف هذا الاجراء او ليس لديك الصلاحية الكافية" };
                }

                var moneyTransferSettings = permission.Details.Find(x => x.ToRole.Id == partner.Role.Id);
                if (moneyTransferSettings == null) 
                    return new CreateMoneyTransferDto { Error = "لم يتم تعريف هذا الاجراء او ليس لديك الصلاحية الكافية" };

                if (permission.Scope.Id == "CurOpOnly")
                {
                    return new CreateMoneyTransferDto { Error = "نظرا للقيود التي تم تعريفها على العملية لا يمكنك تنفيذ الاجراء" };
                }
                if (permission.Scope.Id == "Exclusive" && partner.RefPartner.Id != currentId)
                {
                    return new CreateMoneyTransferDto { Error = "ليس لديك الصلاحية الكافية لنقل رصيد الى هذه الجهة" };
                }

                var model = new CreateMoneyTransferDto
                {
                    PartnerId = partner.Id,
                    PartnerName = partner.Name,
                    PartnerRoleName = partner.Role.Name,
                    PartnerBalance = partner.Balance,
                    TaxPercent = moneyTransferSettings.TaxPercent,
                    BonusPercent = moneyTransferSettings.BonusPercent,
                    BounsTaxPercent = moneyTransferSettings.BonusTaxPercent,
                    FixedFactor = moneyTransferSettings.FixedFactor,
                    Error = "N/A"
                };
                if (amount <= 0) return model;

                if (moneyTransferSettings.MaxValue > 0 && amount > moneyTransferSettings.MaxValue)
                {
                    model.Error = $"المبلغ اكبر من الاحد الاعلى المسموح به {moneyTransferSettings.MaxValue.ToString("N0")} " ;
                    return model;
                }
                if (moneyTransferSettings.MinValue > 0 && amount < moneyTransferSettings.MinValue)
                {
                    model.Error = $"المبلغ اقل من الاحد الادنى المسموح به {moneyTransferSettings.MinValue.ToString("N0")} ";
                    return model;
                }

                var currParAccountd = _partnerManager.GetCurrentUserAccount(this.HttpContext);
                var currPart = _partnerManager.GetPartnerByAccount(currParAccountd);
                model.CreateorId = currPart.Id;
                model.CreateorName = currPart.Name;
                model.CreateorRoleId = currPart.Role.Id;
                model.CreateorRoleName = currPart.Role.Name;
                model.CreatorBalance = currPart.Balance - currPart.Reserved;
                if (moneyTransferSettings.CheckBalanceRequired)
                {
                    
                    if (amount > model.CreatorBalance)
                    {
                        model.Error = $"رصيدك غير كافي { model.CreatorBalance.ToString("N0") } ";
                        return model;
                    }
                }

                var netAmount = amount * (moneyTransferSettings.FixedFactor <= 0 ? 1 : moneyTransferSettings.FixedFactor) ;
                var taxAmount = netAmount * (moneyTransferSettings.TaxPercent / 100);
                var bounsAmount = netAmount * (moneyTransferSettings.BonusPercent / 100);
                var bounsTaxAmount = bounsAmount * (moneyTransferSettings.BonusTaxPercent / 100);
                var recievedAmount = netAmount;

                model.Amount = amount;
                model.NetAmount = netAmount;
                model.TaxAmount = taxAmount;
                model.BounsAmount = bounsAmount;
                model.BounsTaxAmount = bounsTaxAmount;
                model.ReceivedAmount = recievedAmount;

                return model;

            }
            else
            {
                return new CreateMoneyTransferDto { Error = "غير موجود" };
            }
            

        }
        [HttpGet]
        public IActionResult MoneyTranferQuery()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("MoneyTransfer.Query", currentRoleId);
            if (permission == null)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new MoneyTransferQueryDto();
            model.Paging.PageNo = 1;
            model.Paging.PageSize = 50;
            model.Paging.Count = 0;
            model.QFromDate = DateTime.Today.AddMonths(-1);
            model.QToDate = DateTime.Today;
            model.QPartnerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value;
            return View(model);
        }
        [HttpPost]
        
        public IActionResult MoneyTranferQuery(MoneyTransferQueryDto model, [FromQuery
        (Name = "direction")] string direction  )
        {
            #region Prepare Query
            model.Error = string.Empty;
            Partner targetPartner = null;
            var currUserId = _partnerManager.GetCurrentUserId(this.HttpContext);
            var currRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var currAccountId = _partnerManager.GetCurrentUserAccount(this.HttpContext);

            var permission = _partnerActivity.GetPartAct("MoneyTransfer.Query", currRoleId);
            if (permission == null)
            {
                model.Error = "ليس لديك الصلاحيات الكافية";
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.QPartnerId) && model.QPartnerId != currUserId)
            {
                var validateTargetPartnerResult = _partnerManager.Validate(model.QPartnerId);
                targetPartner = validateTargetPartnerResult.Success ? validateTargetPartnerResult.Partner : null;
                if (targetPartner == null)
                {
                    model.Error = "يرجى التأكد من الرقم المراد الاستعلام عنه";
                    return View(model);
                }
            }
            if (permission.Scope.Id == "CurOpOnly" && model.QPartnerId != currUserId)
            {
                model.Error = "ليس لديك الصلاحيات الكافية للاستعلام عن هذا الرقم";
                return View(model);
            }
            else if (permission.Scope.Id == "Exclusive" && targetPartner != null &&  targetPartner.RefPartner.Id != currUserId)
            {
                model.Error = "ليس لديك الصلاحيات الكافية للاستعلام عن هذا الرقم";
                return View(model);
            }

            
            #endregion
            ModelState.Clear();
            if (direction == "pre" && model.Paging.PageNo > 1)
            {
                model.Paging.PageNo -= 1;
            }
            if (direction == "next")
            {
                model.Paging.PageNo += 1;
            }

            if (model.QFromDate == DateTime.MinValue) model.QFromDate = DateTime.Today.AddMonths(-1);
            if (model.QToDate == DateTime.MinValue) model.QFromDate = DateTime.Today;

            model.QueryUser = _partnerManager.GetCurrentUserId(this.HttpContext);
            model.QScope = permission.Scope.Id;
            var result = new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).MTQueryWithPaging(model);
            if (result != null && result.Results != null)
            {
                model.Paging.Count = new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).GetCount(model);
            }
            else
            {
                model.Paging.Count = 0;
            }
            
            return View(result);
     
        }
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var model = new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).GetSingleOrDefault(id);
            return View(model);
        }
    }
}
