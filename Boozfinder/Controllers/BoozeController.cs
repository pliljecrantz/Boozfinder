using System;
using System.Net;
using System.Threading.Tasks;
using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        public BoozeController(IBoozeProvider boozeProvider, ICacheProvider cacheProvider, IUserProvider userProvider)
        {
            _boozeProvider = boozeProvider;
            _cacheProvider = cacheProvider;
            _userProvider = userProvider;
        }

        // GET api/v1/booze
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var items = await _boozeProvider.GetAsync();
                return Ok(items);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
            }   
        }

        // GET api/v1/booze/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            try
            {
                var item = await _boozeProvider.GetAsync(id);
                return Ok(item);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
            }
        }

        //// POST api/v1/booze?token={token}
        //[HttpPost]
        //public async Task<IActionResult> PostAsync(string token, Enums.Task task, [FromBody] Booze item)
        //{
        //    var cachedAuth = _cacheProvider.Get($"__Token_{token}").Split(":");
        //    var cachedToken = cachedAuth[0];
        //    var cachedEmail = cachedAuth[1];
        //    var isAdmin = await _userProvider.HasAdminRoleAsync(cachedEmail);
        //    if (!string.IsNullOrWhiteSpace(cachedToken) && cachedToken.Equals(token))
        //    {
        //        try
        //        {
        //            switch (task)
        //            {
        //                case Enums.Task.Create:
        //                    item.Id = Guid.NewGuid().ToString();
        //                    await _boozeProvider.SaveAsync(item);
        //                    break;
        //                case Enums.Task.Update:
        //                    // Check that token belongs to a user with admin rights or creator of the item
        //                    if (isAdmin || item.Creator.Equals(cachedEmail))
        //                    {
        //                        await _boozeProvider.UpdateAsync(item);
        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }
        //            return Ok(new ItemResponse { Successful = true, Message = "Item saved." });
        //        }
        //        catch
        //        {
        //            return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
        //        }
        //    }
        //    else
        //    {
        //        return Unauthorized(new ItemResponse { Successful = false, Message = "Not authorized or token has expired." });
        //    }
        //}

        // POST api/v1/booze?token={token}
        
        [HttpPost]
        public async Task<IActionResult> PostAsync(string token, [FromBody] Booze item)
        {
            var cachedToken = _cacheProvider.Get($"__Token_{token}").Split(":")[0];
            if (!string.IsNullOrWhiteSpace(cachedToken) && cachedToken.Equals(token))
            {
                try
                {
                    item.Id = Guid.NewGuid().ToString();
                    await _boozeProvider.SaveAsync(item);
                    return Ok(new ItemResponse { Successful = true, Message = "Item saved." });
                }
                catch
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
                }
            }
            else
            {
                return Unauthorized(new ItemResponse { Successful = false, Message = "Not authorized or token has expired." });
            }
        }

        // POST api/v1/booze/delete?token={token}
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteAsync(string token, [FromBody] Booze item)
        {
            var cachedAuth = _cacheProvider.Get($"__Token_{token}").Split(":");
            var isAdmin = await _userProvider.HasAdminRoleAsync(cachedAuth[1]);
            if (!string.IsNullOrWhiteSpace(cachedAuth[0]) && cachedAuth[0].Equals(token) && isAdmin)
            {
                try
                {
                    await _boozeProvider.DeleteAsync(item);
                    return Ok(new ItemResponse { Successful = true, Message = "Item deleted." });
                }
                catch (Exception)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
                }
            }
            else
            {
                return Unauthorized(new ItemResponse { Successful = false, Message = "Not authorized or token has expired." });
            }
        }

        // GET api/v1/booze/search?text={text}(&type={type})
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchAsync(string text, string type)
        {
            try
            {
                var items = await _boozeProvider.SearchAsync(text, type);
                return Ok(items);
            }
            catch
            {
                return BadRequest(new ItemResponse { Successful = false, Message = "Server error." });
            }
        }
    }
}