using System;

namespace CertificateGeneratorAPI.Models.ViewModels
{
    public class CertificatePDFViewModel
    {
        public string BusinessName { get; set; }
        public string RIF { get; set; }
        public string Type { get; set; }
        public string GUID { get; set; }
        public DateTime ExpeditionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
