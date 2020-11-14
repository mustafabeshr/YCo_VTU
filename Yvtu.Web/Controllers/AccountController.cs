using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partner;

        public AccountController(IAppDbContext db
            ,IPartnerManager partner)
        {
            this.db = db;
            this.partner = partner;
        }
        public IActionResult Index()
        {
            return View();
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
            var model = new CreatePartnerDto();
            var roles = new RoleRepo(db).GetRoles();
            var idTypes = new IdTypeRepo(db).GetTypes();
            var cities = new CityRepo(db).GetCities();
            //var districts = new DistrictRepo(db).GetDistricts();
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
                createdPartner.CreatedBy.Id = "777010055";
                createdPartner.Status.Id = 1;
                createdPartner.StatusBy.Id = "777010055";


                var result = await partner.CreateAsync(createdPartner);
                if (result.Success)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            //var model = new CreatePartnerDto();
            var roles = new RoleRepo(db).GetRoles();
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
    }
}
