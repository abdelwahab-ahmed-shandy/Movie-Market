
namespace Models
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }

        public CurrentState? CurrentState { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }

        public string? BlockedBy { get; set; }
        public DateTime? BlockedDateUtc { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        // Soft Delete
        public bool IsDeleted { get; set; } = false;

        public string? RestoredBy { get; set; }
        public DateTime? RestoredDateUtc { get; set; }

        public string? LastAction { get; set; }
    }
}

