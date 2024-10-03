using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
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

    [Route("api/[Controller]")]
    [ApiController]
    [XmlConverterFilter]
    public class PostController
    (
        IPostService postService, 
        IInterestService interestService, 
        IRegularUserService userService, 
        INotificationService notificationService,
        ITimelineService timelineService,
        IRecommendationService recommendationService
    ) 
    : ControllerBase
    {
        private readonly IPostService postService = postService;
        private readonly IInterestService interestService = interestService;
        private readonly IRegularUserService userService = userService;
        private readonly INotificationService notificationService = notificationService;
        private readonly ITimelineService timelineService = timelineService;
        private readonly IRecommendationService recommendationService = recommendationService;

        [Route("create/{userId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType<Post>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult CreatePost(long userId, PostCreationRequest request)
        {
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            var resultPost = this.postService.CreateNewPost(request.Content, user, request.PostFiles); 
            return resultPost is not null ? this.Ok(resultPost) : this.Conflict();
        }

        [Route("reply/{postId}/{userId}")]
        [HttpPost]
        [Authorize( HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType<Post>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ReplyToPost(long postId, long userId, PostCreationRequest request)
        {
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            var resultPost = this.postService.ReplyToPost(postId, request.Content, user, request.PostFiles);
            if(resultPost is not null){
                Post? post = this.postService.GetPostById(postId);
                if(post is not null)
                {
                    this.notificationService.SendNotificationTo(post.PostedBy, $"User {user.FullName} has replied to your post!", resultPost);
                } 
            }
            return resultPost is not null ? this.Ok(resultPost) : this.NotFound("Original Post not found.");
        }
        
        [Route("{id}")]
        [HttpDelete]
        [Authorize( CreatedPostPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long id)
            => this.postService.RemovePost(id) ? this.Ok() : this.NotFound();

        [HttpGet]
        [Authorize]
        [ProducesResponseType<Post[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll()
            => this.Ok(this.postService.GetAllPosts());

        [Route("{id}")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType<Post>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(long id)
        {
            var user = this.postService.GetPostById(id);
            return user is not null ? this.Ok(user) : this.NotFound();
        }

        [Route("{postId}/interest/set/{userId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeclareInterest(uint userId, uint postId)
        {   
            var user = this.userService.GetUserById(userId);
            var post = this.postService.GetPostById(postId);
            if(user is null || post is null) return this.NotFound("User not found.");
            
            var result = this.interestService.DeclareInterestForPost(userId, postId);
            if(result == UpdateResult.Ok){
                this.notificationService.SendNotificationTo(post.PostedBy, $"{user.Name} was interested in your post!", post);
            }
            return result.ToResultObject(this);
        }

        [Route("{postId}/interest/unset/{userId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RemoveInterest(uint userId, uint postId)
        {
            return this.interestService.RemoveInterestForPost(userId, postId).ToResultObject(this);
        }

        [Route("timeline/following/{userId}")]
        [HttpGet]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType<Post[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetFollowingTimelineForUser(uint userId, int skip = 0, int take = 10)
        {
            if(skip < 0 || take < 0) 
                return this.BadRequest("Skip and take parameters must be positive integer values.");
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(this.timelineService.GetPostTimelineForUser(user, skip, take));
        }

        [Route("timeline/suggested/{userId}")]
        [HttpGet]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType<Post[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetSuggestedTimelineForUser(uint userId, int skip = 0, int take = 10)
        {
            if(skip < 0 || take < 0) 
                return this.BadRequest("Skip and take parameters must be positive integer values.");
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(this.recommendationService.RecommendPosts(user, skip, take));
        }

        [Route("from/{userId}")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType<Post[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetPostsCreatedBy(long userId, bool includeReplies = false)
        {
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(this.postService.GetPostsFrom(user, includeReplies));
        }      


        [HttpGet("interested/{userId}")]
        [Authorize]
        [ProducesResponseType<Post[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetArticlesUserIsInterestedIn(long userId)
        {   
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(this.interestService.GetPostsUserIsInterestedIn(user));
        }

        [HttpGet("commented/{userId}")]
        [Authorize]
        [ProducesResponseType<Post[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetArticlesUserHasCommentedOn(long userId)
        {   
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(this.interestService.GetPostsUserHasCommentedOn(user));
        }

        [HttpGet("original/{replyId}")]
        [Authorize]
        [ProducesResponseType<Post>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOriginalPostOf(long replyId)
        {   
            var reply = this.postService.GetPostById(replyId);
            if(reply is null) return this.NotFound("Reply not found.");
            return this.Ok(this.postService.GetOriginalPostOf(reply));
        }

    }
}