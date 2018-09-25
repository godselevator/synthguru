using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AOS.Platform.Common
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
            }
        }
    }

    public sealed class UserRole
    {
        // Define values here.
        public static readonly UserRole User = new UserRole("User", "This defines a normal user");
        public static readonly UserRole Administrator = new UserRole("Administrator", "This defines an administrator");
        public static readonly UserRole SystemUser = new UserRole("System user", "This defines a system user");

        public readonly string Name;
        public readonly string Description;

        // Constructor is private: values are defined within this class only!
        private UserRole(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}