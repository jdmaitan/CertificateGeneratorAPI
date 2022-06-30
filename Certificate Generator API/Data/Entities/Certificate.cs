using System;
using System.Text.Json.Serialization;

namespace CertificateGeneratorAPI.Data
{
    public class Certificate
    {
        public int ID { get; set; }
        public int HolderID { get; set; }
        public int TypeID { get; set; }
        public DateTime ExpeditionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public Guid GUID { get; set; }

        [JsonIgnore]
        public virtual Holder Holder { get; set; }
        [JsonIgnore]
        public virtual CertificateType Type { get; set; }
    }
}
