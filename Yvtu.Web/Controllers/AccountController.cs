using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
    public class AccountController : Controller
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;
        private readonly IPartnerActivityRepo _partnerActivity;
        private readonly IToastNotification _toastNotification;
        private readonly IConverter _converter;
        private readonly IWebHostEnvironment _environment;

        public AccountController(IAppDbContext db
            ,IPartnerManager partner
            ,IPartnerActivityRepo partnerActivity
            , IToastNotification toastNotification
            , IConverter converter, 
            IWebHostEnvironment environment)
        {
            this._db = db;
            this._partnerManager = partner;
            this._partnerActivity = partnerActivity;
            this._toastNotification = toastNotification;
            this._converter = converter;
            this._environment = environment;
        }
        public IActionResult Index()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.Query", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new PartnerQuery();
            var roles = new RoleRepo(_db, _partnerActivity).GetRoles();
            var statuses = new PartnerStatusRepo(_db).GetStatusList();
            model.Roles = roles;
            model.Statuses = statuses;
            model.Paging.PageNo = 1;
            model.Paging.PageSize = 50;
            model.Paging.Count = 0;
            return View(model);
        }

        public IActionResult Detail(int account)
        {
            var model = _partnerManager.GetPartnerByAccount(account);
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPass(int account)
        {
            var model = new ResetPassDto();
            var roleCode = _partnerManager.GetCurrentUserRoleCode(this.HttpContext);
            if (roleCode != "Admin")
            {
                model.Error = "ليس لديك الصلاحية الكافية";
                model.Success = string.Empty;
            }
            else
            {
                var partnerModel = this._partnerManager.GetPartnerByAccount(account);
                var persmission = _partnerActivity.GetPartAct("Partner.ResetPassword", this._partnerManager.GetCurrentUserRole(this.HttpContext));

                if (persmission == null)
                {
                    model.PartnerId = partnerModel.Id;
                    model.PartnerName = partnerModel.Name;
                    model.Error = "ليس لديك الصلاحية الكافية";
                    model.Success = string.Empty;
                }
                else if (persmission.Details == null)
                {
                    model.PartnerId = partnerModel.Id;
                    model.PartnerName = partnerModel.Name;
                    model.Error = "ليس لديك الصلاحية الكافية";
                    model.Success = string.Empty;
                }
                else if (!persmission.Details.Exists(x => x.ToRole.Id == partnerModel.Role.Id))
                {
                    model.PartnerId = partnerModel.Id;
                    model.PartnerName = partnerModel.Name;
                    model.Error = "ليس لديك الصلاحية الكافية";
                    model.Success = string.Empty;
                }
                else
                {
                    partnerModel.CreatedBy.Id = _partnerManager.GetCurrentUserId(this.HttpContext);
                    var result = _partnerManager.ResetPassword(partnerModel);
                    if (result)
                    {
                        model.PartnerId = partnerModel.Id;
                        model.PartnerName = partnerModel.Name;
                        model.Error = string.Empty;
                        model.Success = "تم تغيير كلمة المرور";
                        _toastNotification.AddSuccessToastMessage("تم اعادة تعيين كلمة المرور بنجاح");
                    }
                }
            }
            return base.View(model);
            
        }

        [HttpPost]
        public IActionResult Index(PartnerQuery model, [FromQuery
        (Name = "direction")] string direction)
        {
            ModelState.Clear();
            if (direction == "pre" && model.Paging.PageNo > 1)
            {
                model.Paging.PageNo -= 1;
            }
            if (direction == "next")
            {
                model.Paging.PageNo += 1;
            }
            model.Partners = _partnerManager.GetPartnersWithPaging(model);
            if (model != null && model.Partners != null)
            {
                model.Paging.Count = _partnerManager.GetCount(model);
            }
            else
            {
                model.Paging.Count = 0;
            }
            var roles = new RoleRepo(_db, _partnerActivity).GetRoles();
            var statuses = new PartnerStatusRepo(_db).GetStatusList();
            model.Roles = roles;
            model.Statuses = statuses;
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RoleList()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("AppRole.Query", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                      Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var lst = new RoleRepo(_db, null).GetRoles();
            if (lst == null) return View(new List<RoleQueryDto>());
            var queryList = new List<RoleQueryDto>();
            foreach (var item in lst)
            {
                var obj = new RoleQueryDto();
                obj.Id = item.Id;
                obj.Name = item.Name;
                obj.Code = item.Code;
                obj.Order = item.Order;
                obj.Weight = item.Weight;
                obj.IsActive = item.IsActive;
                obj.PartnerCount = new RoleRepo(_db, null).GetPartnerCount(item.Id);
                queryList.Add(obj);
            }
            return View(queryList);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model, string returnUrl = "")
        {
            model.Error = string.Empty;
            if (ModelState.IsValid)
            {
                var partnerResult = this._partnerManager.Validate(model.Id);
                if (partnerResult.Success)
                {
                    var permission = _partnerActivity.GetPartAct("System.Login", partnerResult.Partner.Role.Id);
                    if (permission == null)
                    {
                        _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                        {
                            Title = "تنبيه"
                        });
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                    if (partnerResult.Partner.Status.Id > 2)
                    {
                        _toastNotification.AddInfoToastMessage("عذرا ، بحسب حالة الحساب لايمكنك استخدام النظام حاليا");
                        return View(model);
                    }
                    if (partnerResult.Partner.LockTime > DateTime.Now)
                    {
                        _toastNotification.AddInfoToastMessage("عذرا ، حسابك متوقف مؤقتا لمدة  " + Utility.HowMuchLeftTime(partnerResult.Partner.LockTime));
                        _toastNotification.AddErrorToastMessage("حسابك متوقف مؤقتا");
                        return View(model);
                    }

                        byte[] salt = Convert.FromBase64String(partnerResult.Partner.Extra);
                        string hash = Pbkdf2Hasher.ComputeHash(model.Pwd, salt);

                        if (partnerResult.Partner.Pwd != hash)
                        {
                            bool lockAccount = false;
                            if (partnerResult.Partner.WrongPwdAttempts >= 2) lockAccount = true;
                            _partnerManager.IncreaseWrongPwdAttempts(partnerResult.Partner.Id, lockAccount);
                            _toastNotification.AddInfoToastMessage("عذرا ، رمز المستخدم او كلمة المرور غير صحيح" + Environment.NewLine 
                                +"(" + partnerResult.Partner.WrongPwdAttempts + ")");
                            return View(model);
                        }

                        _partnerManager.PreSuccessLogin(partnerResult.Partner.Id);

                        ClaimsIdentity identity = new ClaimsIdentity(_partnerManager.GetUserClaims(partnerResult.Partner)
                            , CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal
                            , new AuthenticationProperties() { IsPersistent = model.RememberMe });


                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                    
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult AppUsersList()
        {
            
                return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.Create", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreatePartnerDto();
            var roles = new RoleRepo(_db, _partnerActivity).GetAuthorizedRoles("Partner.Create", currentRoleId);
            var idTypes = new IdTypeRepo(_db).GetTypes();
            var cities = new CityRepo(_db).GetCities();
         
            model.RefPartnerId = _partnerManager.GetCurrentUserId(this.HttpContext);
            model.Roles = roles;
            model.IdTypes = idTypes;
            model.Cities = cities;
            model.Districts = new List<District>();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePartnerDto model)
        {
            if (ModelState.IsValid)
            {
                var createdPartner = new Partner();

                createdPartner.Id = model.Id;
                createdPartner.Name = model.Name;
                createdPartner.PairMobile = model.PairMobile;
                createdPartner.Role.Id = model.RoleId ?? 0;
                createdPartner.BrandName = model.BrandName;
                createdPartner.PersonalId.Id = model.PersonalIdNo;
                createdPartner.PersonalId.IdType.Id = model.PersonalIdType ?? 0;
                createdPartner.PersonalId.Issued = model.PersonalIssued ?? DateTime.MinValue;
                createdPartner.PersonalId.Place = model.PersonalIdPlace;
                createdPartner.Address.City.Id = model.CityId ?? 0;
                createdPartner.Address.District.Id = model.DistrictId ?? 0;
                createdPartner.Address.Street = model.Street;
                createdPartner.Address.Zone = model.Zone;
                createdPartner.Address.ExtraInfo = model.ExtraAddressInfo;
                createdPartner.ContactInfo.Mobile = model.MobileNo;
                createdPartner.ContactInfo.Fixed = model.Fixed;
                createdPartner.ContactInfo.Fax = model.Fax;
                createdPartner.ContactInfo.Email = model.Email;
                createdPartner.Pwd = Utility.GenerateNewCode(4);
                createdPartner.CreatedBy.Id = _partnerManager.GetCurrentUserId(this.HttpContext);
                createdPartner.Status.Id = 1;
                createdPartner.StatusBy.Id = _partnerManager.GetCurrentUserId(this.HttpContext);
                createdPartner.IPAddress = model.IPAddress;
                createdPartner.RefPartner.Id = model.RefPartnerId;


                var result = await _partnerManager.CreateAsync(createdPartner);
                if (result.Success)
                {
                    _toastNotification.AddSuccessToastMessage("تم انشاء جهة جديدة بنجاح", new ToastrOptions
                    {
                        Title = "انشاء جهة"
                    });
                    return RedirectToAction("Index", "Home");
                }
            }
            //var model = new CreatePartnerDto();
            var roles = new RoleRepo(_db, _partnerActivity).GetRoles();
            var idTypes = new IdTypeRepo(_db).GetTypes();
            var cities = new CityRepo(_db).GetCities();
            var districts = new DistrictRepo(_db).GetDistrictsByCity(model.CityId ?? 0);
            model.Roles = roles;
            model.IdTypes = idTypes;
            model.Cities = cities;
            model.Districts = districts;
            return View(model);
        }

        public IActionResult GetDistrictsByCity(int id)
        {
            return Json(new DistrictRepo(_db).GetDistrictsByCity(id));
        }

        public async Task<ActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ChangePass()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePass(ChangePassDto model)
        {
            if (ModelState.IsValid)
            {
                var currentPartner = _partnerManager.GetPartnerByAccount(_partnerManager.GetCurrentUserAccount(this.HttpContext));
                if (currentPartner == null)
                {
                    model.Error = "عذرا ، لا يمكنك تغيير كلمة المرور الخاصة بك";
                    return View(model);
                }
                if (currentPartner.Status.Id > 2)
                {
                    model.Error = "عذرا ، بحسب حالة الحساب لايمكنك استخدام النظام حاليا";
                    return View(model);
                }
                if (currentPartner.LockTime > DateTime.Now)
                {
                    model.Error = "عذرا ، حسابك متوقف مؤقتا لمدة  " + Utility.HowMuchLeftTime(currentPartner.LockTime);
                    await SignOut();
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewPass.ToString(), "^[0-9]{4,6}$"))
                {
                    model.Error = "كلمة المرور الجديدة غير مستوفية للشروط";
                    return View(model);
                }
                byte[] salt = Convert.FromBase64String(currentPartner.Extra);
                string hash = Pbkdf2Hasher.ComputeHash(model.OldPass.ToString(), salt);

                if (currentPartner.Pwd != hash)
                {
                    bool lockAccount = false;
                    if (currentPartner.WrongPwdAttempts >= 2) lockAccount = true;
                    _partnerManager.IncreaseWrongPwdAttempts(currentPartner.Id, lockAccount);
                    model.Error = "كلمة المرور القديمة غير صحيح" + Environment.NewLine + "(" + currentPartner.WrongPwdAttempts + ")";
                    return View(model);
                }

                _partnerManager.ChangePwd(currentPartner.Account, currentPartner.Id, model.NewPass.ToString(),true);
                _toastNotification.AddSuccessToastMessage("تم تغيير كلمة المرور بنجاح");
            }

            return View(model);
        }


        public IActionResult Suspend()
        {
            var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Suspend", currentRole);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreateChangeStatusDto();
            model.NewStatus = new PartnerStatusRepo(_db).GetStatus(3);
            model.NewStatusExpireOn = DateTime.Today.Add(TimeSpan.FromDays(30));
            return View(model);
        }
        [HttpPost]
        public IActionResult Suspend(CreateChangeStatusDto model)
        {
            if (ModelState.IsValid)
            {
                if (!Utility.ValidYMobileNo(model.PartnerId))
                {
                    _toastNotification.AddErrorToastMessage("الرقم غير صحيح");
                } else
                {
                    var partner = _partnerManager.GetPartnerBasicInfo(model.PartnerId);
                    if (partner == null) 
                    {
                        _toastNotification.AddErrorToastMessage("البيانات غير متوفرة");
                    }
                    else if (partner.Status.Id > 2)
                    {
                        _toastNotification.AddErrorToastMessage("لا يمكن ايقاف نشاط هذه الجهة بسبب حالتها الحالية");
                    }
                    else
                    {
                        var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
                        var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Suspend", currentRole);
                        if (permission == null)
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        } else if (permission.Details == null || permission.Details.Count == 0)
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        } else if (permission.Scope.Id == "CurOpOnly")
                        {
                           _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != _partnerManager.GetCurrentUserId(this.HttpContext))
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        } else
                        {
                            var insObj = new PartnerStatusLog();
                            insObj.CreatedBy.Id = _partnerManager.GetCurrentUserId(this.HttpContext);
                            insObj.CreatedBy.Account = _partnerManager.GetCurrentUserAccount(this.HttpContext);
                            insObj.Partner.Id = partner.Id;
                            insObj.Partner.Account = partner.Account;
                            insObj.OldStatus = partner.Status;
                            insObj.NewStatus.Id = 3;
                            insObj.Note = model.Note;
                            insObj.NewStatusExpireOn = model.NewStatusExpireOn;
                            var result = new PartnerStatusLogRepo(_db, _partnerManager).Create(insObj);
                            if (result.Success)
                            {
                                _toastNotification.AddSuccessToastMessage("تم ايقاف النشاط بنجاح");
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                _toastNotification.AddErrorToastMessage("فشلت عملية تغيير الحالة");
                            }
                        }
                    }
                }

            }
            model.NewStatus = new PartnerStatusRepo(_db).GetStatus(3);
            return View(model);
        }

        public PartBasicInfo GetBasicInfo4S(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new PartBasicInfo { Error = "رقم غير صحيح" };
            var partner = _partnerManager.GetPartnerBasicInfo(id);
            if (partner == null) return new PartBasicInfo { Error = "البيانات غير متوفرة" };
            var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Suspend", currentRole);
            if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (!permission.Details.Any(x => x.ToRole.Id == partner.Role.Id) || permission.Details.Count == 0)
                return new PartBasicInfo { Error = " ليس لديك الصلاحية الكافية لتنفيذ هذا الاجراء على هذه الجهة" };
            partner.Error = "N/A";
            return partner;
        }

        public PartBasicInfo GetBasicInfo4C(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new PartBasicInfo { Error = "رقم غير صحيح" };
            var partner = _partnerManager.GetPartnerBasicInfo(id);
            if (partner == null) return new PartBasicInfo { Error = "البيانات غير متوفرة" };
            var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Cancel", currentRole);
            if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (!permission.Details.Any(x => x.ToRole.Id == partner.Role.Id) || permission.Details.Count == 0)
                return new PartBasicInfo { Error = " ليس لديك الصلاحية الكافية لتنفيذ هذا الاجراء على هذه الجهة" };
            partner.Error = "N/A";
            return partner;
        }

        public PartBasicInfo GetBasicInfo4A(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new PartBasicInfo { Error = "رقم غير صحيح" };
            var partner = _partnerManager.GetPartnerBasicInfo(id);
            if (partner == null) return new PartBasicInfo { Error = "البيانات غير متوفرة" };
            var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Reactive", currentRole);
            if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (!permission.Details.Any(x => x.ToRole.Id == partner.Role.Id) || permission.Details.Count == 0)
                return new PartBasicInfo { Error = " ليس لديك الصلاحية الكافية لتنفيذ هذا الاجراء على هذه الجهة" };
            partner.Error = "N/A";
            return partner;
        }

        public PartBasicInfo GetBasicInfo4Co(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new PartBasicInfo { Error = "رقم غير صحيح" };
            var partner = _partnerManager.GetPartnerBasicInfo(id);
            if (partner == null) return new PartBasicInfo { Error = "البيانات غير متوفرة" };
            var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Confiscate", currentRole);
            if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (!permission.Details.Any(x => x.ToRole.Id == partner.Role.Id) || permission.Details.Count == 0)
                return new PartBasicInfo { Error = " ليس لديك الصلاحية الكافية لتنفيذ هذا الاجراء على هذه الجهة" };
            if (partner.Balance <= 0)
                return new PartBasicInfo { Error = "عذرا لا يوجد رصيد لمصادرته " };
            partner.Error = "N/A";
            return partner;
        }

        public async Task<List<IdName>> GetAccounts(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return null;
            var accounts = await _partnerManager.GetAccountsAsync(id);
            if (accounts == null) return null;
            //var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
            //var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Confiscate", currentRole);
            //if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            //if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            //accounts.Error = "N/A";
            return accounts;
        }
        public Partner GetP(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new Partner { Extra = "رقم غير صحيح" };
            var getResult = _partnerManager.Validate(id);
            if (getResult.Success)
            {
                var partner = getResult.Partner;
                var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Edit", currentRole);
                if (permission == null) return new Partner { Extra = "ليس لديك الصلاحية الكافية" };
                if (permission.Details == null || permission.Details.Count == 0) return new Partner { Extra = "ليس لديك الصلاحية الكافية" };
                if (!permission.Details.Any(x => x.ToRole.Id == partner.Role.Id) || permission.Details.Count == 0) 
                    return new Partner { Extra = " ليس لديك الصلاحية الكافية لتعديل هذه الجهة" };
                partner.Extra = "N/A";
                return partner;
            }
            else
            {
                return new Partner { Extra = "رقم غير صحيح" };
            }
        }

        public IActionResult Cancel()
        {
            var model = new CreateChangeStatusDto();
            model.NewStatus = new PartnerStatusRepo(_db).GetStatus(4);
            //model.NewStatusExpireOn = DateTime.Today.Add(TimeSpan.FromDays(30));
            return View(model);
        }

        [HttpPost]
        public IActionResult Cancel(CreateChangeStatusDto model)
        {
            if (ModelState.IsValid)
            {
                if (!Utility.ValidYMobileNo(model.PartnerId))
                {
                    _toastNotification.AddErrorToastMessage("الرقم غير صحيح");
                }
                else
                {
                    var partner = _partnerManager.GetPartnerBasicInfo(model.PartnerId);
                    if (partner == null)
                    {
                        _toastNotification.AddErrorToastMessage("البيانات غير متوفرة");
                    }
                    else if (partner.Status.Id > 2)
                    {
                        _toastNotification.AddErrorToastMessage("لا يمكن ايقاف نشاط هذه الجهة بسبب حالتها الحالية");
                    }
                    else if (partner.Balance > 0)
                    {
                        _toastNotification.AddErrorToastMessage("لا يمكن ايقاف نهائي لجهة لديها رصيد يجب اولا مصادرة الرصيد");
                    }
                    else
                    {
                        var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
                        var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Cancel", currentRole);
                        if (permission == null)
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Details == null || permission.Details.Count == 0)
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "CurOpOnly")
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != _partnerManager.GetCurrentUserId(this.HttpContext))
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else
                        {
                            var insObj = new PartnerStatusLog();
                            insObj.CreatedBy.Id = _partnerManager.GetCurrentUserId(this.HttpContext);
                            insObj.CreatedBy.Account = _partnerManager.GetCurrentUserAccount(this.HttpContext);
                            insObj.Partner.Id = partner.Id;
                            insObj.Partner.Account = partner.Account;
                            insObj.OldStatus = partner.Status;
                            insObj.NewStatus.Id = 4;
                            insObj.Note = model.Note;
                            insObj.NewStatusExpireOn = model.NewStatusExpireOn;
                            var result = new PartnerStatusLogRepo(_db, _partnerManager).Create(insObj);
                            if (result.Success)
                            {
                                _toastNotification.AddSuccessToastMessage("تم ايقاف النشاط بشكل نهائي بنجاح");
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                _toastNotification.AddErrorToastMessage("فشلت عملية تغيير الحالة");
                            }
                        }
                    }
                }

            }
            model.NewStatus = new PartnerStatusRepo(_db).GetStatus(4);
            return View(model);
        }

        public IActionResult Reactive()
        {
            var model = new CreateChangeStatusDto();
            model.NewStatus = new PartnerStatusRepo(_db).GetStatus(1);
            //model.NewStatusExpireOn = DateTime.Today.Add(TimeSpan.FromDays(30));
            return View(model);
        }

        [HttpPost]
        public IActionResult Reactive(CreateChangeStatusDto model)
        {
            if (ModelState.IsValid)
            {
                if (!Utility.ValidYMobileNo(model.PartnerId))
                {
                    _toastNotification.AddErrorToastMessage("الرقم غير صحيح");
                }
                else
                {
                    var partner = _partnerManager.GetPartnerBasicInfo(model.PartnerId);
                    if (partner == null)
                    {
                        _toastNotification.AddErrorToastMessage("البيانات غير متوفرة");
                    }
                    else if (partner.Status.Id != 3)
                    {
                        _toastNotification.AddErrorToastMessage("لا يمكن إعادة تفعيل هذه الجهة بسبب حالتها الحالية");
                    }
                    else
                    {
                        var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
                        var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Reactive", currentRole);
                        if (permission == null)
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Details == null || permission.Details.Count == 0)
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "CurOpOnly")
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != _partnerManager.GetCurrentUserId(this.HttpContext))
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else
                        {
                            var insObj = new PartnerStatusLog();
                            insObj.CreatedBy.Id = _partnerManager.GetCurrentUserId(this.HttpContext);
                            insObj.CreatedBy.Account = _partnerManager.GetCurrentUserAccount(this.HttpContext);
                            insObj.Partner.Id = partner.Id;
                            insObj.Partner.Account = partner.Account;
                            insObj.OldStatus = partner.Status;
                            insObj.NewStatus.Id = 1;
                            insObj.Note = model.Note;
                            insObj.NewStatusExpireOn = model.NewStatusExpireOn;
                            var result = new PartnerStatusLogRepo(_db, _partnerManager).Create(insObj);
                            if (result.Success)
                            {
                                _toastNotification.AddSuccessToastMessage("تم إعادة تفعيل الجهة بنجاح");
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                _toastNotification.AddErrorToastMessage("فشلت عملية تغيير الحالة");
                            }
                        }
                    }
                }
            }
            model.NewStatus = new PartnerStatusRepo(_db).GetStatus(1);
            return View(model);
        }

        public IActionResult Confiscate()
        {
            var model = new ConfiscationDto();
            return View(model);
        }

        [HttpPost]
        public IActionResult Confiscate(ConfiscationDto model)
        {
            if (ModelState.IsValid)
            {
                if (!Utility.ValidYMobileNo(model.PartnerId))
                {
                    _toastNotification.AddErrorToastMessage("الرقم غير صحيح");
                }
                else
                {
                    var partner = _partnerManager.GetPartnerBasicInfo(model.PartnerId);
                    if (partner == null)
                    {
                        _toastNotification.AddErrorToastMessage("البيانات غير متوفرة");
                    }
                    else if (partner.Status.Id > 2)
                    {
                        _toastNotification.AddErrorToastMessage("لا يمكن مصادرة رصيد هذه الجهة بسبب حالتها الحالية");
                    }
                    else if (partner.Balance == 0)
                    {
                        _toastNotification.AddErrorToastMessage("لا يوجد رصيد للجهة يمكن مصادرته");
                    }
                    else
                    {
                        var currentRole = _partnerManager.GetCurrentUserRole(this.HttpContext);
                        var permission = new PartnerActivityRepo(_db).GetPartAct("Partner.Confiscate", currentRole);
                        if (permission == null)
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Details == null || permission.Details.Count == 0)
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "CurOpOnly")
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != _partnerManager.GetCurrentUserId(this.HttpContext))
                        {
                            _toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else
                        {
                            var insObj = new Confiscation();
                            insObj.CreatedBy.Id = _partnerManager.GetCurrentUserId(this.HttpContext);
                            insObj.CreatedBy.Account = _partnerManager.GetCurrentUserAccount(this.HttpContext);
                            insObj.Partner.Id = partner.Id;
                            insObj.Partner.Account = partner.Account;
                            insObj.Note = model.Note;
                            var result = new ConfiscationRepo(_db, _partnerManager).Create(insObj);
                            if (result.Success)
                            {
                                _toastNotification.AddSuccessToastMessage("تم مصاردة الرصيد بنجاح");
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                _toastNotification.AddErrorToastMessage("فشلت عملية مصادرة الرصيد");
                            }
                        }
                    }
                }

            }
            return View(model);
        }
        public IActionResult ChgStateQuery()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.ChangeStateQuery", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new PartnerStatusLogQueryDto();
            model.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(30));
            model.EndDate = DateTime.Today;
            return View(model);
        }
        [HttpPost]
        public IActionResult ChgStateQuery(PartnerStatusLogQueryDto model)
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.ChangeStateQuery", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var results = new PartnerStatusLogRepo(_db, _partnerManager).GetList(new PartnerStatusLogRepo.GetListParam
            {
                 CreatedByAccount = model.CreatedByAccount,
                 CreatedById = model.CreatedById,
                 PartnerId = model.PartnerId,
                 PartnerAccount = model.PartnerAccount,
                 IncludeDates = model.IncludeDates,
                 StartDate = model.StartDate,
                 EndDate = model.EndDate
            });
            model.results = results;
            return View(model);
        }

        public IActionResult Pfr()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.PFR.Query", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new PFRQueryDto();
            model.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(30));
            model.EndDate = DateTime.Today;
            model.Paging.PageNo = 1;
            model.Paging.PageSize = 50;
            return View(model);
        }
        [HttpPost]
        public IActionResult Pfr(PFRQueryDto model, [FromQuery
        (Name = "direction")] string direction)
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.PFR.Query", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            ModelState.Clear();
            if (direction == "pre" && model.Paging.PageNo > 1)
            {
                model.Paging.PageNo -= 1;
            }
            if (direction == "next")
            {
                model.Paging.PageNo += 1;
            }
            var results = new PFRRepo(_db).GetListWithPaging(model.PartnerAccount, model.IncludeDates, model.StartDate, model.EndDate, model.Paging);
            if (results != null)
            {
                model.Paging.Count = new PFRRepo(_db).GetCount(model.PartnerAccount, model.PartnerId, model.IncludeDates, model.StartDate, model.EndDate);
            }
            else
            {
                model.Paging.Count = 0;
            }
            model.results = results;
            return View(model);
        }

        public IActionResult CreatePfrReportPdf(int account, string id, bool includeDates, string startDate, string endDate)
        {
            var sDate = DateTime.Parse(startDate);
            var eDate = DateTime.Parse(endDate);
            var model = new PFRRepo(_db).GetList(account, includeDates, sDate, eDate);
            if (model == null) return Ok("غير موجود");
            var roleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.PFR.Print", roleId);
            var currUserId = _partnerManager.GetCurrentUserId(this.HttpContext);
            if (permission == null) return LocalRedirect("/Account/AccessDenied");
            //if (permission.Scope.Id != "Everyone" && model.CreatedBy.Id != currUserId) return LocalRedirect("/Account/AccessDenied");


            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PFR"

            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = new PFRTemplate(_db,_partnerManager, _environment, _partnerActivity).GetHTMLString(account, id, includeDates, sDate, eDate),
                WebSettings =
                {
                    DefaultEncoding = "utf-8",UserStyleSheet=Path.Combine(_environment.WebRootPath, "css","Reports","PFR.css")
                },
                //HeaderSettings = {FontName = "Arial", FontSize = 9, Right = "page [page] of [topage]",Line=true},
                FooterSettings = { FontName = "Arial", FontSize = 9, Right = "page [page] of [topage]", Line = true, Center = "Y Company" }
                
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);

            return File(file, "application/pdf");
        }


        public IActionResult Edit()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.Edit", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var idTypes = new IdTypeRepo(_db).GetTypes();
            var cities = new CityRepo(_db).GetCities();
            var model = new EditPartnerDto();
            model.IdTypes = idTypes;
            model.Cities = cities;
            model.Districts = new List<District>();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(EditPartnerDto model)
        {
            if (ModelState.IsValid)
            {
                var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = _partnerActivity.GetPartAct("Partner.Edit", currentRoleId);
                if (permission == null)
                {
                    _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (permission.Details == null || permission.Details.Count == 0)
                {
                    _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var target = _partnerManager.Validate(model.Id);
                if (!target.Success || target.Partner == null)
                {
                    _toastNotification.AddErrorToastMessage("لم يتم العثور على البيانات", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var allowEdit = permission.Details.Exists(m => m.ToRole.Id == target.Partner.Role.Id);
                if (!allowEdit)
                {
                    _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (permission.Scope.Id == "CurOpOnly" && model.Id != _partnerManager.GetCurrentUserId(this.HttpContext))
                {
                    _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (permission.Scope.Id == "Exclusive" && model.Id != target.Partner.RefPartner.Id)
                {
                    _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var oldResult = _partnerManager.Validate(model.Id);
                if (oldResult.Success)
                {
                    var newPartner = new Partner();
                    newPartner = ObjectCopier.CloneJson<Partner>(oldResult.Partner);
                    newPartner.Name = model.Name;
                    newPartner.PairMobile = model.PairMobile;
                    newPartner.BrandName = model.BrandName;
                    newPartner.PersonalId.IdType.Id = model.PersonalIdType ?? 0;
                    newPartner.PersonalId.Id = model.PersonalIdNo;
                    newPartner.PersonalId.Issued = model.PersonalIssued ?? DateTime.MinValue;
                    newPartner.PersonalId.Place = model.PersonalIdPlace;
                    newPartner.Address.Street = model.Street;
                    newPartner.Address.Zone = model.Zone;
                    newPartner.Address.ExtraInfo = model.ExtraAddressInfo;
                    newPartner.ContactInfo.Mobile = model.MobileNo;
                    newPartner.ContactInfo.Fixed = model.Fixed;
                    newPartner.ContactInfo.Fax = model.Fax;
                    newPartner.ContactInfo.Email = model.Email;
                    newPartner.RefPartner.Id = model.RefPartnerId;
                    newPartner.IPAddress = model.IPAddress;
                    var result = await _partnerManager.EditAsync(oldResult.Partner, newPartner);
                    if (result.Success)
                    {
                        newPartner = _partnerManager.GetPartnerByAccount(newPartner.Account);
                        var audit = new DataAudit();
                        audit.Activity.Id = "Partner.Edit";
                        audit.PartnerId = _partnerManager.GetCurrentUserId(this.HttpContext);
                        audit.PartnerAccount = _partnerManager.GetCurrentUserAccount(this.HttpContext);
                        audit.Action.Id = "Update";
                        audit.Success = true;
                        audit.OldValue = oldResult.Partner.ToString();
                        audit.NewValue = newPartner.ToString();
                        var auditResult = new DataAuditRepo(_db).Create(audit);
                        _toastNotification.AddSuccessToastMessage("تم تعديل البيانات بنجاح", new ToastrOptions
                        {
                            Title = "تنبيه"
                        });
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("فشلت عملية التعديل", new ToastrOptions
                        {
                            Title = "تنبيه"
                        });
                    }
                }
            }

            var idTypes = new IdTypeRepo(_db).GetTypes();
            var cities = new CityRepo(_db).GetCities();
            model.IdTypes = idTypes;
            model.Cities = cities;
            model.Districts = new List<District>();
            return View("Edit", model);
        }

        public IActionResult ConfiscationQuery()
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.Confiscate.Query", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new ConfiscationQueryDto();
            model.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(30));
            model.EndDate = DateTime.Today.AddDays(1);
            return View(model);
        }
        [HttpPost]
        public IActionResult ConfiscationQuery(ConfiscationQueryDto model)
        {
            var currentRoleId = _partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = _partnerActivity.GetPartAct("Partner.Confiscate.Query", currentRoleId);
            if (permission == null)
            {
                _toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var results = new ConfiscationRepo(_db, _partnerManager).GetList(new ConfiscationRepo.GetListParam
            {
                PartnerId = model.PartnerId,
                PartnerAccount = model.PartnerAccount,
                CreatorId = model.CreatedById,
                CreatorAccount = model.CreatedByAccount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IncludeDates = model.IncludeDates
            });
            model.Results = results;
            return View(model);
        }
    }
}

