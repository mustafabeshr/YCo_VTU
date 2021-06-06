using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;
using Yvtu.Web.Models;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppDbContext db;
        private readonly IPartnerActivityRepo _partnerActivityRepo;

        public HomeController(ILogger<HomeController> logger, IAppDbContext db, IPartnerActivityRepo partnerActivityRepo)
        {
            _logger = logger;
            this.db = db;
            _partnerActivityRepo = partnerActivityRepo;
        }

        public IActionResult Index()
        {
            var roleId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.GivenName)?.Value);
            var model = _partnerActivityRepo.GetListByFrom(roleId);
            if (model != null)
            {
                return View(model);
            }

            return null;
        }

        public IActionResult globalparam()
        {
            var model = new AppGlobalSettingsRepo(db).GetAll();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
