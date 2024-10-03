using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BackendApp.Model
{
    [method: JsonConstructor]
    public class JobPost
    (   
        RegularUser postedBy,
        List<RegularUser> interestedUsers,
        DateTime postedAt,
        List<PostFile> postFiles,
        string jobTitle,
        string description,
        string[] requirements
    ) 
    : PostBase(postedBy, interestedUsers, postedAt, postFiles)
    {

        protected JobPost(DateTime postedAt, string jobTitle, string description, string[] requirements)
        : this
        (null!,[], postedAt, [], jobTitle, description, requirements)
        {}

        public string JobTitle {get; set;} = jobTitle;
        public string Description {get; set;} = description;
        public string[] Requirements {get; set;} = requirements;

        public void Update(JobPost job) {
            base.Update(job);
            this.PostedBy = job.PostedBy;
            this.InterestedUsers = job.InterestedUsers;
            this.JobTitle = job.JobTitle;
            this.Description = job.Description;
            this.Requirements = job.Requirements;
        }
    }
}