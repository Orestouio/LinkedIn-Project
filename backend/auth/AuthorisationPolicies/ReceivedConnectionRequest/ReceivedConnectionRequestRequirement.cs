

using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class ReceivedConnectionRequestRequirement
(string connectionIdParamName) 
: IAuthorizationRequirement
{   
    public string ConnectionIdParamName {get; init;} = connectionIdParamName;

}