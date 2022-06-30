using CertificateGeneratorAPI.Models.ViewModels;
using System.IO;

namespace CertificateGeneratorAPI.Services.Interfaces
{
    public interface ICertificateGenerator
    {
        public MemoryStream GenerateCertificate(CertificatePDFViewModel certificate, string urlForQRCode);
    }
}
