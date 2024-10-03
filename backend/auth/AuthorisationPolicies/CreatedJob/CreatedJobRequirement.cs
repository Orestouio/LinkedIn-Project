

using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class CreatedJobRequirement
(string postIdName) 
: IAuthorizationRequirement
{   
    public string JobIdParamName {get; init;} = postIdName;

}