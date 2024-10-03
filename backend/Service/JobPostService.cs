using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendApp.Data;
using BackendApp.Model;
using BackendApp.Model.Enums;

namespace BackendApp.Service
{
    public interface IJobService
    {

        public JobPost? GetJobById(long id);
        public JobPost[] GetAllJobs();
        public void AddJob(JobPost post);
        public bool RemoveJob(long id);
        public UpdateResult UpdateJob(long id, JobPost postContent);
        public JobPost? CreateNewJobPost(RegularUser user, string title, string description, PostFile[] postFiles, string[] requirements);
        public UpdateResult AddInterestedUser(long id, RegularUser user);
        public JobPost[] GetJobPostsBy(RegularUser user);
    
    }
    public class JobService(ApiContext context) : IJobService
    {
        private readonly ApiContext context = context;
        
        public void AddJob(JobPost job)
        {
            this.context.JobPosts.Add(job);
            this.context.SaveChanges();
        }

        public JobPost[] GetAllJobs()
            => [.. this.context.JobPosts];

        public JobPost? GetJobById(long id)
            => this.context.JobPosts.FirstOrDefault(post => post.Id == id);

        public bool RemoveJob(long id)
        {
            JobPost? post = this.GetJobById(id);
            if(post == null) return false;

            this.context.JobPosts.Remove(post);
            this.context.SaveChanges();
            return true;
        }

        public UpdateResult UpdateJob(long id, JobPost JobContent)
        {
            //Check if user exists
            JobPost? jobInDb = this.GetJobById(id);
            if(jobInDb is null) return UpdateResult.NotFound;

            //Save new data
            jobInDb.Update(JobContent);
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }

        public UpdateResult AddInterestedUser(long id, RegularUser user)
        {
            JobPost? jobInDb = this.GetJobById(id);
            if(jobInDb is null) return UpdateResult.NotFound;

            //Save new data
            jobInDb.InterestedUsers.Add(user);
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }

        public UpdateResult RemoveInterestedUser(long id, RegularUser user)
        {
            JobPost? jobInDb = this.GetJobById(id);
            if(jobInDb is null) return UpdateResult.NotFound;

            //Save new data
            var interestedUsers = jobInDb.InterestedUsers;
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }

        public JobPost? CreateNewJobPost(RegularUser user, string title, string description, PostFile[] postFiles, string[] requirements)
        {
            if(postFiles.Length > 4) return null;
            var job = new JobPost(user, [], DateTime.Now, postFiles.ToList(), title, description, requirements);
            this.AddJob(job);
            return job;
        }

        public JobPost[] GetJobPostsBy(RegularUser user)
            => this.context.JobPosts
                .Where( jobPost => jobPost.PostedBy == user)
                .ToArray();
    }           
}