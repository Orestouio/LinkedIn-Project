

using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class CreatedPostRequirement
(string postIdName) 
: IAuthorizationRequirement
{   
    public string PostIdParamName {get; init;} = postIdName;

}