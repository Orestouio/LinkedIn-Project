using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model.Requests
{
    public class PostCreationRequest
    {
        public string Content { get; set; } = "";
        public PostFile[] PostFiles {get; set;} = [];
    }
}