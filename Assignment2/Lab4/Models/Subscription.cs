namespace Lab4.Models
{
    public class Subscription
    {
        public int FanId { get; set; }
        public string SportClubId { get; set; }

        // Navigation properties
        public virtual Fan Fan { get; set; }
        public virtual SportClub SportClub { get; set; }
    }
}