using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
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

        public msgController(IAppDbContext db, IPartnerManager partnerManager,  IToastNotification toastNotification)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(MessageTemplateQuery model)
        {
            model.Results = new MessageTemplateRepo(db, partnerManager).GetByPartTitle(model.QMessage);
            return View(model);
        }
        public IActionResult Create()
        {
            var model = new CreateMessageTemplateDto();
            model.Dictionary = new MessageTemplateRepo(db, partnerManager).GetDictionaryAll();
            model.Message = string.Empty;
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var message = new MessageTemplateRepo(db, partnerManager).GetSingle(id);
            if (message == null)
            {
                toastNotification.AddErrorToastMessage("البيانات غير موجودة ");
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new CreateMessageTemplateDto();
            model.Dictionary = new MessageTemplateRepo(db, partnerManager).GetDictionaryAll();
            model.Id = message.Id;
            model.Title = message.Title;
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
                    toastNotification.AddErrorToastMessage("البيانات غير موجودة ");
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var param = new MessageTemplate();
                param.Id = model.Id;
                param.Title = model.Title;
                param.Message = model.Message;
                param.CreatedBy.Id =  partnerManager.GetCurrentUserId(this.HttpContext);
                param.CreatedBy.Account  = partnerManager.GetCurrentUserAccount(this.HttpContext);
                var result = new MessageTemplateRepo(db, partnerManager).Update(param);
                if (result.Success)
                {
                    toastNotification.AddSuccessToastMessage("تم تعديل البيانات بنجاح ");
                }else
                {
                    toastNotification.AddErrorToastMessage("لم يتم تعديل البيانات بنجاح ");
                }
            }
            model.Dictionary = new MessageTemplateRepo(db, partnerManager).GetDictionaryAll();
            return View(model);
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
                    insertedObj.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
                    insertedObj.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);

                    var result = new MessageTemplateRepo(db, partnerManager).Create(insertedObj);
                    if (result.AffectedCount > 0)
                    {
                        toastNotification.AddSuccessToastMessage("تم الحفظ بنجاح رقم " + result.AffectedCount);
                         model.Id = result.AffectedCount;
                    } else
                    {
                        toastNotification.AddWarningToastMessage("لم تنجح عملية الحفظ ");
                    }
                }
                else
                {
                    toastNotification.AddErrorToastMessage("هذه الرسالة موجودة مسبقا ");
                }
            }
            model.Dictionary = new MessageTemplateRepo(db, partnerManager).GetDictionaryAll();
            model.Message = string.Empty;
            return View(model);
        }
    }
}
