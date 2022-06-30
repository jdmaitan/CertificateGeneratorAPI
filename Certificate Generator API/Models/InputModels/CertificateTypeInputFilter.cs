using System.ComponentModel.DataAnnotations;

namespace CertificateGeneratorAPI.Models.InputModels
{
    public class CertificateTypeInputFilter
    {
        public string Description { get; set; }
        public int ValidityMonths { get; set; }
        public int Limit { get; set; }
    }
}
