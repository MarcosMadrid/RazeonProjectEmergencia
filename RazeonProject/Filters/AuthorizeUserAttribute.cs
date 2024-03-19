using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RazeonProject.Filters
{
    public class AuthorizeUserAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public AuthorizeUserAttribute() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity!.IsAuthenticated == false)
            {
                context.Result = this.RedirectTo("Managed", "LogIn");
            }
        }

        private RedirectToRouteResult RedirectTo(string controller, string action)
        {
            RouteValueDictionary ruta =
                new RouteValueDictionary(
                    new {controller,action});
            RedirectToRouteResult result =
                new RedirectToRouteResult(ruta);
            return result;
        }
    }
}
