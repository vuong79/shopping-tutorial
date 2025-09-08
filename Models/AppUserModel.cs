using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Tutorial.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; }

        public string RoleId { get; set; }
        [NotMapped]
        public string RoleNames { get; set; }
    }
}
