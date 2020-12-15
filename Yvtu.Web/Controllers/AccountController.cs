using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partner;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IToastNotification toastNotification;

        public AccountController(IAppDbContext db
            ,IPartnerManager partner
            ,IPartnerActivityRepo partnerActivity
            , IToastNotification toastNotification)
        {
            this.db = db;
            this.partner = partner;
            this.partnerActivity = partnerActivity;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            var currentRoleId = partner.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.Query", currentRoleId);
            if (permission == null)
            {
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
            var model = partner.GetPartnerByAccount(account);
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPass(int account)
        {
            var model = new ResetPassDto();
            var roleCode = partner.GetCurrentUserRoleCode(this.HttpContext);
            if (roleCode != "Admin")
            {
                model.Error = "ليس لديك الصلاحية الكافية";
                model.Success = string.Empty;
            }
            else
            {
                var partnerModel = this.partner.GetPartnerByAccount(account);
                var persmission = partnerActivity.GetPartAct("Partner.ResetPassword", this.partner.GetCurrentUserRole(this.HttpContext));

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
                    var result = partner.ResetPassword(partnerModel);
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
            model.Partners = partner.GetPartners(model);
            var roles = new RoleRepo(db, partnerActivity).GetRoles();
            var statuses = new PartnerStatusRepo(db).GetStatusList();
            model.Roles = roles;
            model.Statuses = statuses;
            return View(model);
        }

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
            var currentRoleId = partner.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("AppRole.Query", currentRoleId);
            if (permission == null)
            {
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



        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            model.Error = string.Empty;
            if (ModelState.IsValid)
            {
                var partnerResult = this.partner.Validate(model.Id);
                if (partnerResult.Success)
                {
                    if (partnerResult.Partner.Status.Id > 2)
                    {
                        model.Error = "عذرا ، بحسب حالة الحساب لايمكنك استخدام النظام حاليا";
                        return View(model);
                    }
                    if (partnerResult.Partner.LockTime > DateTime.Now)
                    {
                        model.Error = "عذرا ، حسابك متوقف مؤقتا لمدة  " + Utility.HowMuchLeftTime(partnerResult.Partner.LockTime);
                        toastNotification.AddErrorToastMessage("حسابك متوقف مؤقتا");
                        return View(model);
                    }

                        byte[] salt = Convert.FromBase64String(partnerResult.Partner.Extra);
                        string hash = Pbkdf2Hasher.ComputeHash(model.Pwd, salt);


                        if (partnerResult.Partner.Pwd != hash)
                        {
                            bool lockAccount = false;
                            if (partnerResult.Partner.WrongPwdAttempts >= 2) lockAccount = true;
                            partner.IncreaseWrongPwdAttempts(partnerResult.Partner.Id, lockAccount);
                            model.Error = "عذرا ، رمز المستخدم او كلمة المرور غير صحيح" + Environment.NewLine +"(" + partnerResult.Partner.WrongPwdAttempts + ")";
                            return View(model);
                        }

                            partner.PreSuccessLogin(partnerResult.Partner.Id);

                            ClaimsIdentity identity = new ClaimsIdentity(partner.GetUserClaims(partnerResult.Partner)
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
            var currentRoleId = partner.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Partner.Create", currentRoleId);
            if (permission == null)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreatePartnerDto();
            var roles = new RoleRepo(db, partnerActivity).GetAuthorizedRoles("Partner.Create", currentRoleId);
            var idTypes = new IdTypeRepo(db).GetTypes();
            var cities = new CityRepo(db).GetCities();
         
            model.RefPartnerId = partner.GetCurrentUserId(this.HttpContext);
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
                createdPartner.CreatedBy.Id = partner.GetCurrentUserId(this.HttpContext);
                createdPartner.Status.Id = 1;
                createdPartner.StatusBy.Id = partner.GetCurrentUserId(this.HttpContext);
                createdPartner.IPAddress = model.IPAddress;
                createdPartner.RefPartner.Id = model.RefPartnerId;


                var result = await partner.CreateAsync(createdPartner);
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
                var currentPartner = partner.GetPartnerByAccount(partner.GetCurrentUserAccount(this.HttpContext));
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
                    partner.IncreaseWrongPwdAttempts(currentPartner.Id, lockAccount);
                    model.Error = "كلمة المرور القديمة غير صحيح" + Environment.NewLine + "(" + currentPartner.WrongPwdAttempts + ")";
                    return View(model);
                }

                partner.ChangePwd(currentPartner.Account, currentPartner.Id, model.NewPass.ToString());
                toastNotification.AddSuccessToastMessage("تم تغيير كلمة المرور بنجاح");
            }

            return View(model);
        }
    }
}
