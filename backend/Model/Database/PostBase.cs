using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model
{
    public class PostBase(
        RegularUser postedBy, 
        List<RegularUser> interestedUsers,
        DateTime postedAt,
        List<PostFile> postFiles
    )
    {

        protected PostBase(DateTime postedAt)
        : this(null!, [], postedAt, [])
        {}

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long Id {get; set;}
        public virtual RegularUser PostedBy {get; set;} = postedBy;
        public virtual List<RegularUser> InterestedUsers { get; set; } = interestedUsers;
        public DateTime PostedAt {get; set;} = postedAt;
        public virtual List<PostFile> PostFiles {get; set;} = postFiles; 
        public void Update(PostBase postBase)
        {
            this.PostedBy = postBase.PostedBy;
            this.InterestedUsers = postBase.InterestedUsers;
            this.PostedAt = postBase.PostedAt;
            this.PostFiles = postBase.PostFiles;
        }
    }
}