﻿using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
