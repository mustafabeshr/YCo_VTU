using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.IO;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class BgservController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IToastNotification toastNotification;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IWebHostEnvironment environment;

        public BgservController(IAppDbContext db, IPartnerManager partnerManager,
            IToastNotification toastNotification, IPartnerActivityRepo partnerActivity,
             IWebHostEnvironment environment)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.toastNotification = toastNotification;
            this.partnerActivity = partnerActivity;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("BgService.Query", currentRoleId);
            if (permission == null || permission.Details == null || permission.Details.Count == 0)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية", new ToastrOptions { Title = "" });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new AppBackgroundServiceQueryDto();
            var sources = new CommonCodeRepo(db).GetCodesByType("bg_service_source");
            var statuses = new CommonCodeRepo(db).GetCodesByType("bg_service_status");
            model.Sources = sources;
            model.Statuses = statuses;
            model.StartDate = DateTime.Today.AddMonths(-1);
            model.EndDate = DateTime.Today;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(AppBackgroundServiceQueryDto model)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var currentId = partnerManager.GetCurrentUserId(this.HttpContext);
            var currentAccount = partnerManager.GetCurrentUserAccount(this.HttpContext);
            var permission = partnerActivity.GetPartAct("BgService.Query", currentRoleId);
            if (permission == null || permission.Details == null || permission.Details.Count == 0)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحيات الكافية", new ToastrOptions { Title = "" });
                return Redirect(Request.Headers["Referer"].ToString());
            }


            var results = await new AppBackgroundServiceRepo(db).GetBackgroundServicesAsync(new AppBackgroundServiceRepo.BackgroundServiceListParam
            {
                Id = model.Id,
                PartnerId = model.PartnerId,
                CreatedById = model.CreatedById,
                IncludeDates = model.IncludeDates,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Source = model.Source,
                Status = model.Status,
                Name = model.Name
            }, permission, currentId, currentAccount);
            model.Results = results;
            var sources = new CommonCodeRepo(db).GetCodesByType("bg_service_source");
            var statuses = new CommonCodeRepo(db).GetCodesByType("bg_service_status");
            model.Sources = sources;
            model.Statuses = statuses;
            return View(model);
        }

        public IActionResult Create()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("BgService.Create", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new CreateBackgroundServiceDto();
            model.StartDate = DateTime.Now.AddDays(-30);
            model.EndDate = DateTime.Now;
            var sources = new CommonCodeRepo(db).GetCodesByType("bg_service_source");
            model.Sources = sources;
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(CreateBackgroundServiceDto model)
        {
            if (model.Id > 0)
            {
                toastNotification.AddErrorToastMessage("لم يتم حفظ الطلب ، لقد تم حفظه مسبقا", new ToastrOptions
                {
                    Title = ""
                });
            }
            else if (ModelState.IsValid)
            {
                var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
                var currentId = partnerManager.GetCurrentUserId(this.HttpContext);
                var permission = partnerActivity.GetPartAct("BgService.Create", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions { Title = "" });
                    //return Redirect(Request.Headers["Referer"].ToString());
                } else if (permission.Details == null || permission.Details.Count == 0)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions { Title = "" });
                    //return Redirect(Request.Headers["Referer"].ToString());
                }
                else if (model.StartDate != null && model.EndDate != null && model.StartDate > model.EndDate)
                {
                    ModelState.AddModelError("EndDate", "تاريخ النهاية اقل من تاريخ البداية");
                }
                else if (string.IsNullOrEmpty(model.PartnerId) || model.PartnerAccount <= 0)
                {
                    toastNotification.AddErrorToastMessage("يجب تحديد الجهة المطلوب بياناتها", new ToastrOptions { Title = "" });
                    //return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    var targetPartner = partnerManager.GetPartnerByAccount(model.PartnerAccount);
                    if (targetPartner == null)
                    {
                        toastNotification.AddErrorToastMessage("الجهة المراد بياناتها غير موجودة", new ToastrOptions { Title = "" });
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                    if (!permission.Details.Exists(m => m.ToRole.Id == targetPartner.Role.Id))
                    {
                        toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية لطلب بيانات لهذه الجهة", new ToastrOptions { Title = "" });
                        return Redirect(Request.Headers["Referer"].ToString());
                    }

                    if (permission.Scope.Id == "CurOpOnly" && targetPartner.Id != currentId)
                    {
                        toastNotification.AddErrorToastMessage("صلاحيتك محدودة لطلب بيانات لرقمك فقط", new ToastrOptions { Title = "" });
                        return Redirect(Request.Headers["Referer"].ToString());
                    }

                    if (permission.Scope.Id == "Exclusive" && targetPartner.RefPartner.Id != currentId)
                    {
                        toastNotification.AddErrorToastMessage("صلاحيتك محدودة لطلب بيانات للجهات التابعة لك فقط", new ToastrOptions { Title = "" });
                        return Redirect(Request.Headers["Referer"].ToString());
                    }

                    var createdObj = new AppBackgroundService();
                    createdObj.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                    createdObj.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                    createdObj.Source.Id = model.Source;
                    createdObj.Name = model.Name;
                    createdObj.Partner.Id = model.PartnerId;
                    createdObj.Partner.Account = model.PartnerAccount;
                    createdObj.StartDate = model.StartDate ?? DateTime.MinValue;
                    createdObj.EndDate = model.EndDate ?? DateTime.MinValue;
                    createdObj.Note = model.Note;
                    createdObj.ActionPartner.Id = model.ActionPartnerId;
                    createdObj.ActionPartner.Account = model.ActionPartnerAccount;
                    createdObj.ActiveTime = model.ActiveTime ?? DateTime.MinValue;
                    var result = new AppBackgroundServiceRepo(db).Create(createdObj);
                    if (result.Success)
                    {
                        ModelState.Clear();
                        model.Id = result.AffectedCount;
                        toastNotification.AddSuccessToastMessage("تم حفظ العملية بنجاح، لمعرفة حالتها يرجى الاستعلام من شاشة استعلام الطلبات", new ToastrOptions
                        {
                            Title = ""
                        });
                    }
                    else
                    {
                        toastNotification.AddErrorToastMessage("لم يتم حفظ الطلب " + result.AffectedCount, new ToastrOptions
                        {
                            Title = ""
                        });
                    }
                }
            }
            var sources = new CommonCodeRepo(db).GetCodesByType("bg_service_source");
            model.Sources = sources;
            return View(model);
        }

        public async Task<IActionResult> download(int id)
        {
            if (id <= 0)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = await new AppBackgroundServiceRepo(db).GetBackgroundServiceAsync(id);
            if (model == null)
            {
                toastNotification.AddErrorToastMessage("البيانات غير متوفرة", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var file = Path.Combine(environment.WebRootPath, model.FileLocation, model.FileName);
            if (System.IO.File.Exists(file))
            {
                var net = new System.Net.WebClient();
                var data = net.DownloadData(file);
                var content = new System.IO.MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";
                var fileName = Path.GetFileName(file);
                return File(content, contentType, fileName);
            }
            else
            {
                toastNotification.AddErrorToastMessage("الملف لم يعد موجودا", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
    }
}
