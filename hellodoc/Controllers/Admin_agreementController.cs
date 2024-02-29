
using Microsoft.AspNetCore.Mvc;
using hellodoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace hellodoc.Controllers
{
    public class Admin_agreementController : Controller
    {
        private readonly ILogger<Admin_agreementController> _logger;


        public Admin_agreementController(ILogger<Admin_agreementController> logger)
        {
            _logger = logger;
        }

        public IActionResult agreement()
        {
            return View();
        }
    }
}
