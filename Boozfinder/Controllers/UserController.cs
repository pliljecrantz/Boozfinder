﻿using Boozfinder.Helpers;
using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Boozfinder.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserProvider _userProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly ILogger _logger;

        public UserController(IUserProvider userProvider, ICacheProvider cacheProvider, ILogger<UserController> logger)
        {
            _userProvider = userProvider;
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        // api/v1/user
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            AuthenticationResponse response;
            try
            {
                user.Id = $"u_{user.Email}";
                var created = await _userProvider.CreateAsync(user);
                if (created)
                {
                    response = new AuthenticationResponse
                    {
                        Authenticated = true,
                        Token = TokenGenerator.Token(),
                        Message = "Account creation succeeded.",
                        Expires = DateTime.Now.AddMinutes(30).ToString()
                    };
                    _cacheProvider.Set(response.Token, user.Email);
                }
                else
                {
                    response = new AuthenticationResponse
                    {
                        Authenticated = false,
                        Message = "Account creation failed. Possible cause: e-mail address already exists in system."
                    };
                    _logger.LogInformation($"Account creation failed - User: [{user.Email}] - Date: [{DateTime.Now}]");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UserController method Post - Stack Trace: [{ex.StackTrace}] - Message: [{ex.Message}] - Date: [{DateTime.Now}]");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
            }
        }
    }
}
