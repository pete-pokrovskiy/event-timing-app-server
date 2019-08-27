using System;

namespace EventTiming.Logic.Services.Auth
{
    public class UserData
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
