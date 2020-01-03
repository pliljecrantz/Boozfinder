using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Boozfinder.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BoozeController : ControllerBase
    {
        public BoozeController(IBoozeProvider boozeProvider, ICacheProvider cacheProvider)
        {
            BoozeProvider = boozeProvider;
            CacheProvider = cacheProvider;
        }

        public IBoozeProvider BoozeProvider { get; }
        public ICacheProvider CacheProvider { get; }

        // GET api/v1/booze
        [HttpGet]
        public IActionResult Get()
        {
            var all = BoozeProvider.Get();
            return Ok(all);
        }

        // GET api/v1/booze/{id}
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var item = BoozeProvider.Get(id);
            return Ok(item);
        }

        // POST api/v1/booze?token={token}
        [HttpPost]
        public IActionResult Create(string token, [FromBody] Booze booze)
        {
            var cachedToken = CacheProvider.Get($"__Token_{token}").Split(":")[0];
            if (!string.IsNullOrWhiteSpace(cachedToken) && cachedToken.Equals(token))
            {
                var item = BoozeProvider.Save(booze);
                if (item != null)
                {
                    var response = new BoozeResponse { Item = item, Successful = true, Message = "Item saved." };
                    return Ok(response);
                }
                else
                {
                    var response = new BoozeResponse { Successful = false, Message = "Item could not be saved." };
                    return StatusCode(500, response);
                }
            }
            else
            {
                var response = new BoozeResponse { Successful = false, Message = "Not authorized or token has expired." };
                return Unauthorized(response);
            }
        }

        // PUT api/v1/booze/{id}?token={token}
        [HttpPut("{id}")]
        public IActionResult Update(int id, string token, [FromBody] Booze booze)
        {
            // TODO: Implement update on item for admin role
            throw new NotImplementedException();
        }

        // DELETE api/v1/booze/{id}?token={token}
        [HttpDelete("{id}")]
        public void Delete(int id, string token)
        {
            // TODO: Implement delete on item for admin role
            throw new NotImplementedException();
        }
    }
}
