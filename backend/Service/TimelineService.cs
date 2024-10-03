using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackendApp.Model;
using BackendApp.Data;
using BackendApp.Model.Requests;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using BackendApp.Model.Enums;
using BackendApp.auth;
using Utilities;
using Util;

namespace BackendApp.Service
{
    public interface ITimelineService{
        Post[] GetPostTimelineForUser(RegularUser user, int skip, int take);
        Post[] GetPostTimelineForUser(RegularUser user);
    }
    public class TimelineService(
        ApiContext context,
        IRecommendationService recommendationService
    ) : ITimelineService
    {  
        private readonly ApiContext context = context;
        private readonly IRecommendationService recommendationService = recommendationService;


        public Post[] GetPostTimelineForUser(RegularUser user, int skip, int take)
        {
            var result = context.Connections
                .Where( 
                    con =>
                        con.Accepted && 
                        (con.SentBy == user || con.SentTo == user) 
                )
                .SelectMany( 
                    con => this.context.Posts
                        .Where( 
                            post => (post.PostedBy == con.SentBy && con.SentBy != user) 
                                || (post.PostedBy == con.SentTo && con.SentTo != user) )
                )
                .OrderByDescending(x => x.PostedAt)
                .Skip(skip)
                .Take(take)
                .ToArray();
            return result;
        }

        public Post[] GetPostTimelineForUser(RegularUser user)
        {
            return context.Connections
                .Where( 
                    con =>
                        con.Accepted && 
                        (con.SentBy == user || con.SentTo == user) 
                )
                .SelectMany( 
                    con => this.context.Posts
                        .Where( 
                            post => (post.PostedBy == con.SentBy && con.SentBy != user) 
                                || (post.PostedBy == con.SentTo && con.SentTo != user) )
                )
                .OrderByDescending(x => x.PostedAt)
                .ToArray();
        }
    }
}




