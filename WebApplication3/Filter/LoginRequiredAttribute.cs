using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Filter
{
    public class LoginRequiredAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result =new  RedirectResult("~/Account/Login");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
           
        }
    }
}
