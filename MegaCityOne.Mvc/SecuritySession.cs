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
    public static class SecuritySession
    {

        public static void Login(HttpContextBase context, UserInfo userInfo)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (userInfo == null)
            {
                throw new ArgumentNullException("user");
            }

            Logoff(context);  // Cleanse the context before proceeding
            string securitySessionId = Guid.NewGuid().ToString();
            DateTime expiration = DateTime.Now.AddMonths(6);
            context.Response.Cookies.Set(new HttpCookie("mco", securitySessionId)
            {
                Expires = expiration,
                HttpOnly = true
            });

            userInfo.Data["__host"] = context.Request.UserHostAddress;
            MemoryCache.Default.Add(securitySessionId, userInfo, new DateTimeOffset(expiration));

            LogManager.GetLogger("MegaCityOne.Mvc.SecuritySession").Info(
            string.Format("User session started for '{0}' from [{1}].",
            userInfo.Name,
            context.Request.UserHostAddress));
        }


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
                    UserInfo userInfo = (UserInfo)MemoryCache.Default.Get(mcoCookie.Value); 
                    MemoryCache.Default.Remove(mcoCookie.Value);
                    LogManager.GetLogger("MegaCityOne.Mvc.SecuritySession").Info(
                    string.Format("User session terminated for '{0}' from [{1}].",
                    userInfo.Name,
                    context.Request.UserHostAddress));
                }
                mcoCookie.Expires = DateTime.Now.AddDays(-1d);
                context.Response.Cookies.Set(mcoCookie);
            }
        }


        public static UserInfo GetUserInfo(HttpContextBase context)
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

            UserInfo userInfo = (UserInfo)MemoryCache.Default.Get(mcoCookie.Value);
            if (userInfo == null)
            {
                mcoCookie.Expires = DateTime.Now.AddDays(-1d);
                context.Response.SetCookie(mcoCookie);
                return null;
            }

            if (context.Request.UserHostAddress != ((string)userInfo.Data["__host"]))
            {
                LogManager.GetLogger("MegaCityOne.Mvc.SecuritySession").Warn(
                    string.Format("Request host address [{0}] do not match with stored user host address [{1}] for user '{2}'.",
                    context.Request.UserHostAddress,
                    userInfo.Data["__host"],
                    userInfo.Name));
                Logoff(context);
                return null;
            }

            return userInfo;
        }
    }
}
