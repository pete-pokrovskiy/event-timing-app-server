using EventTiming.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EventTiming.API.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("/api/v1/events")]    
    public class EventsController : Controller
    {
        //http://localhost:54839/api/events?eventid=A89162B9-BF7B-4664-23AB-08D6E4298D7C
        [HttpGet("{eventId}")]
        public async Task<IActionResult> Get(Guid eventId)
        {
            var userIdentity = HttpContext.User.Identity;

            return Ok(new Event
            {
                Id = eventId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Name = "День соц работника",
                Description = "Ежегодное празднование дня"
            });
        }
    }
}
