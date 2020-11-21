﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class MoneyTransferController : Controller
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;
        private readonly IPartnerActivityRepo _partnerActivity;

        public MoneyTransferController(IAppDbContext db, IPartnerManager partnerManager
            , IPartnerActivityRepo partnerActivity)
        {
            this._db = db;
            this._partnerManager = partnerManager;
            this._partnerActivity = partnerActivity;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateMoneyTransferDto();
            var currentPartId = _partnerManager.GetCurrentUserId(this.HttpContext);
            var currentPart = _partnerManager.Validate(currentPartId).Partner;
            model.CreatorBalance = currentPart.Balance - currentPart.Reserved;
            var payTypes = new CommonCodeRepo(_db).GetCodesByType("pay.type");
            model.PayType = payTypes;

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
                    return View(model);
                }
                if (model.Amount <= 0)
                {
                    model.Error = "المبلغ غير صحيح";
                    return View(model);
                }
                var result = GetBasicInfo(model.PartnerId, model.Amount);
                if (result.Error != "N/A")
                {
                    model.Error =result.Error;
                    return View(model);
                }
                result.AmountName = new MonyToString().NumToStr(result.Amount);
                result.PayTypeName = new CommonCodeRepo(_db).GetCodesById(model.PayTypeId).Name;
                result.PayTypeId = model.PayTypeId;
                result.PayNo = model.PayNo;
                result.PayDate = model.PayDate;
                result.PayBank = model.PayBank;
                result.Note = model.Note;
                result.BillNo = model.BillNo;
                result.RequestNo = model.RequestNo;
                result.RequestAmount = model.RequestAmount;
                return View("Confirmation", result);
            }
            var payTypes = new CommonCodeRepo(_db).GetCodesByType("pay.type");
            model.PayType = payTypes;
            return View(model);
        }

        [HttpPost]
        public IActionResult Confirmation(CreateMoneyTransferDto model)
        {
            return null;
        }

        public CreateMoneyTransferDto GetBasicInfo(string pId, double amount = 0)
        {
            if (pId.Length != 9) return new CreateMoneyTransferDto { Error = "رقم خاطئ" };
            var validateResult =  _partnerManager.Validate(pId);
            if (validateResult.Success)
            {
                var partner = validateResult.Partner;
                var roleId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.GivenName).Value;
                var moneyTransferSettings = _partnerActivity.GetPartAct("Money.Transfer", roleId, partner.Role.Id.ToString());
                if (moneyTransferSettings == null) return new CreateMoneyTransferDto { Error = "لم يتم تعريف هذا الاجراء او ليس لديك الصلاحية الكافية" };
                
                var model = new CreateMoneyTransferDto
                {
                    PartnerId = partner.Id,
                    PartnerName = partner.Name,
                    PartnerRoleName = partner.Role.Name,
                    PartnerBalance = partner.Balance,
                    TaxPercent = moneyTransferSettings.TaxPercent,
                    BonusPercent = moneyTransferSettings.BonusPercent,
                    BounsTaxPercent = moneyTransferSettings.BonusTaxPercent,
                    Error = "N/A"
                };
                if (amount <= 0) return model;

                if (moneyTransferSettings.MaxValue > 0 && amount > moneyTransferSettings.MaxValue)
                {
                    model.Error = $"المبلغ اكبر من الاحد الاعلى المسموح به {moneyTransferSettings.MaxValue} " ;
                    return model;
                }
                if (moneyTransferSettings.MinValue > 0 && amount < moneyTransferSettings.MinValue)
                {
                    model.Error = $"المبلغ اقل من الاحد الادنى المسموح به {moneyTransferSettings.MinValue} ";
                    return model;
                }

                var currPartId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value;
                var currPart = _partnerManager.Validate(currPartId).Partner;
                model.CreateorId = currPartId;
                model.CreateorName = currPart.Name;
                model.CreateorRoleId = currPart.Role.Id;
                model.CreateorRoleName = currPart.Role.Name;
                model.CreatorBalance = currPart.Balance - currPart.Reserved;
                if (moneyTransferSettings.CheckBalanceRequired)
                {
                    
                    if (amount > model.CreatorBalance)
                    {
                        model.Error = $"رصيدك غير كافي { model.CreatorBalance } ";
                        return model;
                    }
                }

                var netAmount = amount / ((moneyTransferSettings.TaxPercent / 100) + 1) ;
                var taxAmount = netAmount * (moneyTransferSettings.TaxPercent / 100);
                var bounsAmount = netAmount * (moneyTransferSettings.BonusPercent / 100);
                var bounsTaxAmount = bounsAmount * (moneyTransferSettings.BonusTaxPercent / 100);
                var recievedAmount = (amount - bounsAmount + bounsTaxAmount);

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
    }
}
