using Boozfinder.Helpers;
using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Boozfinder.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserProvider _userProvider;
        private readonly ICacheProvider _cacheProvider;

        public AuthenticationController(IUserProvider userProvider, ICacheProvider cacheProvider)
        {
            _userProvider = userProvider;
            _cacheProvider = cacheProvider;
        }

        // api/v1/authentication
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            AuthenticationResponse response;
            try
            {
                var authenticated = await _userProvider.AuthenticateAsync(user.Email, user.Password);
                if (authenticated)
                {
                    response = new AuthenticationResponse
                    {
                        Authenticated = true,
                        Expires = DateTime.Now.AddMinutes(30).ToString(),
                        Message = "Authentication succeeded.",
                        Token = TokenGenerator.Token()
                    };
                    _cacheProvider.Set(response.Token, user.Email);
                    return Ok(response);
                }
                else
                {
                    response = new AuthenticationResponse
                    {
                        Authenticated = false,
                        Message = "Authentication failed. Possible causes: e-mail address does not exist in system or wrong password."
                    };
                    return Unauthorized(response);
                }
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
            }
        }
    }
}
