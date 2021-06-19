using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.Threading.Tasks;
using Yvtu.Core.Entities;
using Yvtu.Core.Queries;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Dto;

namespace Yvtu.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class msgController : Controller
    {

        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IToastNotification toastNotification;
        private readonly IPartnerActivityRepo partnerActivity;
        private readonly IDataAuditRepo auditing;

        public msgController(IAppDbContext db, IPartnerManager partnerManager, IToastNotification toastNotification,
            IPartnerActivityRepo partnerActivity, IDataAuditRepo auditing)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.toastNotification = toastNotification;
            this.partnerActivity = partnerActivity;
            this.auditing = auditing;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(MessageTemplateQuery model)
        {
            model.Results = new MessageTemplateRepo(db, partnerManager).GetByPartTitle(model.QMessage, true);
            return View(model);
        }
        public IActionResult Create()
        {
            var model = new CreateMessageTemplateDto();
            model.Dictionary = new MessageTemplateRepo(db, partnerManager).GetDictionaryAll();
            model.Message = string.Empty;
            model.ToWho = 1;
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var message = new MessageTemplateRepo(db, partnerManager).GetSingle(id);
            if (message == null)
            {
                toastNotification.AddErrorToastMessage("البيانات غير موجودة ", new ToastrOptions { Title = "" });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreateMessageTemplateDto();
            model.Dictionary = new MessageTemplateRepo(db, partnerManager).GetDictionaryAll();
            model.Id = message.Id;
            model.Title = message.Title;
            model.ToWho = message.ToWho;
            model.Message = message.Message;
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(CreateMessageTemplateDto model)
        {
            if (ModelState.IsValid)
            {
                var message = new MessageTemplateRepo(db, partnerManager).GetSingle(model.Id);
                if (message == null)
                {
                    toastNotification.AddErrorToastMessage("البيانات غير موجودة ", new ToastrOptions { Title = "" });
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var param = new MessageTemplate();
                param.Id = model.Id;
                param.Title = model.Title;
                param.ToWho = model.ToWho;
                param.Message = model.Message;
                param.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                param.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                var result = new MessageTemplateRepo(db, partnerManager).Update(param);
                if (result.Success)
                {
                    toastNotification.AddSuccessToastMessage("تم تعديل البيانات بنجاح ", new ToastrOptions { Title = "" });
                }
                else
                {
                    toastNotification.AddErrorToastMessage("لم يتم تعديل البيانات بنجاح ", new ToastrOptions { Title = "" });
                }
            }
            model.Dictionary = new MessageTemplateRepo(db, partnerManager).GetDictionaryAll();
            return View(model);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("MessageTemplate.Delete", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions { Title = "" });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var old = new MessageTemplateRepo(db, partnerManager).GetSingle(id);
            if (old != null)
            {
                var audit = new DataAudit();
                audit.Activity.Id = "MessageTemplate.Delete";
                audit.PartnerId = partnerManager.GetCurrentUserId(this.HttpContext);
                audit.PartnerAccount = partnerManager.GetCurrentUserAccount(this.HttpContext);
                audit.Action.Id = "Delete";
                audit.Success = true;
                audit.OldValue = old.ToString();
                auditing.Create(audit);
                new MessageTemplateRepo(db, partnerManager).RemoveMessage(id);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Create(CreateMessageTemplateDto model)
        {
            if (ModelState.IsValid)
            {
                var oldMessage = new MessageTemplateRepo(db, partnerManager).GetByExactTitle(model.Title);
                if (oldMessage == null)
                {
                    var insertedObj = new MessageTemplate();
                    insertedObj.Title = model.Title;
                    insertedObj.Message = model.Message;
                    insertedObj.ToWho = model.ToWho;
                    insertedObj.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                    insertedObj.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);

                    var result = new MessageTemplateRepo(db, partnerManager).Create(insertedObj);
                    if (result.AffectedCount > 0)
                    {
                        toastNotification.AddSuccessToastMessage("تم الحفظ بنجاح رقم " + result.AffectedCount, new ToastrOptions { Title = "" });
                        model.Id = result.AffectedCount;
                    }
                    else
                    {
                        toastNotification.AddWarningToastMessage("لم تنجح عملية الحفظ ", new ToastrOptions { Title = "" });
                    }
                }
                else
                {
                    toastNotification.AddErrorToastMessage("هذه الرسالة موجودة مسبقا ", new ToastrOptions { Title = "" });
                }
            }
            model.Dictionary = new MessageTemplateRepo(db, partnerManager).GetDictionaryAll();
            model.Message = string.Empty;
            return View(model);
        }
        public IActionResult SendSMSOne()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("SMS.SendOne", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new SMSOneDto();
            return View(model);
        }
        [HttpPost]
        public IActionResult SendSMSOne(SMSOneDto model)
        {
            if (ModelState.IsValid)
            {

                var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
                var permission = partnerActivity.GetPartAct("SMS.SendOne", currentRoleId);
                if (permission == null)
                {
                    toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                    {
                        Title = ""
                    });
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var inserted = new SMSOne();
                inserted.Message = model.Message;
                inserted.Note = model.Note;
                inserted.Receiver = model.Receiver;
                inserted.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                inserted.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
                var result = new SMSOneRepo(db, partnerManager).Create(inserted);
                if (result.Success)
                {
                    ModelState.Clear();
                    model.Receiver = string.Empty;
                    toastNotification.AddSuccessToastMessage("تم حفظ الرسالة بنجاح وسيتم ارساله فورا ", new ToastrOptions { Title = "" });
                }
                else
                {
                    toastNotification.AddInfoToastMessage("فشل عملية حفظ الرسالة ", new ToastrOptions { Title = "" });
                }
            }
            return View(model);
        }

        public IActionResult SendSMSOneQuery()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("SMS.SendOne.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new SMSOneQueryDto();
            model.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(10));
            model.EndDate = DateTime.Today.AddDays(1);
            return View(model);
        }
        [HttpPost]
        public IActionResult SendSMSOneQuery(SMSOneQueryDto model)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("SMS.SendOne.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var results = new SMSOneRepo(db, partnerManager).GetList(new SMSOneRepo.GetListParam
            {
                Receiver = model.Receiver,
                Message = model.Message,
                CreatorId = model.CreatedById,
                CreatorAccount = model.CreatedByAccount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IncludeDates = model.IncludeDates
            });
            model.Results = results;
            return View(model);
        }

        public IActionResult SMSInQuery()
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("SMSIn.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var model = new SMSInQueryDto();
            model.StartDate = DateTime.Today.Subtract(TimeSpan.FromDays(2));
            model.EndDate = DateTime.Today.AddDays(1);
            return View("insms", model);
        }

        [HttpPost]
        public async Task<IActionResult> SMSInQuery(SMSInQueryDto model)
        {
            var currentRoleId = partnerManager.GetCurrentUserRole(this.HttpContext);
            var permission = partnerActivity.GetPartAct("SMSIn.Query", currentRoleId);
            if (permission == null)
            {
                toastNotification.AddErrorToastMessage("ليس لديك الصلاحية الكافية", new ToastrOptions
                {
                    Title = ""
                });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var results = await new SMSInRepo(db).GetSMSMessagesAsync(model.Sender, model.Message, model.IncludeDates, model.StartDate, model.EndDate);
            model.Results = results;
            return View("insms", model);
        }
    }

}
