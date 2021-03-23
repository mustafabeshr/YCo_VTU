using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Yvtu.api.Dtos;
using Yvtu.api.Errors;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.api.Controllers
{
    [Authorize]
    public class MoneyTransferController : BaseApiController
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;
        private readonly IPartnerActivityRepo _partnerActivity;

        public MoneyTransferController(IAppDbContext db,
              IPartnerManager partnerManager,
             IPartnerActivityRepo partnerActivity)
        {
            this._db = db;
            this._partnerManager = partnerManager;
            this._partnerActivity = partnerActivity;
        }


        [HttpPost("/api/mt/transfer")]
        public IActionResult Transfer(MoneyTransferDto mt)
        {
            if (!Utility.ValidYMobileNo(mt.pid)) return BadRequest(new ApiResponse(-3000, "Sorry, the target mobile was wrong"));
            if (mt.amt <= 0) return BadRequest(new ApiResponse(-3001, "Sorry, the amount was wrong"));
            var partnerResult = this._partnerManager.Validate(mt.pid);
            if (!partnerResult.Success) return BadRequest(new ApiResponse(-3002, "Sorry, the target pos was wrong"));

            var currentUser = _partnerManager.GetPartnerById(this.HttpContext.User.Identity.Name);

            var moneyTransfer = new MoneyTransfer();
            moneyTransfer.Partner = partnerResult.Partner;
            moneyTransfer.PayType.Id = "cash";
            moneyTransfer.PayNo = "0000";
            moneyTransfer.PayDate = DateTime.Now;
            moneyTransfer.PayBank = "";
            moneyTransfer.CreatedBy = currentUser;
            moneyTransfer.AccessChannel.Id = "api";
            moneyTransfer.Amount = mt.amt;
            moneyTransfer.BillNo = "00";
            moneyTransfer.RequestNo = "00";
            moneyTransfer.RequestAmount = mt.amt;
            moneyTransfer.Note = mt.note;
            moneyTransfer.ApiTransaction = mt.seq;

            var result = new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).Create(moneyTransfer);
            if (!result.Success)
            {
                if (result.AffectedCount == -500) return BadRequest(new ApiResponse(-3003, "Sorry, undefined rule"));
                if (result.AffectedCount == -501) return BadRequest(new ApiResponse(-3004, $"Sorry, your balance was not enough {(currentUser.Balance - currentUser.Reserved).ToString("N0")}"));
                if (result.AffectedCount == -502) return BadRequest(new ApiResponse(-3005, $"Sorry, amount less than min limit"));
                if (result.AffectedCount == -503) return BadRequest(new ApiResponse(-3006, "Sorry, amount more than max limit"));
                if (result.AffectedCount == -506) return BadRequest(new ApiResponse(-3007, "Sorry, your account was invalid"));
                if (result.AffectedCount == -507) return BadRequest(new ApiResponse(-3008, "Sorry, inconsistent data"));
                if (result.AffectedCount == -508) return BadRequest(new ApiResponse(-3009, $"Sorry, duplicated sequence {moneyTransfer.ApiTransaction}"));
            }

            moneyTransfer = new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).GetSingleOrDefault(result.AffectedCount);

            return Ok(new { 
                resultCode = 0,
                resultDesc = "OK",
                transferId = moneyTransfer.Id,
                seq = moneyTransfer.ApiTransaction,
                from = moneyTransfer.CreatedBy.Id +" | "+ moneyTransfer.CreatedBy.Account,
                to = moneyTransfer.Partner.Id + " | " + moneyTransfer.Partner.Account,
                amt = moneyTransfer.Amount,
                netAmt = moneyTransfer.NetAmount,
                recAmt = moneyTransfer.ReceivedAmount,
                taxPer = moneyTransfer.TaxPercent,
                taxAmt = moneyTransfer.TaxAmount,
                bonusPer = moneyTransfer.BonusPercent,
                bonusAmt = moneyTransfer.BounsAmount,
                bonusTaxPer = moneyTransfer.BounsTaxPercent,
                bonusTaxAmt = moneyTransfer.BounsTaxAmount,
                yourBal = (moneyTransfer.CreatedBy.Balance - moneyTransfer.CreatedBy.Reserved)
            });
        }

        [HttpGet("/api/mt/query/{id}")]
        public IActionResult TransferQuery(int id)
        {
            var currentUser = _partnerManager.GetPartnerById(this.HttpContext.User.Identity.Name);
            var moneyTransfer = new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).GetByApiTransaction(id, currentUser.Account);
            
            if (moneyTransfer == null) return Ok(new { resultCode = 0, resultDesc = "OK", success = "no", queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") });

            return Ok(new
            {
                resultCode = 0,
                resultDesc = "OK",
                success = "yes",
                data = new
                {
                    transferTime = moneyTransfer.CreatedOn.ToString("yyyy/MM/dd HH:mm:ss"),
                    transferId = moneyTransfer.Id,
                    seq = moneyTransfer.ApiTransaction,
                    from = moneyTransfer.CreatedBy.Id + " | " + moneyTransfer.CreatedBy.Account,
                    to = moneyTransfer.Partner.Id + " | " + moneyTransfer.Partner.Account,
                    amt = moneyTransfer.Amount,
                    netAmt = moneyTransfer.NetAmount,
                    recAmt = moneyTransfer.ReceivedAmount,
                    taxPer = moneyTransfer.TaxPercent,
                    taxAmt = moneyTransfer.TaxAmount,
                    bonusPer = moneyTransfer.BonusPercent,
                    bonusAmt = moneyTransfer.BounsAmount,
                    bonusTaxPer = moneyTransfer.BounsTaxPercent,
                    bonusTaxAmt = moneyTransfer.BounsTaxAmount
                },
                queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            });
        }
    }
}
