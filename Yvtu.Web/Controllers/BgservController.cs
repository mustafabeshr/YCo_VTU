using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
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

        public BgservController(IAppDbContext db, IPartnerManager partnerManager, IToastNotification toastNotification)
        {
            this.db = db;
            this.partnerManager = partnerManager;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var model = new CreateBackgroundServiceDto();
            return View(model);
        }
    }
}
