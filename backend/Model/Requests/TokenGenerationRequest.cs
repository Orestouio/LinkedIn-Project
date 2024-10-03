using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model.Requests
{
    public class TokenGenerationRequest
    {
        public string Email { get; set; }  = "";
        public string Password { get; set; }  = "";
    }
}