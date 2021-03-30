using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnTrac.Integration.ComponentHost.WebApi.Classes
{
    public class ServiceProperties
    {
        [Required]
        public IDictionary<string, string> ComponentProperties { get; set; }
        
        public IDictionary<string, string> MessagePublisherProperties { get; set; }
    }
}
