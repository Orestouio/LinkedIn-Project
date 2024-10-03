

using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class IsMemberOfConnectionRequirement
(string connectionIdParamName) 
: IAuthorizationRequirement
{   
    public string ConnectionIdParamName {get; init;} = connectionIdParamName;
}