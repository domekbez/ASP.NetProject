using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CVEditor.Models;
using CVEditor.EntityFramework;
using Microsoft.EntityFrameworkCore;
using static CVEditor.Controllers.HomeController;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using static CVEditor.Models.JobApplication;

namespace CVEditor.Controllers
{

    public class JobOfferController : Controller
    {

        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public JobOfferController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }
             

        [Authorize]
        [HttpGet]
        [UserType(UserType.User)]
        [ActionName("Index")]
        public IActionResult IndexUser()
        {
            List<JobOffer> searchResult = _context.JobOffers.ToList();

            return View("~/Views/JobOffer/IndexUser.cshtml", searchResult);
        }

        [Authorize]
        [HttpGet]
        [UserType(UserType.HR)]
        [ActionName("Index")]
        public IActionResult IndexHR()
        {
            HR hr = _context.HRs.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);
            int id = hr.CompanyId;
            List<JobOffer> searchResult = _context.JobOffers.ToList().FindAll(x => x.CompanyId == id);

            return View("~/Views/JobOffer/IndexHR.cshtml", searchResult);
        }

        [Authorize]
        [HttpGet]
        [UserType(UserType.Admin)]
        [ActionName("Index")]
        public  ActionResult IndexAdmin()
        {
            List<JobOffer> searchResult = _context.JobOffers.ToList();

            return View("~/Views/JobOffer/IndexAdmin.cshtml", searchResult);
        }

        /// <summary>
        /// Returns list of akk Job Offers
        /// </summary>
        /// <returns>View of list of all Job offers</returns>
        /// <seealso cref="CVEditor.Models.JobOffer"/>
        [AllowAnonymous]
        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            List<JobOffer> searchResult = await _context.JobOffers.ToListAsync();

            return View(searchResult);
        }


        /// <summary>
        /// Performs search for offers with given title.
        /// </summary>
        /// <param name="title">Title for search purposes</param>
        /// <returns>Serialized to json format list of Job Offers</returns>
        /// <remarks>On exception this method returns json object containing error message</remarks>
        /// <seealso cref="CVEditor.Models.JobOffer"/>
        [HttpGet]
        public IActionResult JobOffer(string title)
        {
            try
            {
                var jobOffer = _context.JobOffers.ToList().FindAll((x) => title == null || title == "" || x.Title.Contains(title) || x.Location.Contains(title));
                var s = JsonConvert.SerializeObject(jobOffer);
                return new JsonResult(s);
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message);
            }
        }

        /// <summary>
        /// Asynchronously returns view of job offer with given id
        /// </summary>
        /// <param name="id">given id</param>
        /// <returns>view of job offer with given id</returns>
        /// <seealso cref="CVEditor.Models.JobOffer"/>
        public async Task<IActionResult> Details(int id)
        {
            var offer = await _context.JobOffers.FirstOrDefaultAsync(x => x.Id == id);
            return View(offer);
        }


        /// <summary>
        /// First searches for job offer in database with the given id.<p/>
        /// Then returns view of found offer.
        /// </summary>
        /// <param name="model">id of job offer</param>
        /// <returns>Bad request if id was null, notfound if job offer with id wasn't present, view of found job offer on success</returns>
        /// <remarks>Method finds job offer Asynchronously</remarks>
        /// <seealso cref="CVEditor.Models.JobOffer"/>
        [Authorize]
        [UserType(UserType.HR)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest($"id shouldn't be null");
            }
            JobOffer jobOffer = await _context.JobOffers.FirstOrDefaultAsync(u => u.Id == id.Value);
            HR hr = _context.HRs.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);

            if (jobOffer.HRId != hr.Id) return BadRequest("That is not your offer. GET OUT!");
            
            if (jobOffer == null)
            {
                return NotFound($"offer not found in DB");
            }

            return View(jobOffer);
        }


        /// <summary>
        /// First searches for job offer in database with the same id as the model passed as parameter.<p/>
        /// Then replaces current data in database with the given model or creates it if job offer wasn't existing.
        /// </summary>
        /// <param name="model">new or edited model</param>
        /// <returns>redirect to the details of new or edited model</returns>
        /// <remarks>Method performs search and applies changes Asynchronously</remarks>
        /// <seealso cref="CVEditor.Models.JobOffer"/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [UserType(UserType.HR)]
        public async Task<ActionResult> Edit(JobOffer model)
        {
            //if (model == null) return new EmptyResult();
            if (!ModelState.IsValid)
            {
                return View();
            }

            var offer = await _context.JobOffers.FirstOrDefaultAsync(x => x.Id == model.Id);
            offer.Description = model.Description;
            offer.Title = model.Title;
            offer.Location = model.Location;
            offer.SalaryFrom = model.SalaryFrom;
            offer.SalaryTo = model.SalaryTo;
            offer.ValidUntil = model.ValidUntil;
            _context.Update(offer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { model.Id });
        }

        /// <summary>
        /// Creates new empty job offer with default values
        /// </summary>
        /// <returns>view of new job offer</returns>
        /// <remarks>Method adds new job offer Asynchronously</remarks>
        /// <seealso cref="CVEditor.Models.JobOffer"/>
        [Authorize]
        [UserType(UserType.HR)]
        public async Task<ActionResult> Create()
        {
            var model = new JobOfferCompanies
            {
                Companies = await _context.Companies.ToListAsync()
            };

            return View(model);
        }

        /// <summary>
        /// Adds new job offer to the database.
        /// </summary>
        /// <param name="model">new model</param>
        /// <returns>redirect to the index page</returns>
        /// <remarks>Method adds new job offer Asynchronously</remarks>
        /// <seealso cref="CVEditor.Models.JobOffer"/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [UserType(UserType.HR)]
        public async Task<ActionResult> Create(JobOffer model)
        {
            HR hr = _context.HRs.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);

            JobOffer jobOffer = new JobOffer
            {
                CompanyId = hr.CompanyId,
                HRId = hr.Id,
                Id = model.Id,
                Description = model.Description,
                Title = model.Title,
                Location = model.Location,
                SalaryFrom = model.SalaryFrom,
                SalaryTo= model.SalaryTo,
                ValidUntil = model.ValidUntil,
                Created = DateTime.Now
            };

            await _context.JobOffers.AddAsync(jobOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Removes Asynchronously job offer with the given id from database
        /// </summary>
        /// <param name="id">given id of job offer</param>
        /// <returns>Bad request if id was null, notfound if job offer with id wasn't present, redirect to the index page in case of success</returns>
        /// <remarks>Method removes job offer Asynchronously</remarks>
        /// <seealso cref="CVEditor.Models.JobOffer"/>
        [Authorize]
        [UserType(UserType.HR)]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest($"id should not be null");
            }
            JobOffer jobOffer = await _context.JobOffers.FirstOrDefaultAsync(u => u.Id == id.Value);
            HR hr = _context.HRs.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);

            if (jobOffer.HRId != hr.Id) return BadRequest("That is not your offer. GET OUT!");
            var isEmpty = _context.JobOffers.Any((x) => x.Id == id.Value);
            if (isEmpty == false)
            {
                return NotFound($"id doesn't exist");

            }


            _context.JobOffers.Remove(jobOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Method for ajax to edit job offer in form without reloading page.
        /// </summary>
        /// <param name="id">id of the offer</param>
        /// <param name="title">title of the offer</param>
        /// <param name="salaryfrom">minimum salary</param>
        /// <param name="salaryto">maximum salary</param>
        /// <param name="location">location of offer</param>
        /// <param name="description">description of offer</param>
        /// <param name="valid">expiration date of the offer passed as string with format acceptable by Date.Parse()</param>
        /// <returns>json object containing information whatever edit was successful or not</returns>
        /// <remarks>On exception this method returns json object containing error message</remarks>
        /// <seealso cref="CV_Editor.Models.JobOffer"/>
        [HttpPost]
        [Authorize]
        [UserType(UserType.HR)]
        public string EditAjax(string salaryFrom, string salaryTo, string description, string title, string location, DateTime validUntil, int jobId)
        {
            int s1, s2;
            if (validUntil.CompareTo(DateTime.Now) < 0)
            {
                return "datefailure";
            }
            if (!Int32.TryParse(salaryFrom, out s1))
            {
                return "notdecimalfailure";
            }
            if (!Int32.TryParse(salaryTo, out s2))
            {
                return "notdecimalfailure";
            }

            if (s1 < 0 || s2 < 0)
            {
                return "negativesalaryfailure";
            }
            if (s1 > s2)
            {
                return "salaryfailure";
            }

            if (description.Length < 10)
            {
                return "descriptionfailure";
            }

            var offer = _context.JobOffers.FirstOrDefault(x => x.Id == jobId);
            offer.Description = description;
            offer.Title = title;
            offer.Location = location;
            offer.SalaryFrom = Int32.Parse(salaryFrom);
            offer.SalaryTo = Int32.Parse(salaryTo);
            offer.ValidUntil = validUntil;

            _context.Update(offer);
            _context.SaveChanges();
            return "success";
        }


        [Authorize]
        [UserType(UserType.User)]
        public async Task<IActionResult> Apply(int id)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);

            if (user.CVUrl == null) return BadRequest("There is no CV.");

            List<JobApplication> applications = _context.JobApplications.ToList().FindAll(x => x.UserId == user.Id);
            if (applications.Find(x => x.OfferId == id) != null) return BadRequest("You already applied for this job");

            JobApplication jobApplication = new JobApplication();

            jobApplication.ApplicationStatus = status.applied;
            jobApplication.Comment = "";
            jobApplication.OfferId = id;
            jobApplication.UserId = user.Id;




            SendGridMessage msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("", "work.com Team"));
            JobOffer jobOffer = _context.JobOffers.FirstOrDefault(u => u.Id == id);
            HR hr = _context.HRs.FirstOrDefault(h => h.Id == jobOffer.HRId);

            _context.JobApplications.Add(jobApplication);
            _context.SaveChanges();
            List<EmailAddress> recipients = new List<EmailAddress>
            {
                new EmailAddress(hr.EmailAddress)
            };

            msg.AddTos(recipients);

            msg.SetSubject("Someone applied to your job offer.");

            msg.AddContent(MimeType.Html, @"<p>Hi " + hr.Name + @",</p>
                            <p>" + HttpContext.User.Claims.First(claim => claim.Type.Contains("email")).Value + @" applied to one your job offers: " + jobOffer.Title + @".</p>
                            <p>&nbsp;</p>
                            <p>Their CV:</p>
                            <a href=" + user.CVUrl + ">CV&nbsp;</a></br> <p>Best regards</p> <p>work.com Team</p>");


            string apiKey = _config.GetSection("SENDGRID_API_KEY").Value;


            SendGridClient client = new SendGridClient(apiKey);
            var response = client.SendEmailAsync(msg);


            return RedirectToAction("Index");
        }
    }
}