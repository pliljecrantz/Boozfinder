using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Boozfinder.Helpers;
using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Boozfinder.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BoozeController : Controller
    {
        private readonly IBoozeProvider _boozeProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly IUserProvider _userProvider;
        private readonly ILogger _logger;

        public BoozeController(IBoozeProvider boozeProvider, ICacheProvider cacheProvider, IUserProvider userProvider, ILogger logger)
        {
            _boozeProvider = boozeProvider;
            _cacheProvider = cacheProvider;
            _userProvider = userProvider;
            _logger = logger;
        }

        // api/v1/booze
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var items = await _boozeProvider.GetAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in BoozeController method GetAsync - Stack Trace: [{ex.StackTrace}] - Message: [{ex.Message}] - Date: [{DateTime.Now}]");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
            }
        }

        // api/v1/booze/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            try
            {
                var item = await _boozeProvider.GetAsync(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in BoozeController method GetAsync(string id) - Stack Trace: [{ex.StackTrace}] - Message: [{ex.Message}] - Date: [{DateTime.Now}]");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
            }
        }

        // api/v1/booze?token={token}&task={task}
        [HttpPost]
        public async Task<IActionResult> PostAsync(string token, string task, [FromBody] Booze item)
        {
            var cachedAuth = _cacheProvider.Get($"__Token_{token}").Split(":");
            var cachedToken = cachedAuth[0];
            var cachedEmail = cachedAuth[1];
            var isAdmin = await _userProvider.HasAdminRoleAsync(cachedEmail);
            ItemResponse itemResponse = null;

            if (!string.IsNullOrWhiteSpace(cachedToken) && cachedToken.Equals(token))
            {
                try
                {
                    Enum.TryParse(task, out Enums.Task typeOfTask);
                    switch (typeOfTask)
                    {
                        case Enums.Task.Create:
                            item.Id = Guid.NewGuid().ToString();
                            await _boozeProvider.CreateAsync(item);
                            itemResponse = new ItemResponse { Successful = true, Message = "Item created." };
                            break;
                        case Enums.Task.Update:
                            // Check that token belongs to a user with admin rights or creator of the item
                            if (isAdmin || item.Creator.Equals(cachedEmail))
                            {
                                await _boozeProvider.UpdateAsync(item);
                                itemResponse = new ItemResponse { Successful = true, Message = "Item updated." };
                            }
                            else
                            {
                                _logger.LogInformation($"Not authorized to update this item - User: [{cachedEmail}] - Date: [{DateTime.Now}]");
                                return Unauthorized(new ItemResponse { Successful = false, Message = "Not authorized to update this item." });
                            }
                            break;
                        default:
                            break;
                    }
                    return Ok(itemResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in BoozeController method PostAsync - Stack Trace: [{ex.StackTrace}] - Message: [{ex.Message}] - Date: [{DateTime.Now}]");
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
                }
            }
            else
            {
                _logger.LogInformation($"Not authorized or token has expired - User: [{cachedEmail}] - Date: [{DateTime.Now}]");
                return Unauthorized(new ItemResponse { Successful = false, Message = "Not authorized or token has expired." });
            }
        }

        // api/v1/booze/delete?token={token}
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteAsync(string token, [FromBody] Booze item)
        {
            var cachedAuth = _cacheProvider.Get($"__Token_{token}").Split(":");
            var cachedToken = cachedAuth[0];
            var cachedEmail = cachedAuth[1];
            var isAdmin = await _userProvider.HasAdminRoleAsync(cachedEmail);

            if (!string.IsNullOrWhiteSpace(cachedToken) && cachedToken.Equals(token) && isAdmin)
            {
                try
                {
                    await _boozeProvider.DeleteAsync(item);
                    return Ok(new ItemResponse { Successful = true, Message = "Item deleted." });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in BoozeController method DeleteAsync - Stack Trace: [{ex.StackTrace}] - Message: [{ex.Message}] - Date: [{DateTime.Now}]");
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
                }
            }
            else
            {
                _logger.LogInformation($"Not authorized or token has expired - User: [{cachedEmail}] - Date: [{DateTime.Now}]");
                return Unauthorized(new ItemResponse { Successful = false, Message = "Not authorized or token has expired." });
            }
        }

        // api/v1/booze/search?text={text}(&type={type})
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchAsync(string text, string type)
        {
            try
            {
                var items = await _boozeProvider.SearchAsync(text, type);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in BoozeController method SearchAsync - Stack Trace: [{ex.StackTrace}] - Message: [{ex.Message}] - Date: [{DateTime.Now}]");
                return BadRequest(new ItemResponse { Successful = false, Message = "Server error." });
            }
        }
    }
}