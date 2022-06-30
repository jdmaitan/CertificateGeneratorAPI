using CertificateGeneratorAPI.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace CertificateGeneratorAPI.Models.InputModels
{
    public class CertificateInput
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int HolderID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TypeID { get; set; }

        public void UpdateCertificate(Certificate certificate)
        {
            certificate.HolderID = this.HolderID;
            certificate.TypeID = this.TypeID;
        }
    }
}
