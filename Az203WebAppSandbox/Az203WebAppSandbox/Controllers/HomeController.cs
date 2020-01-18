using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Az203WebAppSandbox.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Az203WebAppSandbox.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<HomeController> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "EasyAuth")]
        public IActionResult Secret()
        {
            ViewData["SpecialAgent"] = _configuration.GetValue<string>("SpecialAgent", "Unknown");
            ViewData["UserId"] = _httpContextAccessor.HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"];

            ViewData["Identity"] = User.Identity.Name;

            ViewData["AuthType"] = User.Identity.AuthenticationType;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
