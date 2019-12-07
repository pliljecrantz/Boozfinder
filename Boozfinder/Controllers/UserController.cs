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
    public class UserController : ControllerBase
    {
        public UserController(IUserProvider userProvider, ICacheProvider cacheProvider)
        {
            UserProvider = userProvider;
            CacheProvider = cacheProvider;
        }

        public IUserProvider UserProvider { get; }
        public ICacheProvider CacheProvider { get; }

        // POST api/v1/user
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            AuthenticationResponse response;
            var created = UserProvider.CreateUser(user);
            if (created)
            {
                response = new AuthenticationResponse
                {
                    Authenticated = true,
                    Token = TokenGenerator.Token(),
                    Message = "Account creation succeeded.",
                    Expires = DateTime.Now.AddMinutes(30).ToString()
                };
                CacheProvider.Set(response.Token, user.Email);
                return Ok(response);
            }
            else
            {
                response = new AuthenticationResponse
                {
                    Authenticated = false,
                    Message = "Account creation failed. Likely cause: E-mail address already exists (or system error if error persists)."
                };
                return BadRequest(response);
            }
        }
    }
}
