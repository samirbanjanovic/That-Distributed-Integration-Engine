using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnTrac.Integration.NodeApi.Classes
{
    public class PackageConfiguration
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Version { get; set; }
        [Required]
        public string ContentRoot { get; set; }
        [Required]
        public string Command { get; set; }

        public string Description { get; set; }
    }
}
