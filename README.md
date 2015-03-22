# MegaCityOne-Mvc
MegaCityOne MVC 5 Helper Library

[Google Group](http://bit.ly/19bXnDv)

This library offers an efficient MVC 5 implementation of 
[MegaCityOne](https://github.com/formix/MegaCityOne) security library.

## Installation

MegaCityOne-MVC is available at 
[Nuget.org](https://www.nuget.org/packages/MegaCityOne-Mvc/) and can be 
installed as a package using VisualStudio NuGet package manager or via 
NuGet command line:

> Install-Package MegaCityOne.Mvc

# Documentation

* [Api Reference](https://github.com/formix/MegaCityOne-Mvc/blob/master/MegaCityOne.Mvc/doc/api.md)

# Usage

## Authentication

Like specified for [MegaCityOne](https://github.com/formix/MegaCityOne) 
library, you are responsible of validating the user name and password and then
retrieve user information from your database. 

## Configuration

In App_Start/MegaCityOneActivator.cs Edit the McoDispatcher_Current_Summon method 
to create your Judge and give it to the eventarg.Respondent:

```c#
static void McoDispatcher_Current_Summon(object source, JudgeSummonEventArgs e)
{
    // *** instanciate the Judge you need here. *** \\
    JudgeDredd judge = new JudgeDredd();
    
    // *** Configure your judge as needed here *** \\

    judge.Laws.Add("CanCreateProject", (principal, arguments) =>
    {
        // HttpContext is always the first argument whe in MVC 
        // context. Any other arguments can be found after this one.
        HttpContext httpContext = (HttpContext)arguments[0];

        bool jusgeAnswer = false;
        // do some coding here

        return judgeAnswer;

    }
    
    e.Respondent = judge;
}

```

See the complete example [here](https://github.com/formix/MegaCityOne-Mvc/blob/master/MegaCityOne.Mvc.Example/App_Start/MegaCityOneActivator.cs).

## Securing Your Controllers

Add the `McoAuthenticateAttribute` to your Controller classes. On action methods 
that do not require authentication, add the `AllowAnonymousAttribute`. To add
authorization to your actions, add the `JudgeAuthorizeAttribute(lawName)` 
with a law name (as defined in the MegaCityOneActivator.cs file) on desired 
actions.

```c#
[McoAuthenticate]
public class HomeController : Controller
{
    [AllowAnonymous]
    public ActionResult Index()
    {
        // some code...
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult Index(HomeModel model)
    {
        // some code...
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult Logoff()
    {
        // some code...
    }

    [JudgeAuthorize("CanCreateProject")]
    public ActionResult CreateProject()
    {
        // some code...
    }

    [JudgeAuthorize("CanManageUsers")]
    public ActionResult ManageUsers()
    {
        // some code...
    }
}
```

## Starting a Security Session

Once your user has successfully loged in, you should do the folowing in order 
to register it in MegaCityOne Session:

```c#
McoCitien citizen = new McoCitizen(
    "jhetfield", // User name
    new string[] {"Musician", "Composer", "Producer"}); // User roles

// You can add relevant data to the citizen using the "Data" property.
citizen.Data["Label"] = "Elektra Records";
citizen.Data["Birth"] = new DateTime(1963, 8, 3);

McoSession.Login(HttpContext, citizen);
```

## Ending a Security Session

```c#
McoSession.Logout(HttpContext); // Yeah, just that...
```

### Some Details On MCO Sessions

MegaCityOne.Mvc offers a session manager to keep track of user's security 
session. The session id (a GUID) is kept in an HttpOnly cookie named "mco".
This session id is different from the ASP.NET session id. MegaCityOne 
security session information is not kept in the server's Session object but 
in the System.Runtime.Caching.MemoryCache.Default instance. The session 
contains the given McoCitizen. This gives more flexibility on session 
management. For more security, when a McoCitizen is stored in the cache, we 
save the requestor ip address in the McoCitizen.Data["__host"] value. 
If by any means the requestor IP address changes, the current session is 
terminated. When a session is created, it's set to last 24 hours.

## Razor Usage

To hide (or display) different part of your view, use McoDispatcher static 
helper class.

```cshtml
<div class="panel panel-default">
  <div class="panel-heading">
    <h3 class="panel-title">Actions</h3>
  </div>
  <div class="panel-body">
    @if (McoDispatcher.Advise("CanCreateProject"))
    {
      @Html.ActionLink("Create Project", "CreateProject", "Home");<br />
      ViewBag.ActionAvailable = true;
    }
    @if (McoDispatcher.Advise("CanManageUsers"))
    {
      @Html.ActionLink("Manage Users", "ManageUsers", "Home");<br />
      ViewBag.ActionAvailable = true;
    }
    @if (!ViewBag.ActionAvailable)
    {
      <div>There is no action available for you.</div>
    }
  </div>
</div>
```
## Note on Thread Safety

Always use `McoDispatcher` static class to Advise or Enforce within a Web 
application. McoDispatcher static methods insure that thread safe calls are 
made to the Respondent Judge. Judge implementations aren't thread safe.

## Detailed Example

To get the detailed Mvc example, clone the github project.

> https://github.com/formix/MegaCityOne-Mvc.git
