using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model
{
    [Obsolete]
    public class Reply(
        RegularUser postedBy, 
        List<RegularUser> interestedUsers,
        DateTime postedAt,
        List<PostFile> postFiles
    )
    : PostBase(postedBy, interestedUsers, postedAt, postFiles)
    {}
}