using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model.Requests
{
    public class PasswordChangeRequest
    {
        public string OldPassword {get; set;} = "";
        public string NewPassword {get; set;} = "";
    }
}