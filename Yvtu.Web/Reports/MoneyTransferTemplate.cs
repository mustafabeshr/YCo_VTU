using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Web.Reports
{
    public  class MoneyTransferTemplate
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IWebHostEnvironment environment;
        private readonly IPartnerActivityRepo partnerActivity;

        public MoneyTransferTemplate(IAppDbContext db, IPartnerManager partnerManager, IWebHostEnvironment environment, IPartnerActivityRepo partnerActivity)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.environment = environment;
            this.partnerActivity = partnerActivity;
        }
       
        public  string GetHTMLString(int id)
        {
            var result = new MoneyTransferRepo(db, partnerManager, partnerActivity).GetSingleOrDefault(id);
            var sb = new StringBuilder();

            sb.Append(@"<html>
 <head></head>
<body>
<table class='header_table'>
<tbody>
<tr>
  <td><img src='" + Path.Combine(environment.WebRootPath, "images", "Y-Logo2.png") + @"' alt='YCo Logo' class='brand-image' width='70' height='70' style='opacity: .8'></td>
  <td><h1>بيع رصيد شاحن فوري</h1></td>
  <td>
<div style='display:bloack;'>"+DateTime.Now.ToString("yyyy/MM/dd")+ @"</div>
<div style='display:bloack;'>" + DateTime.Now.ToString("ss:mm:H") + @"</div>
</td>
</tr>
</tbody>
</table>
<table  class='RefData'> 
 <tbody>
    <tr>
     <td style='width:20%;'>رقم المرجع</td>
     <td style='width:30%;font-size:25px;font-weight:bold;'>" + result.Id + @"</td>
     <td style='width:20%;'>التاريخ</td>
     <td style='width:30%;'>" + result.CreatedOn.ToString("yyyy/MM/dd ss:mm:H") + @"</td>
</tr>
</tbody>
<table>

<table  class='AmountDetails' > 
<thead>
<tr style='height:40px;'>
     <th colspan='2' style='border-left:1px solid #cbcdcf;'>من</th>
     <th colspan='2'>الى</th>
   </tr>
</thead>
 <tbody>
    
<tr style='height:40px;'>
     <td style='width:20%;text-align:right;padding-right:10px;'>الرقم :</td>
     <td style='width:30%;border-left:1px solid #cbcdcf;'>" + result.CreatedBy.Id + @"</td>
     <td style='width:20%;text-align:right;padding-right:10px;'>الرقم :</td>
     <td style='width:30%;'>" + result.Partner.Id + @"</td>
   </tr>
<tr style='height:40px;'>
     <td style='width:20%;text-align:right;padding-right:10px;'>الحساب :</td>
     <td style='width:30%;border-left:1px solid #cbcdcf;'>" + result.CreatedBy.Account + @"</td>
     <td style='width:20%;text-align:right;padding-right:10px;'>الحساب :</td>
     <td style='width:30%;'>" + result.Partner.Account + @"</td>
   </tr>
<tr style='height:40px;'>
     <td style='width:20%;text-align:right;padding-right:10px;'>الاسم :</td>
     <td style='width:30%;border-left:1px solid #cbcdcf;'>" + result.CreatedBy.Name + @"</td>
     <td style='width:20%;text-align:right;padding-right:10px;'>الاسم :</td>
     <td style='width:30%;'>" + result.Partner.Name + @"</td>
   </tr>
<tr style='height:40px;'>
     <td style='width:20%;text-align:right;padding-right:10px;'>النوع :</td>
     <td style='width:30%;border-left:1px solid #cbcdcf;'>" + result.CreatedBy.Role.Name + @"</td>
     <td style='width:20%;text-align:right;padding-right:10px;'>النوع :</td>
     <td style='width:30%;'>" + result.Partner.Role.Name + @"</td>
   </tr>
<tr style='height:40px;'>
     <td style='width:20%;text-align:right;padding-right:10px;'>الرصيد :</td>
     <td style='width:30%;border-left:1px solid #cbcdcf;'>" + result.CreatedBy.Balance.ToString("N2") + @"</td>
     <td style='width:20%;text-align:right;padding-right:10px;'>الرصيد :</td>
     <td style='width:30%;'>" + result.Partner.Balance.ToString("N2") + @"</td>
   </tr>
</tbody>
<table>

<table class='AmountDetails'> 
<thead>
<th colspan='4'>التفــــــاصيــــــل</th>
</thead>
 <tbody>
    <tr>
     <td style='width:15%;text-align:right;'>المبلغ</td>
     <td style='width:30%;text-align:center;'>" + result.Amount.ToString("N2") + @"</td>
     <td style='width:55%;text-align:right;font-size:18px;'>" + new MonyToString().NumToStr(result.Amount) + @"</td>
</tr>
<tr>
     <td style='width:15%;text-align:right;'>الصافي</td>
     <td style='width:30%;text-align:center;'>" + result.NetAmount.ToString("N2") + @"</td>
     <td style='width:55%;text-align:right;font-size:18px;'>" + new MonyToString().NumToStr(result.NetAmount) + @"</td>
</tr>
<tr>
     <td style='width:15%;text-align:right;'>نسبة الضريبة</td>
     <td style='width:30%;text-align:center;'>" + result.TaxPercent.ToString("N2") + @" %</td>
     <td style='width:55%;text-align:right;'></td>
</tr>
<tr>
<td style='width:15%;text-align:right;'>مبلغ الضريبة</td>
     <td style='width:30%;text-align:center;'>" + result.TaxAmount.ToString("N2") + @"</td>
     <td style='width:55%;text-align:right;font-size:18px;'>" + new MonyToString().NumToStr(result.TaxAmount) + @"</td>
</tr>
<tr>
<td style='width:15%;text-align:right;'>نسبة العمولة</td>
     <td style='width:30%;text-align:center;'>" + result.BonusPercent.ToString("N2") + @" %</td>
     <td style='width:55%;text-align:right;'></td>
</tr>
<tr>
<td style='width:15%;text-align:right;'>مبلغ العمولة</td>
     <td style='width:30%;text-align:center;'>" + result.BounsAmount.ToString("N2") + @"</td>
     <td style='width:55%;text-align:right;font-size:18px;'>" + new MonyToString().NumToStr(result.BounsAmount) + @"</td>
</tr>
<tr>
<td style='width:15%;text-align:right;'>نسبة ضريبة العمولة</td>
     <td style='width:30%;text-align:center;'>" + result.BounsTaxPercent.ToString("N2") + @" %</td>
     <td style='width:55%;text-align:right;'></td>
</tr>
<tr>
<td style='width:10%;text-align:right;'>مبلغ ضريبة العمولة</td>
     <td style='width:30%;text-align:center;'>" + result.BounsTaxAmount.ToString("N2") + @"</td>
     <td style='width:60%;text-align:right;font-size:18px;'>" + new MonyToString().NumToStr(result.BounsTaxAmount) + @"</td>
</tr>
<tr>
<td style='width:15%;text-align:right;'>المبلغ المطلوب</td>
     <td style='width:30%;text-align:center;'>" + result.ReceivedAmount.ToString("N2") + @"</td>
     <td style='width:55%;text-align:right;font-size:18px;'>" + new MonyToString().NumToStr(result.ReceivedAmount) + @"</td>
</tr>
</tbody>
<table>

<table style='margin-bottom:30px;'> 
 <tbody>
    <tr style='height:50px;'>
     <td style='font-size:26px;font-weight:bold;background-color: #EDF0F2;'>" + new MonyToString().NumToStr(result.ReceivedAmount) + @"</td>
    </tr>
</tbody>
</table>
<table style='margin-bottom:30px;'> 
 <tbody>
    <tr style='height:50px;'>
     <td style='width:33%;'>المستلم</td>
     <td style='width:33%;'></td>
     <td style='width:33%;'>المحاسب </td>
    </tr>
 <tr style='height:50px;'>
     <td style='width:33%;font-weight:bold;'>"+result.Partner.Name+@"</td>
     <td style='width:33%;'></td>
     <td style='width:33%;'></td>
    </tr>
</tbody>
</table>
</body><html> ");
            return sb.ToString();
        }
    }
}
