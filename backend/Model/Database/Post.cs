using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model
{

    public class Post(
        RegularUser postedBy,
        List<RegularUser> interestedUsers,
        DateTime postedAt,
        List<PostFile> postFiles,
        string content,
        List<Post> replies,
        bool isReply
    ) : PostBase(postedBy, interestedUsers, postedAt, postFiles)
    {
        protected Post(DateTime postedAt, string content, bool isReply)
        : this
        (null!, [], postedAt, [], content, [], isReply)
        {}

        public string Content { get; set; } = content;
        public virtual List<Post> Replies { get; set; } = replies;
        public bool IsReply { get; set; } = isReply;
        public void Update( Post post ){
            this.Id = post.Id;
            this.Content = post.Content;
            this.Replies = [.. post.Replies];
            this.PostedBy = post.PostedBy;
            this.InterestedUsers = [.. post.InterestedUsers];
            this.IsReply = post.IsReply;
        }
    }
}