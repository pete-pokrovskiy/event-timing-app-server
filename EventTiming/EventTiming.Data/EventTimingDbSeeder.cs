using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Data
{
    public class EventTimingDbSeeder : IEventTimingDbSeeder
    {
        public const string SeedUserEmail = "89169695738@mail.ru";
        public const string SeedUserName = "89169695738@mail.ru";
        public const string SeedUserPassword = "gtnz1990";

        private readonly EventTimingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public EventTimingDbSeeder(EventTimingDbContext context, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }
        public async Task Seed()
        {
            //TODO: переделать на async
            _context.Database.EnsureCreated();

            if (_context.Users.Any())
                return;

          var result =  _userManager.CreateAsync(new IdentityUser
            {
                Email = _config["Seeding:SeedUserEmail"],
                UserName = _config["Seeding:SeedUserName"]
            }, _config["Seeding:SeedUserPassword"]).Result;

            if(!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                result.Errors.ToList().ForEach(e => sb.AppendLine($"{e.Code}: {e.Description}"));
                throw new Exception(sb.ToString());
            }
           
;        }
    }
}
