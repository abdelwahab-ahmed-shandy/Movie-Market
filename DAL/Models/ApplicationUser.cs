using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // User-specific properties
        public DateTime? PasswordChangedDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Address { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? ProfileImage { get; set; }
        public string? BlockReason { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsBlocked { get; set; } = false;
        public AccountStateType AccountStateType { get; set; }
        public UserType UserType = UserType.User;

        // Navigation properties
        public virtual ICollection<AuditRecord> AuditRecords { get; set; } = new List<AuditRecord>();
    }
}
