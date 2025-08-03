using Microsoft.AspNetCore.Identity;
using System;

namespace Task4.Models
{
    public class User : IdentityUser
    {
        public DateTime? LastLogin { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool IsBlocked { get; set; } = false;
    }
}