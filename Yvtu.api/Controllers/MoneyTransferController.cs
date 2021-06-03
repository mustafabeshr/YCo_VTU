using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yvtu.api.Dtos;
using Yvtu.api.Errors;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
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
            moneyTransfer.Partner.Balance = _partnerManager.GetBalance(moneyTransfer.Partner.Account);
            moneyTransfer.CreatedBy.Balance = _partnerManager.GetBalance(moneyTransfer.CreatedBy.Account);
            new NotificationRepo(_db, _partnerManager).SendNotification<MoneyTransfer>("MoneyTransfer.Create", result.AffectedCount, moneyTransfer);

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

        [HttpPost("/api/mt/myreport")]
        public async Task<IActionResult> MyReport(MoneyTransferMyReportDto param)
        {
            try
            {
                if (!Utility.IsValidDate(param.StartDate))
                    return BadRequest(new ApiResponse(-3011, $"Sorry, start date is invalid {param.StartDate}"));
                if (!Utility.IsValidDate(param.EndDate))
                    return BadRequest(new ApiResponse(-3012, $"Sorry, end date is invalid {param.EndDate}"));
                var startDate = DateTime.ParseExact(param.StartDate, "dd/MM/yyyy", null); //DateTime.Parse(param.StartDate);
                var endDate = DateTime.ParseExact(param.EndDate, "dd/MM/yyyy", null);// DateTime.Parse(param.EndDate);
                var maxDaysSettings = new AppGlobalSettingsRepo(_db).GetSingle("API.MoneyTransfer.Report.MaxDays");
                if (maxDaysSettings != null)
                {
                    if ((endDate - startDate).TotalDays > int.Parse(maxDaysSettings.SettingValue) &&
                        int.Parse(maxDaysSettings.SettingValue) > 0)
                    {
                        return BadRequest(new ApiResponse(-3013,
                            $"Sorry, The report period is greater than the limit {int.Parse(maxDaysSettings.SettingValue)} day(s)"));
                    }
                }

                var currentUser = _partnerManager.GetPartnerById(this.HttpContext.User.Identity.Name);
                if (currentUser == null) return Unauthorized(new ApiResponse(-3003, "Sorry, re-login required"));
                var permission = _partnerActivity.GetPartAct("MoneyTransfer.API.MyReport", currentUser.Role.Id);
                if (permission == null) return Unauthorized(new ApiResponse(401));
                var maxRecordsSettings =
                    new AppGlobalSettingsRepo(_db).GetSingle("API.MoneyTransfer.Report.MaxResults");
                int maxRecords = maxRecordsSettings == null ? 200 : int.Parse(maxRecordsSettings.SettingValue);

                var results = await new MoneyTransferRepo(_db, _partnerManager, _partnerActivity).GetMoneyTransfers(
                    currentUser.Account, startDate, endDate, maxRecords);
                if (results == null || results.Count <= 0) return Ok(
                        new
                        {
                            resultCode = 0,
                            resultDesc = "No data",
                            success = "yes",
                            queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                            data = new object()
                        });
                var retResults = new List<object>();
                foreach (var obj in results)
                {
                    retResults.Add(new
                    {
                        transferTime = obj.CreatedOn,
                        transferId = obj.Id,
                        seq = obj.ApiTransaction,
                        sourceAccount = obj.CreatedBy.Account,
                        sourceMobile = obj.CreatedBy.Id,
                        sourceName = obj.CreatedBy.Name,
                        sourceBalance = obj.CreatedBy.Balance,
                        targetAccount = obj.Partner.Account,
                        targetMobile = obj.Partner.Id,
                        targetName = obj.Partner.Name,
                        targetBalance = obj.Partner.Balance,
                        amount = obj.Amount,
                        netAmount = obj.NetAmount,
                        taxPercent = obj.TaxPercent,
                        taxAmount = obj.TaxAmount,
                        bonusPercent = obj.BonusPercent,
                        bounsAmount = obj.BounsAmount,
                        bounsTaxPercent = obj.BounsTaxPercent,
                        bounsTaxAmount = obj.BounsTaxAmount,
                        note = obj.Note
                    });
                }

                return Ok(
                    new {
                            resultCode = 0,
                            resultDesc = "OK",
                            success = "yes",
                            queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                            data = retResults
                    });
            }
            catch (Exception ex)
            {
              return  BadRequest(new ApiResponse(-9999, ex.Message));
            }
            
        }
    }
}
