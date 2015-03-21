using MegaCityOne.Example.Mvc.Models;
using MegaCityOne.Mvc;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;

namespace MegaCityOne.Example.Mvc.Controllers
{
    [McoAuthenticate]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            McoCitizen citizen = McoSession.GetCitizen(HttpContext);
            if (citizen != null)
            {
                var homeModel = new HomeModel();
                homeModel.User = citizen.Name;
                foreach (var role in citizen.Roles)
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

            McoSession.Login(
                HttpContext, 
                new McoCitizen(model.User, roles.ToArray()));

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Logoff()
        {
            Session["User"] = null;
            McoSession.Logoff(HttpContext);
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