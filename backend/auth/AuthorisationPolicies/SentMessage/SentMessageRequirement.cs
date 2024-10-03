

using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class SentMessageRequirement
(string messageIdParamName) 
: IAuthorizationRequirement
{   
    public string MessageIdParamName {get; init;} = messageIdParamName;

}