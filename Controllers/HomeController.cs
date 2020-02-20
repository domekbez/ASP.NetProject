using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CVEditor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using CVEditor.EntityFramework;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.IO;

namespace CVEditor.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        static CloudBlobClient blobClient;
        const string blobContainerName = "cveditorstorage";
        static CloudBlobContainer blobContainer;
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public HomeController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }
              
        [Authorize]
        [UserType(UserType.Admin)]
        [ActionName("Index")]
        public IActionResult IndexAdmin()
        {
            return RedirectToAction("Index", "JobOffer");
        }

        [Authorize]
        [UserType(UserType.HR)]
        [ActionName("Index")]
        public IActionResult IndexHR()
        {
            return RedirectToAction("Index", "JobOffer");
        }

        [Authorize]
        [UserType(UserType.User)]
        [ActionName("Index")]
        public IActionResult IndexUser()
        {
            User user = _context.Users.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);
            return View("~/Views/Home/IndexUser.cshtml", user);
        }

        [ActionName("Index")]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "JobOffer");
        }

        [Authorize]
        [UserType(UserType.User)]
        [HttpPost]
        public async Task<IActionResult> uploadCVAsync()
        {

            var storageConnectionString = _config.GetSection("StorageConnectionString").Value;
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            blobClient = storageAccount.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference(blobContainerName);
            await blobContainer.CreateIfNotExistsAsync();
            await blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });


            var request = await HttpContext.Request.ReadFormAsync();
            if (request.Files == null || request.Count > 1)
            {
                return BadRequest("No or too many files");
            }

            var file = request.Files.First();

            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(GetRandomBlobName(file.FileName));
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadFromStreamAsync(stream);

            }


            User user = _context.Users.FirstOrDefault(u => u.NameId == HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value);
            user.CVUrl = blob.StorageUri.PrimaryUri.ToString();
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}...{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }
            
        [AllowAnonymous]
        public IActionResult About()
        {
            if(User.Identity.IsAuthenticated)
                ViewData["Message"] = "Hi " + User.FindFirst(ClaimTypes.GivenName).Value + "!";
            else
                ViewData["Message"] = "Hi unknown user!";


            return View(UserType.User);
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
               







        public enum UserType { User, Admin, HR};

        [AttributeUsage(AttributeTargets.Method)]
        public class UserTypeAttribute : ActionMethodSelectorAttribute
        {
            private readonly UserType _userType;

            public UserTypeAttribute(UserType userType)
            {
                _userType = userType;
            }

            public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
            {
                if (!routeContext.HttpContext.User.Identity.IsAuthenticated) return false;
                string nameId = routeContext.HttpContext.User.Claims.First(claim => claim.Type.Contains("nameidentifier")).Value;
                UserType userType = UserTypeManager.StaffData.CheckUserType(nameId);
                return _userType == userType;
            }
        }       

    }
}
