

using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class IsMemberOfConversationRequirement
(string firstUserParamName, string secondUserParamName) 
: IAuthorizationRequirement
{   
    public string FirstUserParamName {get; init;} = firstUserParamName;
    public string SecondUserParamName {get; init;} = secondUserParamName;

}