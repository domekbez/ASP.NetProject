using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CVEditor.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class TestController : Controller
    {
        /// <summary>
        /// Returns list of akk Job Offers
        /// </summary>
        /// <returns>View of list of all Job offers</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}