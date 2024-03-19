using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace RazeonProject.Filters
{
    public class SendRazeonLayout : ActionFilterAttribute
    {
        public SendRazeonLayout() { }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ISession session = context.HttpContext.Session;

            string? controller = context.RouteData.Values["controller"]?.ToString();
            string? action = context.RouteData.Values["action"]?.ToString();

            if (session.Keys.Contains("controller") && session.Keys.Contains("action"))
            {
                string lastController = session.GetString("controller")!;
                string lastAction = session.GetString("action")!;

                if (controller == lastController && action == lastAction)
                {
                    // Redirect to "Razeon/Index" action
                    context.Result = RedirectTo("Razeon", "Index");
                    return;
                }
            }

            // Update session with current controller and action
            session.SetString("controller", controller ?? "");
            session.SetString("action", action ?? "");

            await context.HttpContext.Session.CommitAsync();

            await next();
        }

        private static RedirectToRouteResult RedirectTo(string controller, string action)
        {
            RouteValueDictionary ruta =
                new(
                    new { action, controller }
                );
            RedirectToRouteResult redirect = new(ruta);
            return redirect;
        }
    }
}
