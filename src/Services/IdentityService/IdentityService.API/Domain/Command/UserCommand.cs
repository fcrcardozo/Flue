using System;

namespace IdentityService.Domain.Command
{
    public class UserCommand
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}