using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVEditor.EntityFramework;
using CVEditor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static CVEditor.Controllers.HomeController;

namespace CVEditor.Controllers
{
    public class HRController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public HRController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        public IActionResult Index()
        {

            return View("~/Views/HR/Index.cshtml", _context.HRs);
        }

        [Authorize]
        [UserType(UserType.Admin)]
        [ActionName("Index")]
        public IActionResult IndexAdmin()
        {
            return View("~/Views/HR/IndexAdmin.cshtml", _context.HRs);
        }


        [Authorize]
        [UserType(UserType.Admin)]
        public IActionResult Edit(int id)
        {
            HR hr = _context.HRs.ToList().Find(x => x.Id == id);
            return View(hr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [UserType(UserType.Admin)]
        public IActionResult Edit(HR model)
        {
            var hr = _context.HRs.FirstOrDefault(x => x.Id == model.Id);
            hr.EmailAddress = model.EmailAddress;
            hr.LastName = model.LastName;
            hr.PhoneNumber = model.PhoneNumber;
            _context.Update(hr);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [UserType(UserType.Admin)]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest($"id should not be null");
            }
            HR hr = _context.HRs.FirstOrDefault(u => u.Id == id.Value);
            if (hr == null)
            {
                return NotFound($"id doesn't exist");

            }
            _context.HRs.Remove(hr);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [UserType(UserType.Admin)]
        public IActionResult AddHR()
        {
            return View("~/Views/HR/AddHR.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [UserType(UserType.Admin)]
        public IActionResult AddHR(HR model)
        {
            HR hr = new HR
            {
                NameId = model.NameId,
                Name = model.Name,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                EmailAddress = model.EmailAddress,
                CompanyId = model.CompanyId
                
            };

            _context.HRs.Add(hr);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}