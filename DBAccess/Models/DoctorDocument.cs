using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public class DoctorDocument
    {
        public int Id { get; set; }
        public int DocumentType { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
        public Doctor Doctor { get; set; }
        public int Doctor_Id { get; set; }
    }
    
    public enum DoctorDocumentType
    {
        EductionCertificate=1,
        ProfessionalCertificate
    }

}
