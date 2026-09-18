using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.ViewModels.HomeViewModels;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService homeService;

        public HomeController(ILogger<HomeController> logger,IHomeService homeService)
        {
            _logger = logger;
            this.homeService = homeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return View();
            }

            string userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            HomeDashboardViewModel vmodel =
                await homeService.GetDashboardAsync(userId);

            return View(vmodel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
        }
    }
}