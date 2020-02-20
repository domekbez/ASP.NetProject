using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CVEditor.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        public string NameId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string EmailAddress { get; set; }
    }
}
