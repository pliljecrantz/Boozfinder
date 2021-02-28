using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Boozfinder.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IBoozeProvider _boozeProvider;
        private readonly ICacheProvider _cacheProvider;

        public ReviewController(IBoozeProvider boozeProvider, ICacheProvider cacheProvider)
        {
            _boozeProvider = boozeProvider;
            _cacheProvider = cacheProvider;
        }

        // api/v1/review/{id}?token={token}
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, string token, [FromBody] Review review)
        {
            var cachedItem = _cacheProvider.Get($"__Token_{token}");
            var cachedEmail = cachedItem.Split(":")[1];
            var cachedToken = cachedItem.Split(":")[0];

            Booze itemToUpdate;
            if (!string.IsNullOrWhiteSpace(cachedToken) && cachedToken.Equals(token))
            {
                try
                {
                    itemToUpdate = await _boozeProvider.GetAsync(id);
                    var reviewsFromCurrentUser = itemToUpdate.Reviews?.Where(x => x.Reviewer.Equals(cachedEmail));

                    if (reviewsFromCurrentUser != null && reviewsFromCurrentUser.Any())
                    {
                        return Ok(new ItemResponse { Successful = false, Message = "User has already given review." });
                    }
                    else
                    {
                        // If no user has given any reviews on the item we need to instansiate the review list first
                        if (reviewsFromCurrentUser == null)
                        {
                            itemToUpdate.Reviews = new List<Review>();
                        }
                        var reviewToInsert = new Review { Reviewer = cachedEmail, Rating = review.Rating, Text = review.Text };
                        itemToUpdate.Reviews.Add(reviewToInsert);
                        await _boozeProvider.UpdateAsync(itemToUpdate);
                        return Ok(new ItemResponse { Successful = true, Message = "Review saved." });
                    }
                }
                catch
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ItemResponse { Successful = false, Message = "A server error occured." });
                }
            }
            else
            {
                return Unauthorized(new AuthenticationResponse { Authenticated = false, Message = "Not authorized or token has expired." });
            }
        }
    }
}
