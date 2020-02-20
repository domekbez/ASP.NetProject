using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CVEditor.Models
{   
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string NameId { get; set; }

        public string CVUrl { get; set; }
    }
}
