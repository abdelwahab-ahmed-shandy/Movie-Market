namespace MovieMart.Models
{
    public class Subscriber : BaseModel
    {
        public string Email { get; set; }
        public DateTime SubscribedAt { get; set; } = DateTime.Now;
    }
}
