namespace MovieMart.Models
{
    public class SentEmail
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
