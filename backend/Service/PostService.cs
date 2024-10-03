using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendApp.Data;
using BackendApp.Model;
using BackendApp.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackendApp.Service
{
    public interface IPostService
    {
        public Post? GetPostById(long id);
        public Post[] GetAllPosts();
        public bool AddPost(Post post);
        public bool RemovePost(long id);
        public Post? CreateNewPost(string post, RegularUser creator, PostFile[] postFiles);
        public UpdateResult UpdatePost(long id, Post postContent);
        public Post? ReplyToPost(long originalPostId, string content, RegularUser replyGuy, PostFile[] postFiles);
        public Post[] GetPostsFrom(RegularUser user, bool includeReplies = false);
        public Post? GetOriginalPostOf(Post reply);
    }

    public sealed class PostService(ApiContext context) : IPostService
    {
        private readonly ApiContext context = context;
        public bool AddPost(Post post)
        {
            this.context.Posts.Add(post);
            this.context.SaveChanges();
            return true;
        }

        public Post? CreateNewPost(string content, RegularUser creator, PostFile[] postFiles)
        {
            var post = new Post(
                creator,
                [],
                DateTime.Now,
                [.. postFiles],
                content,
                [],
                false
            );
            if(!this.AddPost(post)) return null;
            return post;
        }

        public Post? ReplyToPost(long originalPostId, string content, RegularUser replyGuy, PostFile[] postFiles)
        {
            var ogPost = this.GetPostById(originalPostId);
            if(ogPost is null) return null;
            var reply = new Post(
                replyGuy,
                [],
                DateTime.Now,
                [.. postFiles],
                content,
                [],
                true
            );
            ogPost.Replies.Add(reply);
            this.context.SaveChanges();
            return reply;
        }

        public Post[] GetAllPosts()
            => [.. this.context.Posts];

        public Post? GetPostById(long id)
            => this.context.Posts.FirstOrDefault(post => post.Id == id);

        public bool RemovePost(long id)
        {
            Post? post = this.GetPostById(id);
            if(post == null) return false;
            if(post.IsReply)
            {
                var originalPost = this.context.Posts.FirstOrDefault( a => a.Replies.Any(reply => reply == post));
                originalPost?.Replies.Remove(post);
            }
            this.context.Posts.Remove(post);
            this.context.SaveChanges();
            return true;
        }

        public UpdateResult UpdatePost(long id, Post postContent)
        {
            //Check if user exists
            Post? postInDb = this.GetPostById(id);
            if(postInDb is null) return UpdateResult.NotFound;

            //Save new data
            postInDb.Update(postContent);
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }

        public Post[] GetPostsFrom(RegularUser user, bool includeReplies = false)
        {
            if(includeReplies)
                return [.. this.context.Posts.Where( x => x.PostedBy == user)];
            return [.. this.context.Posts.Where( x => x.PostedBy == user && !x.IsReply )];
        }

        public Post? GetOriginalPostOf(Post reply)
        {
            return this.context.Posts.FirstOrDefault( post => post.Replies.Any(rep => rep == reply) );
        }
    }
}