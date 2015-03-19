using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace MegaCityOne.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public abstract class AuthenticateAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            var userInfo = (UserInfo)SecuritySession.GetUserInfo(filterContext.HttpContext);
            if (userInfo != null)
            {
                filterContext.HttpContext.User = new GenericPrincipal(
                    new GenericIdentity(userInfo.Name), userInfo.Roles);
            }

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (AllowAnonymous(filterContext))
            {
                return;
            }

            var user = filterContext.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }



        private bool AllowAnonymous(AuthenticationChallengeContext filterContext)
        {
            return filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
        }
    }
}
