using System.ComponentModel.DataAnnotations;

namespace Lab4.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }
    }
}
