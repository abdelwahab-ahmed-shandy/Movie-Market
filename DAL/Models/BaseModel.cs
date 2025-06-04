
namespace Models
{
    public abstract class BaseModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public CurrentState? CurrentState { get; set; } = DAL.Enums.CurrentState.Inactive;

        [MaxLength(100)]
        public string CreatedBy { get; set; } = "System";
        public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }

        [MaxLength(100)]
        public string? BlockedBy { get; set; }
        public DateTime? BlockedDateUtc { get; set; }

        [MaxLength(100)]
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }

        public bool IsDeleted { get; set; } = false;

        [MaxLength(100)]
        public string? RestoredBy { get; set; }
        public DateTime? RestoredDateUtc { get; set; }

        [MaxLength(100)]
        public string? LastAction { get; set; }
    }
}

