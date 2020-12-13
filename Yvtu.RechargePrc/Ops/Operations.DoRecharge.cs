using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.RechargePrc.Dtos;

namespace Yvtu.RechargePrc.Ops
{
    public partial class Operations
    {
        private readonly IAppDbContext db;

        public Operations(IAppDbContext db)
        {
            this.db = db;
        }
        public RechargeResponseDto DoRecharge(GrappedRecharge recharge)
        {

            Thread.Sleep(2000);
            #region Local test
            var result = new RechargeResponseDto()
            {
                ResultCode = 0,
                ResultDesc = "suceess",
                Duration = 500
            };

            var collection = new RechargeCollection();
            collection.Id = recharge.MasterId;
            collection.Status.Id = 1;
            collection.RefNo = result.ResultCode.ToString();
            collection.RefMessage = result.ResultDesc; ;
            collection.RefTransNo = result.TransNo;
            collection.RefTime = DateTime.Now;
            collection.DebugInfo = "success duration("+result.Duration+")";

            var dbResult = new RechargeRepo(db, null).UpdateWithBalance(collection);

            return result;

            #endregion
        }
    }
}
