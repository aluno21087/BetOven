using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BetOven.Models;
using BetOven.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BetOven.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BetOvenDB _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(ILogger<HomeController> logger, BetOvenDB context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> IndexAsync()
        {
            try {
                var user = await _userManager.GetUserAsync(User);
                var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
                ViewBag.Saldo = util.Saldo;

                return View();
            }
            catch { return View(); }  

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
