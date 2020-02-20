using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CVEditor.Models
{
    public class HR
    {
        public int Id { get; set; }

        [Required]
        public string NameId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(12)]
        public string PhoneNumber { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }
}

