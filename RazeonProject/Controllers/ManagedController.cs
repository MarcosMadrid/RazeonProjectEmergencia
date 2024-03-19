using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RazeonProject.Helpers;
using RazeonProject.Models;
using RazeonProject.Repositories.Interfaces;
using System.Security.Claims;
using RazeonProject.Filters;

namespace RazeonProject.Controllers
{
    [ServiceFilter(typeof(GlobalBuilderView))]
    public class ManagedController : Controller
    {
        IRepositoryRazeonBBDD repositoryRazeon;
        HelperWwwroot wwwroot;

        public ManagedController(IRepositoryRazeonBBDD repositoryRazeon, HelperWwwroot helperWwwroot)
        {
            wwwroot = helperWwwroot;
            this.repositoryRazeon = repositoryRazeon;
        }

        public IActionResult LogIn()
        {
            return View("LogIn/LogIn");
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            User? user = null;
            user = await repositoryRazeon.LogIn(email, password);

            if(user != null)
            {                
                ClaimsIdentity identity = new ClaimsIdentity
                    (                    
                        ClaimTypes.Name,
                        ClaimTypes.Email,
                        ClaimTypes.Role
                    );
                Claim claimName = new Claim(ClaimTypes.Name, user.Name!);
                Claim claimEmail = new Claim(ClaimTypes.Email, user.Email!);
                Claim claimIdUser = new Claim("IdUser", user.Id.ToString());
                Claim claimRole = new Claim(ClaimTypes.Role, user.RolId.ToString());
                identity.AddClaim(claimName);
                identity.AddClaim(claimEmail);
                identity.AddClaim(claimIdUser);
                identity.AddClaim(claimRole);

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);                
                
                if (user.Image is not null)
                {
                    wwwroot.BuildTemporalImgWwwroot(user.Image, "assets\\imgs", "userImage");
                }
            }

            return RedirectToAction("Index", "Razeon");
        }        
    }
}
