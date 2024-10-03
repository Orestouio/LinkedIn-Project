using Microsoft.EntityFrameworkCore;
using BackendApp.Data;
using BackendApp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackendApp.auth;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using BackendApp.auth.Filters;
using BackendApp.Auth;
using ImageManipulation.Data.Services;
using Util.DataFeeding;
using Microsoft.Net.Http.Headers;

var corsPolicyName = "_myAllowAllOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiContext>(
    opt => opt
        .UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")), 
    contextLifetime: ServiceLifetime.Scoped
);

//Add model services
builder.Services.AddScoped<IRegularUserService, RegularUserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IInterestService, InterestService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<ITimelineService, TimelineService>();

//Add other general use case services
builder.Services.AddHttpContextAccessor();
// builder.Services.AddMvc(
//     options =>  {
//         options.FormatterMappings.SetMediaTypeMappingForFormat
//                 ("xml", MediaTypeHeaderValue.Parse("application/xml"));
//         options.FormatterMappings.SetMediaTypeMappingForFormat
//             ("json", MediaTypeHeaderValue.Parse("application/json"));
// })
// .AddXmlSerializerFormatters();

// Controllers
builder.Services.AddControllers();

// Add API explorer page
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( setup =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});

// CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: corsPolicyName,
        policy  =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); //TODO: Add only desired origins
        }
    );
});

// Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer( 
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] 
                        ?? throw new Exception("Key has not been set up.")))
                };
        }
    );

// Add Authorization
builder.Services.AddScoped<IAuthorizationHandler, HasIdHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IsAdminHandler>();
builder.Services.AddScoped<IAuthorizationHandler, HasNotificationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SentMessageHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SentConnectionRequestHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ReceivedConnectionRequestHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedJobHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedPostHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IsMemberOfConversationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IsMemberOfConnectionHandler>();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.PolicyNames.HasIdEqualToUserIdParamPolicyName, policy =>
        policy.Requirements.Add( new HasIdRequirement("userId")))
    .AddPolicy(AuthConstants.PolicyNames.HasIdEqualToSenderIdPolicyName, policy =>
        policy.Requirements.Add( new HasIdRequirement("senderId")))
    .AddPolicy(AuthConstants.PolicyNames.HasIdEqualToIdParamPolicyName, policy =>
        policy.Requirements.Add( new HasIdRequirement("id")))
    .AddPolicy(AuthConstants.PolicyNames.IsAdminPolicyName, policy => 
        policy.Requirements.Add( new IsAdminRequirement()))
    .AddPolicy(AuthConstants.PolicyNames.HasNotificationPolicyName, policy => 
        policy.Requirements.Add( new HasNotificationRequirement("id")))
    .AddPolicy(AuthConstants.PolicyNames.SentMessagePolicyName, policy => 
        policy.Requirements.Add( new SentMessageRequirement("id")))
    .AddPolicy(AuthConstants.PolicyNames.SentConnectionRequestPolicyName, policy => 
        policy.Requirements.Add( new SentConnectionRequestRequirement("id")))
    .AddPolicy(AuthConstants.PolicyNames.ReceivedConnectionRequestPolicyName, policy => 
        policy.Requirements.Add( new ReceivedConnectionRequestRequirement("id")))
    .AddPolicy(AuthConstants.PolicyNames.CreatedJobPolicyName, policy => 
        policy.Requirements.Add( new CreatedJobRequirement("id")))
    .AddPolicy(AuthConstants.PolicyNames.CreatedPostPolicyName, policy => 
        policy.Requirements.Add( new CreatedPostRequirement("id")))
    .AddPolicy(AuthConstants.PolicyNames.IsMemberOfConversationPolicyName, policy => 
        policy.Requirements.Add( new IsMemberOfConversationRequirement("userAId","userBId")))
    .AddPolicy(AuthConstants.PolicyNames.IsMemberOfConnectionPolicyName, policy =>
        policy.Requirements.Add( new IsMemberOfConnectionRequirement("id")));
    

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// IServiceScope scope = app.Services.CreateScope();
// DummyDataFeeder.FillWithDummyData(scope.ServiceProvider.GetService<ApiContext>() ?? throw new Exception("Api Context was not set up correctly."));
// scope.Dispose();
app.Run();



