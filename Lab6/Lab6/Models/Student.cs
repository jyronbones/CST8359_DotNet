using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Lab6.Models
{
    public class Student
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        public Guid ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Program { get; set; }
    }
}
