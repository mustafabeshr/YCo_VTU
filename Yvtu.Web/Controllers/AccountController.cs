using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IToastNotification toastNotification;
        private readonly IConverter converter;
        private readonly IWebHostEnvironment environment;

        public AccountController(IAppDbContext db
            ,IPartnerManager partner
            ,IPartnerActivityRepo partnerActivity
            , IToastNotification toastNotification
            , IConverter converter, IWebHostEnvironment environment)
        {
            this.db = db;
            this.partnerManager = partner;
            this.partnerActivity = partnerActivity;
            this.toastNotification = toastNotification;
            this.converter = converter;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new PartnerQuery();
            var roles = new RoleRepo(db, partnerActivity).GetRoles();
            var statuses = new PartnerStatusRepo(db).GetStatusList();
            model.Roles = roles;
            model.Statuses = statuses;
            return View(model);
        }

        public IActionResult Detail(int account)
        {
            var model = partnerManager.GetPartnerByAccount(account);
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPass(int account)
        {
            var model = new ResetPassDto();
            var roleCode = partnerManager.GetCurrentUserRoleCode(this.HttpContext);
            if (roleCode != "Admin")
            {
                model.Error = "ليس لديك الصلاحية الكافية";
                model.Success = string.Empty;
            }
            else
            {
                var partnerModel = this.partnerManager.GetPartnerByAccount(account);
                var persmission = partnerActivity.GetPartAct("Partner.ResetPassword", this.partnerManager.GetCurrentUserRole(this.HttpContext));

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
                    var result = partnerManager.ResetPassword(partnerModel);
                    if (result)
                    {
                        model.PartnerId = partnerModel.Id;
                        model.PartnerName = partnerModel.Name;
                        model.Error = string.Empty;
                        model.Success = "تم تغيير كلمة المرور";
                        toastNotification.AddSuccessToastMessage("تم اعادة تعيين كلمة المرور بنجاح");
                    }
                }
            }
            return base.View(model);
            
        }

        [HttpPost]
        public IActionResult Index(PartnerQuery model)
        {
            model.Partners = partnerManager.GetPartners(model);
            var roles = new RoleRepo(db, partnerActivity).GetRoles();
            var statuses = new PartnerStatusRepo(db).GetStatusList();
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
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("AppRole.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                      Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var lst = new RoleRepo(db, null).GetRoles();
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
                obj.PartnerCount = new RoleRepo(db, null).GetPartnerCount(item.Id);
                queryList.Add(obj);
            }
            return View(queryList);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            model.Error = string.Empty;
            if (ModelState.IsValid)
            {
                var partnerResult = this.partnerManager.Validate(model.Id);
                if (partnerResult.Success)
                {
                    if (partnerResult.Partner.Status.Id > 2)
                    {
                        toastNotification.AddInfoToastMessage("عذرا ، بحسب حالة الحساب لايمكنك استخدام النظام حاليا");
                        return View(model);
                    }
                    if (partnerResult.Partner.LockTime > DateTime.Now)
                    {
                        toastNotification.AddInfoToastMessage("عذرا ، حسابك متوقف مؤقتا لمدة  " + Utility.HowMuchLeftTime(partnerResult.Partner.LockTime));
                        toastNotification.AddErrorToastMessage("حسابك متوقف مؤقتا");
                        return View(model);
                    }

                        byte[] salt = Convert.FromBase64String(partnerResult.Partner.Extra);
                        string hash = Pbkdf2Hasher.ComputeHash(model.Pwd, salt);

                        if (partnerResult.Partner.Pwd != hash)
                        {
                            bool lockAccount = false;
                            if (partnerResult.Partner.WrongPwdAttempts >= 2) lockAccount = true;
                            partnerManager.IncreaseWrongPwdAttempts(partnerResult.Partner.Id, lockAccount);
                            toastNotification.AddInfoToastMessage("عذرا ، رمز المستخدم او كلمة المرور غير صحيح" + Environment.NewLine 
                                +"(" + partnerResult.Partner.WrongPwdAttempts + ")");
                            return View(model);
                        }

                        partnerManager.PreSuccessLogin(partnerResult.Partner.Id);

                        ClaimsIdentity identity = new ClaimsIdentity(partnerManager.GetUserClaims(partnerResult.Partner)
                            , CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal
                            , new AuthenticationProperties() { IsPersistent = model.RememberMe });

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
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.Create", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreatePartnerDto();
            var roles = new RoleRepo(db, partnerActivity).GetAuthorizedRoles("Partner.Create", currentRoleId);
            var idTypes = new IdTypeRepo(db).GetTypes();
            var cities = new CityRepo(db).GetCities();
         
            model.RefPartnerId = partnerManager.GetCurrentUserId(this.HttpContext);
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
                createdPartner.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                createdPartner.Status.Id = 1;
                createdPartner.StatusBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                createdPartner.IPAddress = model.IPAddress;
                createdPartner.RefPartner.Id = model.RefPartnerId;


                var result = await partnerManager.CreateAsync(createdPartner);
                if (result.Success)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            //var model = new CreatePartnerDto();
            var roles = new RoleRepo(db, partnerActivity).GetRoles();
            var idTypes = new IdTypeRepo(db).GetTypes();
            var cities = new CityRepo(db).GetCities();
            var districts = new DistrictRepo(db).GetDistrictsByCity(model.CityId ?? 0);
            model.Roles = roles;
            model.IdTypes = idTypes;
            model.Cities = cities;
            model.Districts = districts;
            return View(model);
        }

        public IActionResult GetDistrictsByCity(int id)
        {
            return Json(new DistrictRepo(db).GetDistrictsByCity(id));
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
                var currentPartner = partnerManager.GetPartnerByAccount(partnerManager.GetCurrentUserAccount(this.HttpContext));
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
                if (!System.Text.RegularExpressions.Regex.IsMatch( model.NewPass.ToString(), "^[0-9]{4,6}$"))
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
                    partnerManager.IncreaseWrongPwdAttempts(currentPartner.Id, lockAccount);
                    model.Error = "كلمة المرور القديمة غير صحيح" + Environment.NewLine + "(" + currentPartner.WrongPwdAttempts + ")";
                    return View(model);
                }

                partnerManager.ChangePwd(currentPartner.Account, currentPartner.Id, model.NewPass.ToString());
                toastNotification.AddSuccessToastMessage("تم تغيير كلمة المرور بنجاح");
            }

            return View(model);
        }


        public IActionResult Suspend()
        {
            var model = new CreateChangeStatusDto();
            model.NewStatus = new PartnerStatusRepo(db).GetStatus(3);
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
                    toastNotification.AddErrorToastMessage("الرقم غير صحيح");
                } else
                {
                    var partner = partnerManager.GetPartnerBasicInfo(model.PartnerId);
                    if (partner == null) 
                    {
                        toastNotification.AddErrorToastMessage("البيانات غير متوفرة");
                    }
                    else if (partner.Status.Id > 2)
                    {
                        toastNotification.AddErrorToastMessage("لا يمكن ايقاف نشاط هذه الجهة بسبب حالتها الحالية");
                    }
                    else
                    {
                        var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
                        var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Suspend", currentRole);
                        if (permission == null)
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        } else if (permission.Details == null || permission.Details.Count == 0)
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        } else if (permission.Scope.Id == "CurOpOnly")
                        {
                           toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != partnerManager.GetCurrentUserId(this.HttpContext))
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        } else
                        {
                            var insObj = new PartnerStatusLog();
                            insObj.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                            insObj.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                            insObj.Partner.Id = partner.Id;
                            insObj.Partner.Account = partner.Account;
                            insObj.OldStatus = partner.Status;
                            insObj.NewStatus.Id = 3;
                            insObj.Note = model.Note;
                            insObj.NewStatusExpireOn = model.NewStatusExpireOn;
                            var result = new PartnerStatusLogRepo(db, partnerManager).Create(insObj);
                            if (result.Success)
                            {
                                toastNotification.AddSuccessToastMessage("تم ايقاف النشاط بنجاح");
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                toastNotification.AddErrorToastMessage("فشلت عملية تغيير الحالة");
                            }
                        }
                    }
                }

            }
            model.NewStatus = new PartnerStatusRepo(db).GetStatus(3);
            return View(model);
        }

        public PartBasicInfo GetBasicInfo4S(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new PartBasicInfo { Error = "رقم غير صحيح" };
            var partner = partnerManager.GetPartnerBasicInfo(id);
            if (partner == null) return new PartBasicInfo { Error = "البيانات غير متوفرة" };
            var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Suspend", currentRole);
            if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            partner.Error = "N/A";
            return partner;
        }

        public PartBasicInfo GetBasicInfo4C(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new PartBasicInfo { Error = "رقم غير صحيح" };
            var partner = partnerManager.GetPartnerBasicInfo(id);
            if (partner == null) return new PartBasicInfo { Error = "البيانات غير متوفرة" };
            var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Cancel", currentRole);
            if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            partner.Error = "N/A";
            return partner;
        }

        public PartBasicInfo GetBasicInfo4A(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new PartBasicInfo { Error = "رقم غير صحيح" };
            var partner = partnerManager.GetPartnerBasicInfo(id);
            if (partner == null) return new PartBasicInfo { Error = "البيانات غير متوفرة" };
            var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Reactive", currentRole);
            if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            partner.Error = "N/A";
            return partner;
        }

        public PartBasicInfo GetBasicInfo4Co(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new PartBasicInfo { Error = "رقم غير صحيح" };
            var partner = partnerManager.GetPartnerBasicInfo(id);
            if (partner == null) return new PartBasicInfo { Error = "البيانات غير متوفرة" };
            var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Confiscate", currentRole);
            if (permission == null) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            if (permission.Details == null || permission.Details.Count == 0) return new PartBasicInfo { Error = "ليس لديك الصلاحية الكافية" };
            partner.Error = "N/A";
            return partner;
        }
        public Partner GetP(string id)
        {
            if (!Utility.ValidYMobileNo(id)) return new Partner { Extra = "رقم غير صحيح" };
            var getResult = partnerManager.Validate(id);
            if (getResult.Success)
            {
                var partner = getResult.Partner;
                var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Edit", currentRole);
                if (permission == null) return new Partner { Extra = "ليس لديك الصلاحية الكافية" };
                if (permission.Details == null || permission.Details.Count == 0) return new Partner { Extra = "ليس لديك الصلاحية الكافية" };
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
            model.NewStatus = new PartnerStatusRepo(db).GetStatus(4);
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
                    toastNotification.AddErrorToastMessage("الرقم غير صحيح");
                }
                else
                {
                    var partner = partnerManager.GetPartnerBasicInfo(model.PartnerId);
                    if (partner == null)
                    {
                        toastNotification.AddErrorToastMessage("البيانات غير متوفرة");
                    }
                    else if (partner.Status.Id > 2)
                    {
                        toastNotification.AddErrorToastMessage("لا يمكن ايقاف نشاط هذه الجهة بسبب حالتها الحالية");
                    }
                    else if (partner.Balance > 0)
                    {
                        toastNotification.AddErrorToastMessage("لا يمكن ايقاف نهائي لجهة لديها رصيد يجب اولا مصادرة الرصيد");
                    }
                    else
                    {
                        var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
                        var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Cancel", currentRole);
                        if (permission == null)
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Details == null || permission.Details.Count == 0)
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "CurOpOnly")
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != partnerManager.GetCurrentUserId(this.HttpContext))
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else
                        {
                            var insObj = new PartnerStatusLog();
                            insObj.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                            insObj.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                            insObj.Partner.Id = partner.Id;
                            insObj.Partner.Account = partner.Account;
                            insObj.OldStatus = partner.Status;
                            insObj.NewStatus.Id = 4;
                            insObj.Note = model.Note;
                            insObj.NewStatusExpireOn = model.NewStatusExpireOn;
                            var result = new PartnerStatusLogRepo(db, partnerManager).Create(insObj);
                            if (result.Success)
                            {
                                toastNotification.AddSuccessToastMessage("تم ايقاف النشاط بشكل نهائي بنجاح");
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                toastNotification.AddErrorToastMessage("فشلت عملية تغيير الحالة");
                            }
                        }
                    }
                }

            }
            model.NewStatus = new PartnerStatusRepo(db).GetStatus(4);
            return View(model);
        }

        public IActionResult Reactive()
        {
            var model = new CreateChangeStatusDto();
            model.NewStatus = new PartnerStatusRepo(db).GetStatus(1);
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
                    toastNotification.AddErrorToastMessage("الرقم غير صحيح");
                }
                else
                {
                    var partner = partnerManager.GetPartnerBasicInfo(model.PartnerId);
                    if (partner == null)
                    {
                        toastNotification.AddErrorToastMessage("البيانات غير متوفرة");
                    }
                    else if (partner.Status.Id != 3)
                    {
                        toastNotification.AddErrorToastMessage("لا يمكن إعادة تفعيل هذه الجهة بسبب حالتها الحالية");
                    }
                    else
                    {
                        var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
                        var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Reactive", currentRole);
                        if (permission == null)
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Details == null || permission.Details.Count == 0)
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "CurOpOnly")
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != partnerManager.GetCurrentUserId(this.HttpContext))
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else
                        {
                            var insObj = new PartnerStatusLog();
                            insObj.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                            insObj.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                            insObj.Partner.Id = partner.Id;
                            insObj.Partner.Account = partner.Account;
                            insObj.OldStatus = partner.Status;
                            insObj.NewStatus.Id = 1;
                            insObj.Note = model.Note;
                            insObj.NewStatusExpireOn = model.NewStatusExpireOn;
                            var result = new PartnerStatusLogRepo(db, partnerManager).Create(insObj);
                            if (result.Success)
                            {
                                toastNotification.AddSuccessToastMessage("تم إعادة تفعيل الجهة بنجاح");
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                toastNotification.AddErrorToastMessage("فشلت عملية تغيير الحالة");
                            }
                        }
                    }
                }
            }
            model.NewStatus = new PartnerStatusRepo(db).GetStatus(1);
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
                    toastNotification.AddErrorToastMessage("الرقم غير صحيح");
                }
                else
                {
                    var partner = partnerManager.GetPartnerBasicInfo(model.PartnerId);
                    if (partner == null)
                    {
                        toastNotification.AddErrorToastMessage("البيانات غير متوفرة");
                    }
                    else if (partner.Status.Id > 2)
                    {
                        toastNotification.AddErrorToastMessage("لا يمكن مصادرة رصيد هذه الجهة بسبب حالتها الحالية");
                    }
                    else if (partner.Balance == 0)
                    {
                        toastNotification.AddErrorToastMessage("لا يوجد رصيد للجهة يمكن مصادرته");
                    }
                    else
                    {
                        var currentRole = partnerManager.GetCurrentUserRole(this.HttpContext);
                        var permission = new PartnerActivityRepo(db).GetPartAct("Partner.Confiscate", currentRole);
                        if (permission == null)
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Details == null || permission.Details.Count == 0)
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "CurOpOnly")
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else if (permission.Scope.Id == "Exclusive" && partner.RefPartnerId != partnerManager.GetCurrentUserId(this.HttpContext))
                        {
                            toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية");
                        }
                        else
                        {
                            var insObj = new Confiscation();
                            insObj.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                            insObj.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                            insObj.Partner.Id = partner.Id;
                            insObj.Partner.Account = partner.Account;
                            insObj.Note = model.Note;
                            var result = new ConfiscationRepo(db, partnerManager).Create(insObj);
                            if (result.Success)
                            {
                                toastNotification.AddSuccessToastMessage("تم مصاردة الرصيد بنجاح");
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                toastNotification.AddErrorToastMessage("فشلت عملية مصادرة الرصيد");
                            }
                        }
                    }
                }

            }
            return View(model);
        }


        public IActionResult ChgStateQuery()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.ChangeStateQuery", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
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
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.ChangeStateQuery", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var results = new PartnerStatusLogRepo(db, partnerManager).GetList(new PartnerStatusLogRepo.GetListParam
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

        public IActionResult PFR()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.PFR.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new PFRQueryDto();
            model.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(30));
            model.EndDate = DateTime.Today;
            return View(model);
        }
        [HttpPost]
        public IActionResult PFR(PFRQueryDto model)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.PFR.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var results = new PFRRepo(db).GetList(model.PartnerAccount, model.PartnerId, model.IncludeDates, model.StartDate, model.EndDate);
            model.results = results;
            return View(model);
        }

        public IActionResult CreatePFRReportPDF(int account, string id, bool includeDates, string startDate, string endDate)
        {
            var sDate = DateTime.Parse(startDate);
            var eDate = DateTime.Parse(endDate);
            var model = new PFRRepo(db).GetList(account, id, includeDates, sDate, eDate);
            if (model == null) return Ok("غير موجود");
            var roleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.PFR.Print", roleId);
            var currUserId = partnerManager.GetCurrentUserId(this.HttpContext);
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
                HtmlContent = new PFRTemplate(db,partnerManager, environment, partnerActivity).GetHTMLString(account, id, includeDates, sDate, eDate),
                WebSettings =
                {
                    DefaultEncoding = "utf-8",UserStyleSheet=Path.Combine(environment.WebRootPath, "css","Reports","PFR.css")
                },
                //HeaderSettings = {FontName = "Arial", FontSize = 9, Right = "page [page] of [topage]",Line=true},
                FooterSettings = { FontName = "Arial", FontSize = 9, Right = "page [page] of [topage]", Line = true, Center = "Y Company" },


            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = converter.Convert(pdf);

            return File(file, "application/pdf");
        }


        public IActionResult Edit()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.Edit", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = "تنبيه"
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var idTypes = new IdTypeRepo(db).GetTypes();
            var cities = new CityRepo(db).GetCities();
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
                var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = partnerActivity.GetPartAct("Partner.Edit", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (permission.Details == null || permission.Details.Count == 0)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var target = partnerManager.Validate(model.Id);
                if (!target.Success || target.Partner == null)
                {
                    toastNotification.AddErrorToastMessage("لم يتم العثور على البيانات", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var allowEdit = permission.Details.Exists(m => m.ToRole.Id == target.Partner.Role.Id);
                if (!allowEdit)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (permission.Scope.Id == "CurOpOnly" && model.Id != partnerManager.GetCurrentUserId(this.HttpContext))
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (permission.Scope.Id == "Exclusive" && model.Id != target.Partner.RefPartner.Id)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = "تنبيه"
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var oldResult = partnerManager.Validate(model.Id);
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
                    var result = await partnerManager.EditAsync(oldResult.Partner, newPartner);
                    if (result.Success)
                    {
                        toastNotification.AddSuccessToastMessage("تم تعديل البيانات بنجاح", new ToastrOptions
                        {
                            Title = "تنبيه"
                        });
                    }
                    else
                    {
                        toastNotification.AddErrorToastMessage("فشلت عملية التعديل", new ToastrOptions
                        {
                            Title = "تنبيه"
                        });
                    }
                }
            }

            var idTypes = new IdTypeRepo(db).GetTypes();
            var cities = new CityRepo(db).GetCities();
            model.IdTypes = idTypes;
            model.Cities = cities;
            model.Districts = new List<District>();
            return View(model);
        }
    }
}

