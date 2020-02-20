using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CVEditor.Models
{
    public class JobApplication
    {
        public enum status { accepted, rejected, applied};

        public int Id { get; set; }

        [Required]
        public int OfferId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public status ApplicationStatus{ get; set; }

        public string Comment { get; set; }
    }
}
