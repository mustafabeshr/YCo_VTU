using System.Collections.Generic;
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
    [Authorize(Roles ="Admin")]
    public class ActivityController : Controller
    {
        private readonly IAppDbContext db;
        private readonly IPartnerManager partnerManager;
        private readonly IToastNotification toastNotification;

        public ActivityController(IAppDbContext db, IPartnerManager partnerManager, IToastNotification toastNotification)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index(string id)
        {
            if (!string.IsNullOrEmpty(id)){
                var activity = new ActivityRepo(db, partnerManager).GetActivity(id, true);
                if (activity != null)
                {
                    var model = new ActivityQuery();
                    model.Results.Add(activity);
                    return View(model);
                }
                else
                {
                    var model = new ActivityQuery();
                    return View(model);
                }
            } else
            {
                var activities = new ActivityRepo(db, partnerManager).GetActivities(id, true);
                var model = new ActivityQuery();
                model.Results = activities;
                return View(model);
            }
        }
        [HttpPost]
        public IActionResult Index(ActivityQuery model)
        {
            var activities = new ActivityRepo(db, partnerManager).GetActivities(model.QName, true);
            model.Results = activities;
            return View(model);
        }

        public IActionResult AssignMessage(string id)
        {
            var activity = new ActivityRepo(db, partnerManager).GetActivity(id);
            if (activity == null)
            {
                toastNotification.AddErrorToastMessage("البيانات غير موجودة ");
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var model = new AssignActivityMessageDto();
            model.ActivityId = activity.Id;
            model.Activity = activity;
            var sendingTime = new CommonCodeRepo(db).GetCodesByType("ActivityMessage.SendingTime");
            model.SendingTime = sendingTime;
            var messages = new MessageTemplateRepo(db, partnerManager).GetAll(activity.Id);
            var listOfSelectedMessage = new List<SelectedMessages>();
            if (messages != null)
            {
                foreach (var item in messages)
                {
                    listOfSelectedMessage.Add(new SelectedMessages
                    {
                        Message = item,
                        SendingTime = sendingTime
                    }); ;
                }
            }
            model.Messages = listOfSelectedMessage;
            model.ActivityMessages = new ActivityMessageRepo(db, partnerManager).GetList(id, -1);
            return View(model);
        }
        [HttpPost]
        public IActionResult AssignMessage(int msgid, string st, AssignActivityMessageDto model)
        {
            var actMsg = new ActivityMessage();
            actMsg.Activity.Id = model.ActivityId;
            actMsg.Message.Id = msgid;
            actMsg.SendingTime.Id = "after";
            actMsg.CreatedBy.Id = partnerManager.GetCurrentUserId(this.HttpContext);
            actMsg.CreatedBy.Account = partnerManager.GetCurrentUserAccount(this.HttpContext);
            actMsg.MessageOrder = -1;
            var result = new ActivityMessageRepo(db, partnerManager).Create(actMsg);
            var activity = new ActivityRepo(db, partnerManager).GetActivity(model.ActivityId);
            model.Activity = activity;
            var sendingTime = new CommonCodeRepo(db).GetCodesByType("ActivityMessage.SendingTime");
            model.SendingTime = sendingTime;
            var messages = new MessageTemplateRepo(db, partnerManager).GetAll(activity.Id);
            var listOfSelectedMessage = new List<SelectedMessages>();
            if (messages != null)
            {

                foreach (var item in messages)
                {
                    listOfSelectedMessage.Add(new SelectedMessages
                    {
                        Message = item,
                        SendingTime = sendingTime
                    }); ;
                }
            }
            model.Messages = listOfSelectedMessage;
            model.ActivityMessages = new ActivityMessageRepo(db, partnerManager).GetList(activity.Id, -1);
            return View(model);
        }

        public IActionResult DeleteMessage(string actId, int msgId)
        {
            var old = new ActivityMessageRepo(db, partnerManager).GetSingle(actId, msgId);
            if (old != null)
            {
                var audit = new DataAudit();
                audit.Activity.Id = "ActivityMessage.Delete";
                audit.PartnerId = partnerManager.GetCurrentUserId(this.HttpContext);
                audit.PartnerAccount = partnerManager.GetCurrentUserAccount(this.HttpContext);
                audit.Action.Id = "Delete";
                audit.Success = true;
                audit.OldValue = old.ToString();
                new DataAuditRepo(db).Create(audit);
                new ActivityMessageRepo(db, partnerManager).RemoveMessage(actId, msgId);

                return LocalRedirect("~/Activity/AssignMessage/" + actId);
            }
            return RedirectToAction("Index");
        }

        public IActionResult OrderUp (string actId, int msgId)
        {
            var currentMessage = new ActivityMessageRepo(db, partnerManager).GetSingle(actId, msgId);

            if (currentMessage.MessageOrder <= 1 ) return Redirect("~/Activity/AssignMessage/" + actId);

            var actList = new ActivityMessageRepo(db, partnerManager).GetList(actId, -1);
            foreach (var item in actList)
            {
                if (item.MessageOrder == currentMessage.MessageOrder - 1)
                {
                    new ActivityMessageRepo(db, partnerManager).UpdateOrder(actId, item.Message.Id, currentMessage.MessageOrder);
                    new ActivityMessageRepo(db, partnerManager).UpdateOrder(actId, currentMessage.Message.Id, currentMessage.MessageOrder - 1);
                }
            }
            return LocalRedirect("~/Activity/AssignMessage/" + actId);
        }

        public IActionResult OrderDown(string actId, int msgId)
        {
            var currentMessage = new ActivityMessageRepo(db, partnerManager).GetSingle(actId, msgId);
            var actList = new ActivityMessageRepo(db, partnerManager).GetList(actId, -1);
            
            if (currentMessage.MessageOrder >= actList.Count) return Redirect("~/Activity/AssignMessage/" + actId);
            foreach (var item in actList)
            {
                if (item.MessageOrder - 1 == currentMessage.MessageOrder)
                {
                    new ActivityMessageRepo(db, partnerManager).UpdateOrder(actId, item.Message.Id, currentMessage.MessageOrder);
                    new ActivityMessageRepo(db, partnerManager).UpdateOrder(actId, currentMessage.Message.Id, currentMessage.MessageOrder + 1);
                }
            }
            return LocalRedirect("~/Activity/AssignMessage/" + actId);
        }
    }
}
