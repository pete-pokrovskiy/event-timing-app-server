using EventTiming.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EventTiming.API.Controllers
{
    [Route("/api/events")]
    [Authorize]
    public class EventsController : Controller
    {
        //http://localhost:54839/api/events?eventid=A89162B9-BF7B-4664-23AB-08D6E4298D7C
        [HttpGet]
        public IActionResult Get(Guid eventId)
        {
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
