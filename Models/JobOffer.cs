using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CVEditor.Models
{
    public class JobOffer
    {
        public int Id { get; set; }

        [Required]
        public int HRId { get; set; }

        [Required]
        public string Title { get; set; }

        public int? SalaryFrom { get; set; }

        public int? SalaryTo { get; set; }  
        
        public DateTime Created { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [MinLength(10)]
        public string Description{ get; set; }

        [DataType(DataType.Date)]
        public DateTime? ValidUntil { get; set; }
    }
}
