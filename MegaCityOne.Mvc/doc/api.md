
# MegaCityOne.Mvc


## JudgeAuthorizeAttribute

This attribute leverage MegaCityOne's Judge security for MVC applications.


### Constructor(rule)

Creates an instance of a JudgeAuthorizeAttribute.

| Name | Description |
| ---- | ----------- |
| rule | *System.String*<br>The rule to be advised during the MVC authorize process. |

### OnAuthorization(filterContext)

This method executes authorization based on the Judge returned by the MegaCityOne.Mvc.Dispatcher.

| Name | Description |
| ---- | ----------- |
| filterContext | *System.Web.Mvc.AuthorizationContext*<br>The authorization context. |

### Rule

The rule to be advised by the Judge upon authorization request.


## JudgeSummonDelegate

This delegate is used to define a Summon event.

| Name | Description |
| ---- | ----------- |
| source | *Unknown type*<br>The source of the event. |
| e | *Unknown type*<br>The Judge summon event args. |

## JudgeSummonEventArgs

Event arguments for a Dispatcher.Summon event.


### Constructor

Creates an instance of SummonEventArgs.


### Respondent

The Judge who answers the summoning from the Dispatcher.


## McoAuthenticateAttribute

Attribute is used to authenticate the current HttpContext.User with the IPrincipal generated using the current logged McoCitizen in the current McoSession.


### OnAuthentication(filterContext)

Sets the filterContext.HttpContext.User to the principal built from the McoCitizen of the current McoSession (if there is any McoCitizen in the session).

| Name | Description |
| ---- | ----------- |
| filterContext | *System.Web.Mvc.Filters.AuthenticationContext*<br>The context of the current ActionFilter |

### OnAuthenticationChallenge(filterContext)

Verifies if the user associated with the current HttpContext is authenticated.

| Name | Description |
| ---- | ----------- |
| filterContext | *System.Web.Mvc.Filters.AuthenticationChallengeContext*<br>The context of the current ActionFilter |

## McoCitizen




### Constructor(name, roles)

Creates an instance of an McoCitizen.

| Name | Description |
| ---- | ----------- |
| name | *System.String*<br>The name of the citizen |
| roles | *System.String[]*<br>Roles given to the citizen |

### CreatePrincipal

By default, this virtual method creates A GenericPrincipal with the informations available in the current citizen. The GenericIdentity.BootstrapContext is set to the current citizen for references in rules. Override this method to create an IPrincipal corresponding to your application needs.


#### Returns

A security Principal


### Data

Gets useful data attached to the current citizen.


### Name

Gets or sets the name of the current citizen.


### Principal

Gets the security principal corresponding to the current citizen.


### Roles

Gets or sets roles given to the current citizen.


## McoDispatcher

The Judge Dispatcher is responsible to check if a Judge is available for the call. If no Judge is available, a Judge will be summoned. Dispatched Judges must be returned to the pool by using the Dispatcher.Return method. Otherwise, the Dispatch method will summon a new Judge on each call. The Dispatcher as a JudgePool. This class is a singleton and cannot be instanciated. You must use the static member Dispatcher.Current to use an instance of this class. This class is thread safe.


### Advise(law, arguments)

Dispatch this Advise call to an available Judge in the pool.

| Name | Description |
| ---- | ----------- |
| law | *System.String*<br>The law to Advise. |
| arguments | *System.Object[]*<br>Optionnal arguments provided to help the judge to give his advice. By default, the first argument is always the HttpContext.Current. |


#### Returns

True is the law is respected, false otherwise.


### Current

The static dispatcher instance for the current application.


### Dispatch

Thread safe. Calling the dispatch method can trigger the Summon event if there is no Judge available in the pool. If this is the case, it is assumed that a Summon event handler will create a Judge and asign it to the SummonEventArgs.Respondent property. Otherwise, return an existing Judge from the pool.


#### Returns

A Judge available to answer the call.


### Enforce(law, arguments)

Dispatch this Enforce call to an available Judge in the pool.

| Name | Description |
| ---- | ----------- |
| law | *System.String*<br>The law to Enforce. |
| arguments | *System.Object[]*<br>Optionnal arguments provided to help the judge to enforce the law. By default, the first argument is always the HttpContext.Current. |

### OnSummon(e)

Method used to fire a Summon event.

| Name | Description |
| ---- | ----------- |
| e | *MegaCityOne.Mvc.JudgeSummonEventArgs*<br>The event arguments. |

### Principal

Gets the Principal of the current thread.


### Returns(judge)

Thread safe. Returns a dispatched judge to the pool. This method do not accept a judge that have not been dispatched by the current instance of the dispatcher.

| Name | Description |
| ---- | ----------- |
| judge | *MegaCityOne.Judge*<br>The judge that answered a previous call to Dispatch. |

### Summon

Event fired when there is no Judge available for the current thread id. The event handler is expected to create a Judge, provide it with laws and attach it the the event args.


## McoSession

This static class offer methods to manage a Mega-City One session .


### GetCitizen(context)

Retrieves the

| Name | Description |
| ---- | ----------- |
| context | *System.Web.HttpContextBase*<br> |


#### Returns




### Login(context, citizen)

Register the given citizen in the current Mega-City One session.

| Name | Description |
| ---- | ----------- |
| context | *System.Web.HttpContextBase*<br>The HttpContext of the current request |
| citizen | *MegaCityOne.Mvc.McoCitizen*<br>The Mega-City One citizen |

### Logoff(context)

Unregister the registered Mega-City One citizen from the session.

| Name | Description |
| ---- | ----------- |
| context | *System.Web.HttpContextBase*<br>The HttpContext of the current request. |

