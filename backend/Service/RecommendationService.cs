using System.Collections;
using System.Security.Cryptography.X509Certificates;
using BackendApp.Model;
using BackendApp.Util;
using Util;

namespace BackendApp.Service;



public interface IRecommendationService
{
    public JobPost[] RecommendJobs(RegularUser user, int skip, int take);
    public Post[] RecommendPosts(RegularUser user, int skip, int take);
}

public sealed class RecommendationService
(
    IRegularUserService regularUserService,
    IConnectionService connectionService,
    IJobService jobService,
    IPostService postService
)
: IRecommendationService
{
    private readonly IRegularUserService regularUserService = regularUserService;
    private readonly IConnectionService connectionService = connectionService;
    private readonly IJobService jobService = jobService;
    private readonly IPostService postService = postService;

    public JobPost[] RecommendJobs(RegularUser user, int skip, int top)
    {
        var users = this.regularUserService.GetAllUsers();
        var jobs = this.jobService.GetAllJobs();
        var userIndex = users.ToList().IndexOf(user);
        if(userIndex == -1) return [];
        
        double[,] dataMatrix = this.CreateInitialMatrixForJobs(users, jobs);
        double[,] matrixApproximation = this.MatrixFactorization(dataMatrix, 1000, 0.005, maxIterations: 1000);
        double[] jobRow = MatrixOperations.GetRow(matrixApproximation, userIndex);

        JobPost[] jobsSelected = jobRow
            .Select( (jobRating, jobIndex) => (jobRating, job: jobs[jobIndex]))
            .Where( pair => !pair.job.InterestedUsers.Contains(user) )
            .Select( pair => (jobRating: pair.jobRating + this.RatingOfSimilarSkillset(user, pair.job), pair.job))
            .OrderByDescending( x => x.jobRating )
            .Select( x => x.job )
            .Skip(skip)
            .Take(top)
            .ToArray();
        
        return jobsSelected;
    }

    public Post[] RecommendPosts(RegularUser user, int skip, int top)
    {
        var users = this.regularUserService.GetAllUsers();
        var posts = this.postService.GetAllPosts().Where( post => !post.IsReply ).ToArray();
        var userIndex = users.ToList().IndexOf(user);
        if(userIndex == -1) return [];
        
        double[,] dataMatrix = this.CreateInitialMatrixForPosts(users, posts);
        double[,] matrixApproximation = this.MatrixFactorization(dataMatrix, 1000, 0.005, maxIterations: 1);
        double[] postRow = MatrixOperations.GetRow(matrixApproximation, userIndex);

        Post[] jobsSelected = postRow
            .Select( (jobRating, jobIndex) => (jobRating, job: posts[jobIndex]))
            .Where( pair => !pair.job.InterestedUsers.Contains(user) )
            .OrderByDescending( x => x.jobRating )
            .Select( x => x.job )
            .Skip(skip)
            .Take(top)
            .ToArray();
        
        return jobsSelected;
    }

    public double[,] MatrixFactorization
    (
        double[,] dataMatrix, 
        uint latentFeatures,
        double learningRate,
        bool normalize = false,
        ulong maxIterations = 0
    )
    {
        if(normalize) throw new NotImplementedException();
        uint numberOfUsers = (uint)dataMatrix.GetLongLength(0);
        uint numberOfProducts = (uint)dataMatrix.GetLongLength(1);
        var initialMatrixMin = MatrixOperations.MatrixMin(dataMatrix); 
        var initialMatrixMax = MatrixOperations.MatrixMax(dataMatrix);
        File.AppendAllLines("./log.txt", [$"Min Value is: {initialMatrixMin}"]);
        File.AppendAllLines("./log.txt", [$"Max Value is: {initialMatrixMax}"]);
        double[,] V = MatrixOperations.RandomDoubleMatrix(
            numberOfUsers, 
            latentFeatures, 
            initialMatrixMin, 
            initialMatrixMax
        );
        double[,] F = MatrixOperations.RandomDoubleMatrix(
            latentFeatures, 
            numberOfProducts, 
            initialMatrixMin, 
            initialMatrixMax
        );
        
        bool keepGoing = true;
        double? error = null;
        ulong iterations = 0;
        while(keepGoing)
        {
            double[,] apprMatrix = MatrixOperations.Multiply(V, F);
            var errorMatrix = MatrixOperations.Subtract(dataMatrix, apprMatrix);
            var newV = new double[numberOfUsers, latentFeatures];
            var newF = new double[latentFeatures, numberOfProducts];
            for(var (i, j)  = (0, 0); i < numberOfUsers && j < numberOfProducts; i++, j++){
                for(var k = 0; k < latentFeatures; k++){
                    newV[i, k] = V[i,k] + learningRate * 2 * errorMatrix[i,j] * F[k,j];
                    newF[k, j] = F[k,j] + learningRate * 2 * errorMatrix[i,j] * V[i,k];
                }
            }
            V = newV;
            F = newF;
            var newError = errorMatrix.Cast<double>().Select(a => a * a).Sum();
            iterations++;
            // if(error == newError || iterations == maxIterations) keepGoing = false;
            if( iterations == maxIterations) keepGoing = false;
            error = newError;
        }
        double[,] finalApproximation = MatrixOperations.Multiply(V,F);
        return finalApproximation; 
    }

    private double[,] CreateInitialMatrixForJobs(IEnumerable<RegularUser> users, IEnumerable<JobPost> jobd)
    {
        double[,] matrix = new double[users.Count(), jobd.Count()];
        foreach(var (user, userIndex) in users.Select( (user, index) => (user,index) ))
        {
            foreach(var (post, postIndex) in jobd.Select( (post, index) => (post,index) ))
            {
                matrix[userIndex, postIndex] = this.DetermineMatrixCellValueForJob(user, post);
            }
        }
        return matrix;
    }

    private double[,] CreateInitialMatrixForPosts(IEnumerable<RegularUser> users, IEnumerable<Post> posts)
    {
        double[,] matrix = new double[users.Count(), posts.Count()];
        foreach(var (user, userIndex) in users.Select( (user, index) => (user,index) ))
        {
            foreach(var (post, postIndex) in posts.Select( (post, index) => (post,index) ))
            {
                matrix[userIndex, postIndex] = this.DetermineMatrixCellValueForPost(user, post);
            }
        }
        return matrix;
    }

    private double DetermineMatrixCellValueForJob(RegularUser user, JobPost post)
    {
        double value = 1;
        if(post.InterestedUsers.Contains(user)) value += 15;
        
        var connectedUsers = this.connectionService.GetUsersConnectedTo(user);
        foreach(var connectedUser in connectedUsers)
        {
            if(post.InterestedUsers.Contains(connectedUser)) value += 1;
        }
        return value;
    }

    private double DetermineMatrixCellValueForPost(RegularUser user, Post post)
    {
        double value = 1;
        if(post.InterestedUsers.Contains(user)) value += 5;
        if(post.Replies.SelectMany( reply => reply.InterestedUsers).Contains(user)) value += 10;
        var connectedUsers = this.connectionService.GetUsersConnectedTo(user);
        foreach(var connectedUser in connectedUsers)
        {
            if(post.InterestedUsers.Contains(connectedUser)) value += 1;
            if(post.PostedBy == connectedUser) value += 20;
        }
        return value;
    }


    private double RatingOfSimilarSkillset(RegularUser user, JobPost job)
    {
        double rating = 0;
        foreach(string requirement in job.Requirements)
        {
            foreach(string skill in user.HideableInfo.Capabilities)
            {
                if(skill == requirement) rating += 20;
            }
        }
        return rating;
    }

}