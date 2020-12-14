using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.RechargePrc.Dtos;

namespace Yvtu.RechargePrc.Ops
{
    public partial class Operations
    {
        public string resultDisplayMsg { get; set; }
        private readonly IAppDbContext db;

        public Operations(IAppDbContext db)
        {
            this.db = db;
        }
        public async Task<RechargeResponseDto> DoRecharge(GrappedRecharge recharge)
        {

            Thread.Sleep(2000);
            #region Local test
            var result = new RechargeResponseDto()
            {
                ResultCode = 212,
                ResultDesc = "Exception has been occured"
            };
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //await HTTPClientWrapper<RechargeResponseDto>.Get("");
            watch.Stop();
            double elapsedMs = watch.ElapsedMilliseconds;
            var collection = new RechargeCollection();
            collection.Id = recharge.MasterId;
            collection.Status.Id = result.ResultCode == 0 ? 1 : 2;
            collection.RefNo = result.ResultCode.ToString();
            collection.RefMessage = result.ResultDesc;
            collection.RefTransNo = result.TransNo;
            collection.RefTime = DateTime.Now;
            collection.DebugInfo = result.ResultDesc + " OCS(" + elapsedMs + ")";
            resultDisplayMsg = collection.DebugInfo;
            elapsedMs = 0;
            watch = System.Diagnostics.Stopwatch.StartNew();
            var dbResult = new RechargeRepo(db, null).UpdateWithBalance(collection);
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            resultDisplayMsg  += " DB(" + elapsedMs + ")";
            return result;

            #endregion
        }
    }
}
