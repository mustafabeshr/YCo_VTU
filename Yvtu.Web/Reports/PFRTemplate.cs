using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Web.Reports
{
    public  class PFRTemplate
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IWebHostEnvironment environment;
        private readonly IPartnerActivityRepo partnerActivity;

        public PFRTemplate(IAppDbContext db, IPartnerManager partnerManager, IWebHostEnvironment environment, IPartnerActivityRepo partnerActivity)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.environment = environment;
            this.partnerActivity = partnerActivity;
        }
       
        public  string GetHTMLString(int account, string id, bool includeDates, DateTime startDate, DateTime endDate)
        {
            int partAccount = 0;
            string partName = string.Empty;
            string partId = string.Empty;

            if (account > 0)
            {
                var partner = partnerManager.GetPartnerByAccount(account);
                partAccount = partner.Account;
                partName = partner.Name;
                partId = partner.Id;
            }
            else if (!string.IsNullOrEmpty(id))
            {
                var partner = partnerManager.GetPartnerBasicInfo(id);
                partAccount = partner.Account;
                partName = partner.Name;
                partId = partner.Id;
            }
            var result = new PFRRepo(db).GetList(account, includeDates, startDate, endDate);
            var resultSb = new StringBuilder();
            //double accumolator = 0;
            var periodTitle = @"<tr style='font-size:14px;'><td colspan='3'>للفترة من " + startDate.ToString("yyyy/MM/dd") + " الى " + endDate.ToString("yyyy/MM/dd") + "</td></tr>";
            if (result != null && result.Count > 0)
            {
                resultSb.Append(@"<table class='detail_table'> <thead> <tr style='font-size:12px;'>
                    <th>الرصيد</th>
                    <th>المبلغ</th>
                    <th>التاريخ</th>
                    <th>الاجراء</th>
                    <th>الحساب</th>
                    <th>الحساب</th>
                    <th>الرقم</th>
                    <th>الاسم</th>
                    <th>#المرجع</th>
                    </tr>
                    </thead>
                    <tbody>");
                foreach (var item in result)
                {
                    //accumolator += item.Amount;
                    resultSb.Append(@"
                                    <tr style='font-size:14px;'>
                                    <td style='font-size:14px;width:5%;padding:5px;'>" + item.Balance.ToString("N2") + @"</td>
                                    <td style='font-size:14px;width:5%;padding:5px;'>" + item.Amount.ToString("N2")+ @"</td>
                                    <td style='font-size:14px;width:7%;padding:5px;'>" + item.CreatedOn.ToString("yyyy/MM/dd ss:mm:H")+ @"</td>
                                    <td style='text-align:right;font-size:14px;width:9%;padding:5px;'>" + item.ActivityName+ @"</td>
                                    <td style='font-size:14px;width:5%;padding:5px;'>" + item.PartnerAccount+ @"</td>
                                    <td style='font-size:14px;width:5%;padding:5px;'>" + item.CreatedBy.Account+ @"</td>
                                    <td style='font-size:14px;width:5%;padding:5px;'>" + item.CreatedBy.Id + @"</td>
                                    <td style='text-align:right;font-size:14px;width:10%;padding:5px;'>" + item.CreatedBy.Name + @"</td>
                                    <td style='font-size:14px;width:5%;padding:5px;'>" + item.TransNo + @"</td>
                                    </tr>");
                }
                resultSb.Append(@"</tbody><table>");
            }
            var sb = new StringBuilder();
            sb.Append(@" <html>
 <head></head>
<body>
<table class='header_table'>
<tbody>
<tr>
  <td><img src='" + Path.Combine(environment.WebRootPath, "images", "Y-Logo2.png") + @"' alt='YCo Logo' class='brand-image' width='70' height='70' style='opacity: .8'></td>
  <td><h1> كشف حساب شاحن فوري</h1></td>
  <td>
<div style='display:bloack;'>" + DateTime.Now.ToString("yyyy/MM/dd") + @"</div>
<div style='display:bloack;'>" + DateTime.Now.ToString("ss:mm:H") + @"</div>
</td>
</tr>
<tr>
  <td>الرقم : " + partId + @"</td>
  <td> الاسم : " + partName + @" </td>
  <td> الحساب : " + partAccount + @" </td>
</tr>" +
periodTitle
+ @"
</tbody>
</table>" + resultSb.ToString() +
@"</body><html> "); ;
            return sb.ToString();
        }
    }
}
