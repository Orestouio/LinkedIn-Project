using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model.Requests
{
    public class RegisterRequest
    {
        public string Email { get; set; }  = "";
        public string Password { get; set; }  = "";
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Location { get; set; } = "";
        public string? CurrentPosition { get; set; } = "";
        public string? ImageName { get; set; } = "";
    }
}