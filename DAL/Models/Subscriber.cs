namespace MovieMart.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public DateTime SubscribedAt { get; set; } = DateTime.Now;
    }
}
