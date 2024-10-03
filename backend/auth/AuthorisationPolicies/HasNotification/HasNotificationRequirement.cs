

using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class HasNotificationRequirement
(string notificationIdParamName) 
: IAuthorizationRequirement
{   
    public string NotificationIdParamName {get; init;} = notificationIdParamName;

}