using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Models
{
    public class Destination
    {
        public string FolderName { get; set; } = null!;
        public string SourceFile { get; set; } = null!;
        public string DestinationFile { get; set; } = null!;
    }
}
