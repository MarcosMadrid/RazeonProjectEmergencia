using Microsoft.AspNetCore.Mvc.Filters;
using RazeonProject.Helpers;

namespace RazeonProject.Filters
{
    public class GlobalBuilderView : ActionFilterAttribute
    {        
        private HelperWwwroot helperWwwroot;

        public GlobalBuilderView() { }

        public GlobalBuilderView(HelperWwwroot helperWwwroot)
        {
            this.helperWwwroot = helperWwwroot;
        }

        //public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        //{
        //    var resultContext = await next();

        //    var actionPath = Path.Combine(context.RouteData.Values["action"].ToString());            
        //    await helperWwwroot.DeleteFilesFromWwwrootAsync(actionPath + ".css", "ViewFiles");
        //    await helperWwwroot.DeleteFilesFromWwwrootAsync(actionPath + ".js", "ViewFiles");

        //}

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await helperWwwroot.RestoreWwwrootFolderAsync("ViewFiles");

            var actionPath = Path.Combine(context.RouteData.Values["controller"].ToString(), context.RouteData.Values["action"].ToString(), context.RouteData.Values["action"].ToString());
            await helperWwwroot.BuildTemporalFileWwwrootAsync(actionPath + ".js", "ViewFiles");
            await helperWwwroot.BuildTemporalFileWwwrootAsync(actionPath + ".css", "ViewFiles");

            await next();
        }

    }
}
