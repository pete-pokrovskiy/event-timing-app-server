using EventTiming.Domain.Base;
using System;

namespace EventTiming.Domain
{
    public class User : Entity
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
