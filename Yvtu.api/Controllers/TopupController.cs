using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Yvtu.api.Dtos;
using Yvtu.api.Errors;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Infra.Services;

namespace Yvtu.api.Controllers
{
    [Authorize]
    public class TopupController : BaseApiController
    {
        private readonly IAppDbContext _db;
        private readonly IPartnerManager _partnerManager;
        private readonly IConfiguration _config;

        public TopupController(IAppDbContext db,
              IPartnerManager partnerManager,
              IConfiguration config
             )
        {
            this._db = db;
            this._partnerManager = partnerManager;
            this._config = config;
        }

        [HttpPost("/api/topup/pay")]
        public async Task<IActionResult> Pay(TopupDto topup)
        {
            if (!Utility.ValidYMobileNo(topup.subsNo)) return BadRequest(new ApiResponse(-3000, "Sorry, the target mobile was wrong"));
            var currentUser = _partnerManager.GetPartnerById(this.HttpContext.User.Identity.Name);
            var recharge = new RechargeCollection();
            recharge.SubscriberNo = topup.subsNo;
            recharge.Amount = topup.amt;
            recharge.PointOfSale = currentUser;
            recharge.QueueNo = 0;
            recharge.AccessChannel.Id = "api";
            recharge.ApiTransaction = topup.seq;
            var result = new RechargeRepo(_db, _partnerManager).Create(recharge);
            if (!result.Success)
            {
                if (result.AffectedCount == -513 ) return Unauthorized(new ApiResponse(-3003, "Sorry, undefined rule"));
                if (result.AffectedCount == -501) return BadRequest(new ApiResponse(-3004, $"Sorry, your balance was not enough {(currentUser.Balance - currentUser.Reserved).ToString("N0")}"));
                if (result.AffectedCount == -502) return BadRequest(new ApiResponse(-3005, $"Sorry, amount less than min limit"));
                if (result.AffectedCount == -503) return BadRequest(new ApiResponse(-3006, "Sorry, amount more than max limit"));
                if (result.AffectedCount == -511) return BadRequest(new ApiResponse(-3007, "Sorry, your account was invalid"));
                if (result.AffectedCount == -512) return BadRequest(new ApiResponse(-3008, "Sorry, inconsistent data"));
                if (result.AffectedCount == -514) return BadRequest(new ApiResponse(-3009, $"Sorry, duplicated sequence {topup.seq}"));
                if (result.AffectedCount == -515) return BadRequest(new ApiResponse(-3010, $"Sorry, amount not allowed"));
            }
            recharge.Id = result.AffectedCount;
            // call web service 
            var endpoint = _config["OCS:Endpoint"];
            var apiUser = _config["OCS:User"];
            var apiPassword = _config["OCS:Password"];
            var remoteAddress = _config["OCS:RemoteAddress"];
            var successCode = _config["OCS:SuccessCode"];
            var payResult = await new TopupService(_db).DoRecharge(recharge, endpoint, apiUser, apiPassword, remoteAddress, successCode);
            return Ok(payResult);
        }

        [HttpGet("/api/topup/query/{id}")]
        public IActionResult TopupQuery(int id)
        {
            var currentUser = _partnerManager.GetPartnerById(this.HttpContext.User.Identity.Name);
            var collection = new RechargeRepo(_db, _partnerManager).GetRechargeByApiTransaction(id, currentUser.Account);

            if (collection == null) return Ok(new { resultCode = 0, resultDesc = "OK", success = "no", status = "notFound", queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") });
            if (collection.Status == 0) return Ok(new { resultCode = 0, resultDesc = "OK", success = "no", status = "pending", queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") });
            if (collection.Status > 1) return Ok(new { resultCode = 0, resultDesc = "OK", success = "no", status = "failed", queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") });

            return Ok( new {
                resultCode = 0,
                resultDesc = "OK",
                success = "yes",
                status = "done",
                data = new
                {
                    subsNo = collection.SubsNo,
                    amt = collection.Amount,
                    seq = collection.ApiTransaction
                },
                queryTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            });
        }
    }
}
