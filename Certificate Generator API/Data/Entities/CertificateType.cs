using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CertificateGeneratorAPI.Data
{
    public class CertificateType
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int ValidityMonths { get; set; }

        [JsonIgnore]
        public virtual ICollection<Certificate> Certificates { get; set; }
    }
}
