using System.ComponentModel.DataAnnotations;

namespace Lab4.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        // Foreign key for SportClub
        public string SportClubId { get; set; }

        // Navigation property for SportClub
        public virtual SportClub SportClub { get; set; }
    }
}
