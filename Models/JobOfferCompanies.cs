using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVEditor.Models
{
    public class JobOfferCompanies:JobOffer
    {
        public IEnumerable<Company> Companies { get; set; }
    }
}
