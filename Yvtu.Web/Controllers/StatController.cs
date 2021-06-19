using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.IO;
using System.Threading.Tasks;
using Yvtu.Core.rpt;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class StatController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IToastNotification toastNotification;

        public StatController(IAppDbContext db, IPartnerManager partnerManager,
            IPartnerActivityRepo partnerActivity, IToastNotification toastNotification)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.partnerActivity = partnerActivity;
            this.toastNotification = toastNotification;
        }

        public IActionResult Collection()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Report.Collection.StatReport", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var channels = new CommonCodeRepo(db).GetCodesByType("access.channel");
            var statuses = new CommonCodeRepo(db).GetCodesByType("Collection.Status");
            var model = new CollectionRptQuery();
            model.Channels = channels;
            model.Statuses = statuses;
            model.Param.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(10));
            model.Param.EndDate = DateTime.Today.AddDays(1);
            return View(model);
        }
        [HttpPost]
        public IActionResult Collection(CollectionRptQuery model)
        {
            if (ModelState.IsValid)
            {
                var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = partnerActivity.GetPartAct("Report.Collection.StatReport", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var result = new CollectionRepo(db).GetStatReport(new CollectionRptQueryParam
                {
                    ChannelId = model.Param.ChannelId,
                    PosId = model.Param.PosId,
                    StatusId = model.Param.StatusId,
                    LevelId = model.Param.LevelId,
                    StartDate = model.Param.StartDate,
                    EndDate = model.Param.EndDate
                });
                model.Results = result;
            }
            var channels = new CommonCodeRepo(db).GetCodesByType("access.channel");
            var statuses = new CommonCodeRepo(db).GetCodesByType("Collection.Status");
            model.Channels = channels;
            model.Statuses = statuses;
            return View(model);
        }
        public IActionResult CollectionToExcel(CollectionRptQuery model)
        {
            if (ModelState.IsValid)
            {
                var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = partnerActivity.GetPartAct("Report.Collection.StatReport.ExportToExcel", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var param = new CollectionRptQueryParam
                {
                    ChannelId = model.Param.ChannelId,
                    PosId = model.Param.PosId,
                    StatusId = model.Param.StatusId,
                    LevelId = model.Param.LevelId,
                    StartDate = model.Param.StartDate,
                    EndDate = model.Param.EndDate
                };
                var result = new CollectionRepo(db).GetStatReport(param);
                model.Results = result;
                if (result != null)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.AddWorksheet("Collections");
                        worksheet.SetRightToLeft();
                        worksheet.ColumnWidth = 15;
                        var currRow = 1;
                        worksheet.Range(1, 1, 1, 7).Merge().Value = param.Title;
                        worksheet.Range(1, 1, 1, 7).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        worksheet.Cell(currRow, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        worksheet.Cell(currRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Row(1).Height = 100;
                        worksheet.Row(1).Style.Font.Bold = true;
                        worksheet.Row(1).Style.Font.FontSize = 16;
                        ++currRow;
                        worksheet.Cell(currRow, 1).Value = "القناة";
                        worksheet.Cell(currRow, 2).Value = model.Param.LevelId == "pos" ? "رقم النقطة" : "اليوم";
                        worksheet.Cell(currRow, 3).Value = model.Param.LevelId == "pos" ? "رقم النقطة" : "";
                        worksheet.Cell(currRow, 4).Value = "الحالة";
                        worksheet.Cell(currRow, 5).Value = "عدد العمليات";
                        worksheet.Cell(currRow, 6).Value = "اجمالي المبلغ";
                        worksheet.Cell(currRow, 7).Value = "عدد النقاط";
                        worksheet.Row(currRow).Style.Font.Bold = true;
                        worksheet.Row(currRow).Style.Font.FontSize = 14;
                        worksheet.Row(currRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Row(currRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        worksheet.Row(currRow).Height = 25;
                        worksheet.Range(currRow, 1, currRow, 7).Style.Fill.BackgroundColor = XLColor.LightPastelPurple;

                        foreach (var item in result)
                        {
                            ++currRow;
                            worksheet.Columns(6, 6).Style.NumberFormat.SetFormat("#,##0.00");
                            worksheet.Columns(5, 5).Style.NumberFormat.SetFormat("#,##0");
                            worksheet.Columns(7, 7).Style.NumberFormat.SetFormat("#,##0");
                            worksheet.Row(currRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Row(currRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            worksheet.Cell(currRow, 1).Value = item.Channel;
                            worksheet.Cell(currRow, 2).Value = model.Param.LevelId == "pos" ? item.Partner.Id : item.CollDay;
                            worksheet.Cell(currRow, 3).Value = model.Param.LevelId == "pos" ? item.Partner.Name : "";
                            worksheet.Cell(currRow, 4).Value = item.Status.Name;
                            worksheet.Cell(currRow, 5).Value = item.Count;
                            worksheet.Cell(currRow, 6).Value = item.Amount;
                            worksheet.Cell(currRow, 7).Value = item.DistinctCount;
                            worksheet.Row(currRow).Height = 20;
                        }
                        ++currRow;
                        worksheet.Row(currRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Row(currRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        worksheet.Row(currRow).Height = 25;
                        worksheet.Row(currRow).Style.Font.Bold = true;
                        worksheet.Row(currRow).Style.Font.FontSize = 12;
                        worksheet.Cell(currRow, "E").SetFormulaA1("=SUM(E3:E" + (currRow - 1) + ")");
                        worksheet.Cell(currRow, "F").SetFormulaA1("=SUM(F3:F" + (currRow - 1) + ")");
                        worksheet.Cell(currRow, "G").SetFormulaA1("=SUM(G3:G" + (currRow - 1) + ")");
                        worksheet.Range(currRow, 1, currRow, 4).Merge().Value = "الاجــمـــالــي";
                        worksheet.Range(currRow, 1, currRow, 7).Style.Fill.BackgroundColor = XLColor.LightPastelPurple;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.BottomBorderColor = XLColor.Black;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.LeftBorderColor = XLColor.Black;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.RightBorderColor = XLColor.Black;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.TopBorderColor = XLColor.Black;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(1, 1, currRow, 7).Style.Border.InsideBorderColor = XLColor.LightGray;
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();
                            return File(
                                    content,
                                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                    "Collection" + DateTime.Today.ToString("yyyyMMdd") + ".xlsx"
                                    );
                        }
                    }
                }
            }
            var channels = new CommonCodeRepo(db).GetCodesByType("access.channel");
            var statuses = new CommonCodeRepo(db).GetCodesByType("Collection.Status");
            model.Channels = channels;
            model.Statuses = statuses;
            return View("Collection", model);
        }

        public IActionResult MoneyTransfer()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("Report.MoneyTransfer.StatReport", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var channels = new CommonCodeRepo(db).GetCodesByType("access.channel");
            var transTypes = new CommonCodeRepo(db).GetCodesByType("TransType");
            var model = new MoneyTransferRptQuery();
            model.Channels = channels;
            model.TransType = transTypes;
            model.Param.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(10));
            model.Param.EndDate = DateTime.Today.AddDays(1);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MoneyTransfer(MoneyTransferRptQuery model)
        {
            if (ModelState.IsValid)
            {
                var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = partnerActivity.GetPartAct("Report.MoneyTransfer.StatReport", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var result = await new MoneyTransferRepo(db, null, null).GetStatReportAsync(new MoneyTransferRptQueryParam
                {
                    ChannelId = model.Param.ChannelId,
                    PosId = model.Param.PosId,
                    TransTypeId = model.Param.TransTypeId,
                    LevelId = model.Param.LevelId,
                    StartDate = model.Param.StartDate,
                    EndDate = model.Param.EndDate
                });
                model.Results = result;
            }
            var channels = new CommonCodeRepo(db).GetCodesByType("access.channel");
            var transTypes = new CommonCodeRepo(db).GetCodesByType("TransType");
            model.Channels = channels;
            model.TransType = transTypes;
            return View(model);
        }

        public async Task<IActionResult> MoneyTransferToExcel(MoneyTransferRptQuery model)
        {
            if (ModelState.IsValid)
            {
                var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = partnerActivity.GetPartAct("Report.MoneyTransfer.StatReport.ExportToExcel", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var param = new MoneyTransferRptQueryParam
                {
                    ChannelId = model.Param.ChannelId,
                    PosId = model.Param.PosId,
                    TransTypeId = model.Param.TransTypeId,
                    LevelId = model.Param.LevelId,
                    StartDate = model.Param.StartDate,
                    EndDate = model.Param.EndDate
                };
                var result = await new MoneyTransferRepo(db, null, null).GetStatReportAsync(param);
                model.Results = result;
                if (result != null)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.AddWorksheet("MoneyTransfer");
                        worksheet.SetRightToLeft();
                        worksheet.ColumnWidth = 15;
                        var currRow = 1;
                        worksheet.Range(1, 1, 1, 5).Merge().Value = param.Title;
                        worksheet.Range(1, 1, 1, 5).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        worksheet.Cell(currRow, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        worksheet.Cell(currRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Row(1).Height = 100;
                        worksheet.Row(1).Style.Font.Bold = true;
                        worksheet.Row(1).Style.Font.FontSize = 16;
                        ++currRow;
                        worksheet.Cell(currRow, 1).Value = "القناة";
                        worksheet.Cell(currRow, 2).Value = model.Param.LevelId == "pos" ? "رقم النقطة" : "اليوم";
                        worksheet.Cell(currRow, 3).Value = model.Param.LevelId == "pos" ? "رقم النقطة" : "";
                        worksheet.Cell(currRow, 4).Value = "عدد العمليات";
                        worksheet.Cell(currRow, 5).Value = "اجمالي المبلغ";
                        worksheet.Row(currRow).Style.Font.Bold = true;
                        worksheet.Row(currRow).Style.Font.FontSize = 14;
                        worksheet.Row(currRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Row(currRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        worksheet.Row(currRow).Height = 25;
                        worksheet.Range(currRow, 1, currRow, 5).Style.Fill.BackgroundColor = XLColor.LightPastelPurple;
                        foreach (var item in result)
                        {
                            ++currRow;
                            worksheet.Columns(5, 5).Style.NumberFormat.SetFormat("#,##0.00");
                            worksheet.Columns(4, 4).Style.NumberFormat.SetFormat("#,##0");
                            worksheet.Row(currRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Row(currRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            worksheet.Cell(currRow, 1).Value = item.Channel;
                            worksheet.Cell(currRow, 2).Value = model.Param.LevelId == "pos" ? item.Partner.Id : item.CollDay;
                            worksheet.Cell(currRow, 3).Value = model.Param.LevelId == "pos" ? item.Partner.Name : "";
                            worksheet.Cell(currRow, 4).Value = item.Count;
                            worksheet.Cell(currRow, 5).Value = item.Amount;
                            worksheet.Row(currRow).Height = 20;
                        }
                        ++currRow;
                        worksheet.Row(currRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Row(currRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        worksheet.Row(currRow).Height = 25;
                        worksheet.Row(currRow).Style.Font.Bold = true;
                        worksheet.Row(currRow).Style.Font.FontSize = 12;
                        worksheet.Cell(currRow, "D").SetFormulaA1("=SUM(D3:D" + (currRow - 1) + ")");
                        worksheet.Cell(currRow, "E").SetFormulaA1("=SUM(E3:E" + (currRow - 1) + ")");
                        worksheet.Range(currRow, 1, currRow, 3).Merge().Value = "الاجــمـــالــي";
                        worksheet.Range(currRow, 1, currRow, 5).Style.Fill.BackgroundColor = XLColor.LightPastelPurple;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.BottomBorderColor = XLColor.Black;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.LeftBorderColor = XLColor.Black;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.RightBorderColor = XLColor.Black;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.TopBorderColor = XLColor.Black;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(1, 1, currRow, 5).Style.Border.InsideBorderColor = XLColor.LightGray;


                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();
                            return File(
                                    content,
                                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                    "MoneyTransfer" + DateTime.Today.ToString("yyyyMMdd") + ".xlsx"
                                    );
                        }
                    }
                }
            }
            var channels = new CommonCodeRepo(db).GetCodesByType("access.channel");
            var transTypes = new CommonCodeRepo(db).GetCodesByType("TransType");
            model.Channels = channels;
            model.TransType = transTypes;
            return View("MoneyTransfer", model);
        }
    }
}
