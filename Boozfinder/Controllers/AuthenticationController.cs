using Boozfinder.Helpers;
using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Boozfinder.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(IAuthenticationProvider authenticationProvider, ICacheProvider cacheProvider)
        {
            AuthenticationProvider = authenticationProvider;
            CacheProvider = cacheProvider;
        }

        public IAuthenticationProvider AuthenticationProvider { get; }
        public ICacheProvider CacheProvider { get; }

        // POST api/v1/authentication
        [HttpPost]
        public IActionResult Authenticate([FromBody] User user)
        {
            AuthenticationResponse response;
            var authenticated = AuthenticationProvider.AuthenticateUser(user.Email, user.Password);
            if (authenticated)
            {
                response = new AuthenticationResponse
                {
                    Authenticated = true,
                    Expires = DateTime.Now.AddMinutes(30).ToString(),
                    Message = "Authentication succeeded.",
                    Token = TokenGenerator.Token()
                };
                CacheProvider.Set(response.Token, user.Email);
                return Ok(response);
            }
            else
            {
                response = new AuthenticationResponse
                {
                    Authenticated = false,
                    Message = "Authentication failed."
                };
                return Unauthorized(response);
            }
        }
    }
}
