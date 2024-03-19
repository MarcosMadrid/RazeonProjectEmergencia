using Microsoft.AspNetCore.Mvc;
using RazeonProject.Filters;
using RazeonProject.Helpers;
using RazeonProject.Models;
using RazeonProject.Repositories.Interfaces;

namespace RazeonProject.Controllers
{    
    [ServiceFilter(typeof(GlobalBuilderView))]
    public class PublicController : Controller
    {
        IRepositoryRazeonBBDD repositoryRazeon;
        HelperWwwroot wwwroot;

        public PublicController(IRepositoryRazeonBBDD repositoryRazeon, HelperWwwroot helperWwwroot)
        {
            wwwroot = helperWwwroot;
            this.repositoryRazeon = repositoryRazeon;
        }

        public IActionResult Index()
        {
            return View("Index/Index");
        }

        public IActionResult SignUp()
        {
            return View("SignUp/SignUp");
        }

        [HttpPost]
        public IActionResult SignUp(User user, IFormFile Image)
        {
            if (Image != null && Image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    Image.CopyTo(memoryStream);
                    user.Image = memoryStream.ToArray();
                }
            }
            repositoryRazeon.SignUp(user.Name!, user.Email!, user.Password!, user.Image, user.RolId);
            return RedirectToAction("Index", "Razeon");
        }
    }
}
