using System;
using System.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MegaCityOne.Mvc.Example.MegaCityOneActivator), "ApplicationStart")]

namespace MegaCityOne.Mvc.Example
{
    public static class MegaCityOneActivator
    {
        /// <summary>
        /// Will run when the application is starting (same as Application_Start)
        /// </summary>
        public static void ApplicationStart()
        {
            JudgeDispatcher.Current.Summon += JudgeDispatcher_Current_Summon;
        }

        static void JudgeDispatcher_Current_Summon(object source, JudgeSummonEventArgs e)
        {
            // *** instanciate the Judge you need here. *** \\
            JudgeDredd judge = new JudgeDredd();


            // *** Configure your judge as needed here *** \\

            judge.Laws.Add("CanCreateProject", (principal, arguments) =>
            {
                // HttpContext is always the first argument whe in MVC 
                // context. Any other arguments can be found after this one.
                HttpContext httpContext = (HttpContext)arguments[0];

                // The create project link is displayed only if the current Principal 
                // is in the "ProjectManager" role and only if we are between 1am and 11pm.
                // This is an example displaying MegaCityOne flexibility, do 
                // your Laws the way you want!
                var startTime = DateTime.MinValue.AddHours(1);
                var endTime = DateTime.MinValue.AddHours(23); // Dunno if MinValue is UTC or Local though...
                var time = DateTime.MinValue.Add(
                    DateTime.Now.Subtract(DateTime.Now.Date));

                return principal.IsInRole("ProjectManager") &&
                    (time.CompareTo(startTime) >= 0) &&
                    (time.CompareTo(endTime) < 0);
            });

            judge.Laws.Add("CanManageUsers", (principal, arguments) =>
            {
                return principal.IsInRole("Administrator");
            });


            // *** Keep this line since the returned judge is set here. *** \\
            e.Respondent = judge;
        }
    }
}
