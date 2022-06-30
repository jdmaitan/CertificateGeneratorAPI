using System.Collections.Generic;

namespace CertificateGeneratorAPI.Data
{
    public class Holder
    {
        public int ID { get; set; }
        public string BusinessName { get; set; }
        public string RIF { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; }
    }
}
