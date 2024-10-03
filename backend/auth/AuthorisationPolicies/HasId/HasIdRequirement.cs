

using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class HasIdRequirement
(string idParamName) 
: IAuthorizationRequirement
{   
    public string IdParamName {get; init;} = idParamName;

}