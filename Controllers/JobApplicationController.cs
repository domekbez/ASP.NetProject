using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVEditor.EntityFramework;
using CVEditor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static CVEditor.Controllers.HomeController;
using static CVEditor.Models.JobApplication;

namespace CVEditor.Controllers
{
    public class JobApplicationController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public JobApplicationController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        public IActionResult Index()
        {
            return BadRequest("Log in first.");
        }

        [Authorize]
        [UserType(UserType.User)]
        [ActionName("Index")]
        public IActionResult IndexUser()
        {
            User user = _context.Users.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);

            List<JobApplication> searchResult = _context.JobApplications.ToList().FindAll(x => x.UserId == user.Id);

            return View("~/Views/JobApplication/IndexUser.cshtml", searchResult);
        }

        [Authorize]
        [UserType(UserType.Admin)]
        [ActionName("Index")]
        public IActionResult IndexAdmin()
        {
            List<JobApplication> searchResult = _context.JobApplications.ToList();

            return View("~/Views/JobApplication/IndexAdmin.cshtml", searchResult);
        }

        [Authorize]
        [UserType(UserType.User)]
        public IActionResult Details(int id)
        {
            JobApplication jobApplication = _context.JobApplications.ToList().Find(x => x.Id == id);
            JobOffer jobOffer = _context.JobOffers.ToList().Find(x => x.Id == jobApplication.OfferId);


            return View(jobOffer);
        }

        [Authorize]
        [UserType(UserType.User)]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest($"id should not be null");
            }
            JobApplication jobApplication = _context.JobApplications.FirstOrDefault(u => u.Id == id.Value);
            User user = _context.Users.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);

            if (jobApplication.UserId != user.Id) return BadRequest("That is not your offer. GET OUT!");
            var isEmpty = _context.JobApplications.Any((x) => x.Id == id.Value);
            if (isEmpty == false)
            {
                return NotFound($"id doesn't exist");

            }
            _context.JobApplications.Remove(jobApplication);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [UserType(UserType.HR)]
        [ActionName("Index")]
        public IActionResult IndexHR()
        {
            HR hr = _context.HRs.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);
            List<JobOffer> jobOffers = _context.JobOffers.ToList().FindAll(x => x.HRId == hr.Id);


            List<JobApplication> searchResult = _context.JobApplications.ToList().FindAll(x => jobOffers.Any(y => y.Id == x.OfferId));
            return View("~/Views/JobApplication/IndexHR.cshtml", searchResult);
        }

        [Authorize]
        [UserType(UserType.HR)]
        public IActionResult AddComment(int id)
        {
            JobApplication jobApplication = _context.JobApplications.ToList().Find(x => x.Id == id);

            return View("~/Views/JobApplication/EditComment.cshtml", jobApplication);
        }

        [Authorize]
        [UserType(UserType.HR)]
        public IActionResult EditComment(JobApplication model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var offer = _context.JobApplications.FirstOrDefault(x => x.Id == model.Id);
            offer.Comment = model.Comment;
            _context.Update(offer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [UserType(UserType.HR)]
        public IActionResult Accept(int id)
        {
            JobApplication application = _context.JobApplications.FirstOrDefault(x => x.Id == id);
            application.ApplicationStatus = status.accepted;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [UserType(UserType.HR)]
        public IActionResult Reject(int id)
        {
            JobApplication application = _context.JobApplications.FirstOrDefault(x => x.Id == id);
            application.ApplicationStatus = status.rejected;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
    }