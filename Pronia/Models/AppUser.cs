using Microsoft.AspNetCore.Identity;
using Pronia.Enumerations;

namespace Pronia.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
