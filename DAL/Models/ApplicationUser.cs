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
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? ProfileImage { get; set; }
        public string? BlockReason { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsBlocked { get; set; } = false;

        // Navigation properties
        public ICollection<AuditRecord> AuditRecords { get; set; } = new List<AuditRecord>();

        // Base 
        public AccountStateType accountStateType { get; set; }
        public CurrentState? CurrentState { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
        public string? BlockedBy { get; set; }
        public DateTime? BlockedDateUtc { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? RestoredBy { get; set; }
        public DateTime? RestoredDateUtc { get; set; }
        public string? LastAction { get; set; }

    }
}
