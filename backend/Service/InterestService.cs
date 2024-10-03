using System;
using System.Collections.Generic;
using System.Linq;
using BackendApp.Data;
using BackendApp.Model;
using BackendApp.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackendApp.Service
{
    public interface IInterestService
    {
        UpdateResult DeclareInterestForPost(uint userId, uint postId);
        UpdateResult RemoveInterestForPost(uint userId, uint postId);
        UpdateResult DeclareInterestForJob(uint userId, uint jobId);
        UpdateResult RemoveInterestForJob(uint userId, uint jobId);
        Post[] GetPostsUserIsInterestedIn(RegularUser user);
        JobPost[] GetJobsUserIsInterestedIn(RegularUser user);
        Post[] GetPostsUserHasCommentedOn(RegularUser user);
    }
    public class InterestService
    (ApiContext dbContext, IPostService postService, IJobService jobService, IRegularUserService userService) 
    : IInterestService
    {
        private readonly ApiContext dbContext = dbContext;
        private readonly IPostService postService = postService;
        private readonly IJobService jobService = jobService;
        private readonly IRegularUserService userService = userService;


        private UpdateResult DeclareInterestFor<T>
        (DbSet<T> dbSet, uint userId, T postBase) 
        where T : PostBase
        {
            RegularUser? userInDb = this.userService.GetUserById(userId);
            if(userInDb is null) return UpdateResult.NotFound;

            //Save new data
            postBase.InterestedUsers.Add(userInDb);
            this.dbContext.SaveChanges();
            return UpdateResult.Ok;
        }
        private UpdateResult RemoveInterestFor<T>
        (DbSet<T> dbSet, uint userId, PostBase postBase) 
        where T : PostBase
        {
            RegularUser? userInDb = this.userService.GetUserById(userId);
            if(userInDb is null) return UpdateResult.NotFound;

            //Save new data
            postBase.InterestedUsers = postBase.InterestedUsers.Where(user => user.Id != userId).ToList();
            this.dbContext.SaveChanges();
            return UpdateResult.Ok;
        }

        public UpdateResult DeclareInterestForJob(uint userId, uint jobId)
        {
            var jobInDb = this.jobService.GetJobById(jobId);
            if(jobInDb is null) return UpdateResult.NotFound;
            return this.DeclareInterestFor(this.dbContext.JobPosts, userId, jobInDb);
        }
        public UpdateResult RemoveInterestForJob(uint userId, uint jobId)
        {
            var jobInDb = this.jobService.GetJobById(jobId);
            if(jobInDb is null) return UpdateResult.NotFound;
            return RemoveInterestFor(this.dbContext.JobPosts, userId, jobInDb);
        }
        public UpdateResult DeclareInterestForPost(uint userId, uint jobId)
        {
            var postInDb = this.postService.GetPostById(jobId);
            if(postInDb is null) return UpdateResult.NotFound;
            return this.DeclareInterestFor(this.dbContext.Posts, userId, postInDb);
        }
        public UpdateResult RemoveInterestForPost(uint userId, uint jobId)
        {
            var postInDb = this.postService.GetPostById(jobId);
            if(postInDb is null) return UpdateResult.NotFound;
            return this.RemoveInterestFor(this.dbContext.Posts, userId, postInDb);
        }

        public Post[] GetPostsUserIsInterestedIn(RegularUser user)
        {
            var query = this.dbContext.Posts
                .Include( post => post.InterestedUsers )
                .Where( post => post.InterestedUsers.Any(x => x == user) );
            return [.. query];
        }

        public JobPost[] GetJobsUserIsInterestedIn(RegularUser user)
        {
            var query = this.dbContext.JobPosts
                .Include( job => job.InterestedUsers )
                .Where( job => job.InterestedUsers.Any(x => x == user) );
            return [.. query];
        }

        public Post[] GetPostsUserHasCommentedOn(RegularUser user)
        {
            var query = this.dbContext.Posts
                .Include( x => x.Replies)
                .ThenInclude( x => x.PostedBy )
                .Where( 
                    post => 
                        post.Replies
                            .Select(reply => reply.PostedBy)
                            .Distinct()
                            .Any(x => x == user)
                );
            return [.. query];
        }
    } 
}