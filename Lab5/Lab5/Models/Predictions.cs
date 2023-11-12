using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models
{
    public enum Question
    {
        Earth,
        Computer
    }

    public class Prediction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PredictionId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [Url]
        public string Url { get; set; }

        [Required]
        public Question Question { get; set; }
    }
}
