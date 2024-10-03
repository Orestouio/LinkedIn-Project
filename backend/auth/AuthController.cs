using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackendApp.Auth;
using BackendApp.Model;
using BackendApp.Model.Requests;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;

namespace BackendApp.auth
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController
    (IAuthenticationService authenticationService, IRegularUserService userService) 
    : ControllerBase
    {
        
        private readonly IAuthenticationService authenticationService = authenticationService;
        private readonly IRegularUserService userService = userService;
        private readonly TimeSpan tokenLifeSpan = TimeSpan.FromHours(4);

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Login(TokenGenerationRequest request)
        {   
            var user = this.authenticationService.Authenticate(request);
            if(user != null)
            {
                var token = this.authenticationService.GenerateToken(
                    user,
                    this.tokenLifeSpan
                );
                return Ok(JsonSerializer.Serialize(new TokenResponse(token, user.Id, user.UserRole.ToString())));
            }
            return NotFound("User Not Found");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Register(RegisterRequest request)
        {   
            var wasAdded = this.userService.AddUser(new RegularUser(
                request.Email, 
                EncryptionUtility.HashPassword(request.Password),
                request.Name, request.Surname, 
                null,
                new(
                    request.PhoneNumber,
                    request.Location,
                    [],
                    request.CurrentPosition,
                    [],
                    []
                )
            ));
            if(!wasAdded) return this.Conflict("User with a duplicate email exists.");
            return this.Login(new(){Email = request.Email, Password = request.Password});
        }
    }
}