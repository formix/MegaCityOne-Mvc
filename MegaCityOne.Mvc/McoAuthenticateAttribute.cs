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
    /// <summary>
    /// Attribute is used to authenticate the current HttpContext.User with 
    /// the IPrincipal generated using the current logged McoCitizen in the 
    /// current McoSession.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class McoAuthenticateAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// Sets the filterContext.HttpContext.User to the principal built 
        /// from the McoCitizen of the current McoSession (if there is any 
        /// McoCitizen in the session).
        /// </summary>
        /// <param name="filterContext">The context of the current 
        /// ActionFilter</param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            McoCitizen citizen = McoSession.GetCitizen(filterContext.HttpContext);
            if (citizen != null)
            {
                filterContext.HttpContext.User = citizen.Principal;
            }
        }

        /// <summary>
        /// Verifies if the user associated with the current HttpContext is 
        /// authenticated.
        /// </summary>
        /// <param name="filterContext">The context of the current 
        /// ActionFilter</param>
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
