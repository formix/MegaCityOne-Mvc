using MegaCityOne.Example.Mvc.Attributes;
using MegaCityOne.Example.Mvc.Models;
using MegaCityOne.Mvc;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;

namespace MegaCityOne.Example.Mvc.Controllers
{
    [SessionAuthenticate]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                UserInfo userData = (UserInfo)Session["User"];
                var homeModel = new HomeModel();
                homeModel.User = userData.Name;
                foreach (var role in userData.Roles)
                {
                    homeModel.SetSelected(role, true);
                }

                return View(homeModel);
            }
            else
            {
                return View(new HomeModel()
                {
                    User = "formix",
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(HomeModel model)
        {
            var roles = new List<string>();
            foreach (var role in model.Roles)
            {
                if (role.Selected)
                {
                    roles.Add(role.Name);
                }
            }

            var user = new UserInfo()
            {
                Name = model.User,
                Roles = roles.ToArray()
            };

            Session["User"] = user;

            HttpContext.User = new GenericPrincipal(
                new GenericIdentity(user.Name), user.Roles);

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Logoff()
        {
            Session["User"] = null;
            HttpContext.User = new GenericPrincipal(new GenericIdentity(""), new string[0]);
            return Redirect("~/Home/Index");
        }

        [JudgeAuthorize("CanCreateProject")]
        public ActionResult CreateProject()
        {
            return Content("Project created");
        }

        [JudgeAuthorize("CanManageUsers")]
        public ActionResult ManageUsers()
        {
            return Content("You can manage users");
        }
    }
}