using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;
using Boozfinder.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Boozfinder.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        public ReviewController(IBoozeProvider boozeProvider, ICacheProvider cacheProvider)
        {
            BoozeProvider = boozeProvider;
            CacheProvider = cacheProvider;
        }

        public IBoozeProvider BoozeProvider { get; }
        public ICacheProvider CacheProvider { get; }

        // PUT api/v1/review/{id}?token={token}
        [HttpPut("{id}")]
        public IActionResult Put(int id, string token, [FromBody] Review review)
        {
            var cachedItem = CacheProvider.Get($"__Token_{token}");
            var cachedEmail = cachedItem.Split(":")[1];
            var cachedToken = cachedItem.Split(":")[0];
            Booze itemToUpdate;
            if (!string.IsNullOrWhiteSpace(cachedToken) && cachedToken.Equals(token))
            {
                itemToUpdate = BoozeProvider.Get(id);
                var userReviews = itemToUpdate.Reviews?.Where(x => x.Email.Equals(cachedEmail));

                if (userReviews != null && userReviews.Any())
                {
                    var response = new ReviewResponse { Successful = false, Message = "User has already given review." };
                    return BadRequest(response);
                }
                else
                {
                    // If no user has given any reviews on the item we need to instansiate the review list first
                    if (userReviews == null)
                    {
                        itemToUpdate.Reviews = new List<Review>();
                    }
                    var reviewToInsert = new Review { Email = cachedEmail, Rating = review.Rating, Text = review.Text };
                    itemToUpdate.Reviews.Add(reviewToInsert);
                    var updatedItem = BoozeProvider.Update(itemToUpdate);
                    updatedItem = null;
                    if (updatedItem != null)
                    {
                        var response = new ReviewResponse { Successful = true, Message = "Review saved." };
                        return Ok(response);
                    }
                    else
                    {
                        var response = new ReviewResponse { Successful = false, Message = "Review could not be saved due to system error." };
                        return StatusCode(500, response);
                    }
                }
            }
            else
            {
                var response = new BoozeResponse { Successful = false, Message = "Not authorized or token has expired." };
                return Unauthorized(response);
            }
        }
    }
}
