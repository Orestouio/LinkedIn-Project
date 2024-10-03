using System;
using System.Collections.Generic;
using System.Linq;
using BackendApp.Controllers.Filters;
using BackendApp.Model;
using BackendApp.Model.Enums;
using BackendApp.Model.Requests;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BackendApp.Auth.AuthConstants.PolicyNames;


namespace BackendApp.Controller
{
    [Route("/api/[Controller]")]
    [ApiController]
    [XmlConverterFilter]
    public class JobController
    (
        IJobService jobService, 
        IInterestService interestService, 
        IRegularUserService regularUserService,
        IRecommendationService recommendationService,
        INotificationService notificationService
    ) 
    : ControllerBase
    {
        private readonly IJobService jobService = jobService;
        private readonly IInterestService interestService = interestService;
        private readonly IRegularUserService userService = regularUserService;
        private readonly IRecommendationService recommendationService = recommendationService;
        private readonly INotificationService notificationService = notificationService;
        
        [Route("{id}")]
        [HttpDelete]
        [Authorize( Policy = CreatedJobPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long id)
            => this.jobService.RemoveJob(id) ? this.Ok() : this.NotFound();

        [HttpGet]
        [Authorize]
        [ProducesResponseType<JobPost[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll()
            => this.Ok(this.jobService.GetAllJobs());

        [Route("{id}")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType<JobPost>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get(long id)
        {
            var job = this.jobService.GetJobById(id);
            return job is not null ? this.Ok(job) : this.NotFound();
        }

        [Route("{jobId}/interest/set/{userId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult DeclareInterest(uint userId, uint jobId)
        {
            var user = this.userService.GetUserById(userId);
            var post = this.jobService.GetJobById(jobId);
            if(user is null || post is null) return this.NotFound("User not found.");
            
            var result = this.interestService.DeclareInterestForJob(userId, jobId);
            if(result == UpdateResult.Ok){
                this.notificationService.SendNotificationTo(post.PostedBy, $"{user.Name} was interested in your post!", post);
            }
            return result.ToResultObject(this);
        }

        [Route("{jobId}/interest/unset/{userId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RemoveInterest(uint userId, uint jobId)
        {
            return this.interestService.RemoveInterestForJob(userId, jobId).ToResultObject(this);
        }

        [HttpPost("by/{userId}")]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType<JobPost>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateJob(long userId, JobCreationRequest request)
        {   
            var creatorOfJob = this.userService.GetUserById(userId);
            if(creatorOfJob is null) return this.NotFound("User not found.");

            var resultingJob = this.jobService
                .CreateNewJobPost(
                    creatorOfJob, 
                    request.Title, 
                    request.Description, 
                    request.PostFiles, 
                    request.Requirements
                );
                
            return resultingJob is not null ? this.Ok(resultingJob) : this.BadRequest("Only up to 4 files per post allowed.");
        }

        [HttpGet("by/{userId}")]
        [Authorize]
        [ProducesResponseType<JobPost[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetJobsPostedBy(long userId)
        {   
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(this.jobService.GetJobPostsBy(user));
        }

        [HttpGet("interested/{userId}")]
        [Authorize]
        [ProducesResponseType<JobPost[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetJobsInterestedIn(long userId)
        {   
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(this.interestService.GetJobsUserIsInterestedIn(user));
        }

        [HttpGet("recommend/{userId}/{skip}/{take}")]
        [Authorize]
        [ProducesResponseType<JobPost[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RecommendJobsTo(long userId, int skip, int take)
        {
            if(skip < 0 || take < 0) 
                return this.BadRequest("Skip and take parameters must be positive integer values.");
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found");
            return this.Ok(this.recommendationService.RecommendJobs(user, skip, take));
        }

    }
}