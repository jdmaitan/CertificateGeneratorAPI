using CertificateGeneratorAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace CertificateGeneratorAPI.Models.InputModels
{
    public class CertificateTypeInput
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Description must contain only letters")]
        [StringLength(30)]
        public string Description { get; set; }

        [Required]
        [Range(1, 120, ErrorMessage = "A certificate can't last more than 10 years")]
        public int ValidityMonths { get; set; }

        public CertificateType ToCertificateType()
        {
            return new CertificateType
            {
                Description = this.Description,
                ValidityMonths = this.ValidityMonths
            };
        }

        public void UpdateCertificateType(CertificateType certificateType)
        {
            certificateType.Description = this.Description;
            certificateType.ValidityMonths = this.ValidityMonths;
        }
    }
}
