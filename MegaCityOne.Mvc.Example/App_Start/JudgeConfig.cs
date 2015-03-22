﻿using MegaCityOne.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegaCityOne.Example.Mvc
{
    public class JudgeConfig
    {
        public static void RegisterJudge(McoDispatcher dispatcher)
        {
            dispatcher.Summon += dispatcher_Summon;
        }


        static void dispatcher_Summon(object source, JudgeSummonEventArgs e)
        {
            JudgeDredd dredd = new JudgeDredd();
            
            dredd.Laws.Add("CanCreateProject", (principal, arguments) =>
            {
                // HttpContext is always the first argument whe in MVC 
                // context. Any other arguments can be found after this one.
                HttpContext httpContext = (HttpContext)arguments[0];

                // The create project link is displayed only if the current Principal 
                // is in the "ProjectManager" role and only if we are between 1am and 11pm.
                var startTime = DateTime.MinValue.AddHours(1); 
                var endTime = DateTime.MinValue.AddHours(23); // Dunno if MinValue is UTC or Local though...
                var time = DateTime.MinValue.Add(
                    DateTime.Now.Subtract(DateTime.Now.Date));

                return principal.IsInRole("ProjectManager")  &&
                    (time.CompareTo(startTime) >= 0) && 
                    (time.CompareTo(endTime) < 0);
            });

            dredd.Laws.Add("CanManageUsers", (principal, arguments) =>
            {
                return principal.IsInRole("Administrator");
            });

            e.Respondent = dredd;
        }
    }
}