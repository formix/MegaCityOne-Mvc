using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MegaCityOne.Mvc
{
    /// <summary>
    /// This static class offer methods to manage a Mega-City One session .
    /// </summary>
    public static class McoSession
    {

        /// <summary>
        /// Register the given citizen in the current Mega-City One session.
        /// </summary>
        /// <param name="context">The HttpContext of the current request</param>
        /// <param name="citizen">The Mega-City One citizen</param>
        public static void Login(HttpContextBase context, McoCitizen citizen)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (citizen == null)
            {
                throw new ArgumentNullException("user");
            }

            Logoff(context);  // Cleanse the context before proceeding
            string securitySessionId = Guid.NewGuid().ToString();
            DateTime expiration = DateTime.Now.AddDays(1);
            context.Response.Cookies.Set(new HttpCookie("mco", securitySessionId)
            {
                Expires = expiration,
                HttpOnly = true
            });

            citizen.Data["UserHostAddress"] = context.Request.UserHostAddress;
            MemoryCache.Default.Add(securitySessionId, citizen, new DateTimeOffset(expiration));

            LogManager.GetLogger("MegaCityOne.Mvc.McoSession").Info(
            string.Format("User session started for '{0}' from [{1}].",
            citizen.Name,
            context.Request.UserHostAddress));

            context.User = citizen.Principal;
        }

        /// <summary>
        /// Unregister the registered Mega-City One citizen from the session.
        /// </summary>
        /// <param name="context">The HttpContext of the current request.</param>
        public static void Logoff(HttpContextBase context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            
            HttpCookie mcoCookie = context.Request.Cookies.Get("mco");
            if (mcoCookie != null)
            {
                if (MemoryCache.Default.Contains(mcoCookie.Value))
                {
                    McoCitizen userInfo = (McoCitizen)MemoryCache.Default.Get(mcoCookie.Value); 
                    MemoryCache.Default.Remove(mcoCookie.Value);
                    LogManager.GetLogger("MegaCityOne.Mvc.McoSession").Info(
                    string.Format("User session terminated for '{0}' from [{1}].",
                    userInfo.Name,
                    context.Request.UserHostAddress));
                }
                mcoCookie.Expires = DateTime.Now.AddDays(-1d);
                context.Response.Cookies.Set(mcoCookie);
            }
        }

        /// <summary>
        /// Retrieves the 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static McoCitizen GetCitizen(HttpContextBase context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            
            HttpCookie mcoCookie = context.Request.Cookies.Get("mco");
            if (mcoCookie == null)
            {
                return null;
            }

            McoCitizen citizen = (McoCitizen)MemoryCache.Default.Get(mcoCookie.Value);
            if (citizen == null)
            {
                mcoCookie.Expires = DateTime.Now.AddDays(-1d);
                context.Response.SetCookie(mcoCookie);
                return null;
            }

            if (context.Request.UserHostAddress != ((string)citizen.Data["UserHostAddress"]))
            {
                LogManager.GetLogger("MegaCityOne.Mvc.SecuritySession").Warn(
                    string.Format("Request host address [{0}] do not match with stored user host address [{1}] for user '{2}'.",
                    context.Request.UserHostAddress,
                    citizen.Data["UserHostAddress"],
                    citizen.Name));
                Logoff(context);
                return null;
            }

            return citizen;
        }
    }
}
