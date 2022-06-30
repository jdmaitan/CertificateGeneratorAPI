using CertificateGeneratorAPI.Models.ViewModels;
using CertificateGeneratorAPI.Services.Interfaces;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Text;

namespace CertificateGeneratorAPI
{
    public class CertificateGenerator : ICertificateGenerator
    {
        public MemoryStream GenerateCertificate(CertificatePDFViewModel certificate, string urlForQRCode)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            PdfDocument pdfDocument = new PdfDocument();
            PdfPage pdfPage = pdfDocument.AddPage();
            pdfPage.Size = PdfSharpCore.PageSize.Letter;
            XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);

            #region Background

            XImage xImage = XImage.FromFile("Assets/CertificateBackground.jpg");
            xGraphics.DrawImage(xImage, 0, 0, pdfPage.Width, pdfPage.Height);

            #endregion

            #region Header

            int logoWitd = 300;
            xImage = XImage.FromFile("Assets/Logos/GenericHeaderLogo.png");
            xGraphics.DrawImage(xImage, (pdfPage.Width / 2) - logoWitd / 2, 100, logoWitd, 100);

            xImage = XImage.FromFile("Assets/CertificateHeader.png");
            xGraphics.DrawImage(xImage, (pdfPage.Width / 2) - 180, 219, 360, 110);

            #endregion

            #region Holder

            XFont font = new XFont("Arial", 20, XFontStyle.Regular);
            xGraphics.DrawString("This certificate is awarded to:",
                           font,
                           XBrushes.Black,
                           new XRect(0, -43, pdfPage.Width, pdfPage.Height),
                           XStringFormats.Center);


            int smallBusinessNameMaxLength = 20;
            int mediumBusinessNameMaxLength = 30;
            int largeBusinessNameLength = 40;
            int xlBusinessNameLength = 50;
            int businessNameLength = certificate.BusinessName.Length;

            if (businessNameLength <= smallBusinessNameMaxLength)
            {
                font = new XFont("Arial", 26, XFontStyle.Bold);
            }
            else if (businessNameLength <= mediumBusinessNameMaxLength)
            {
                font = new XFont("Arial", 23, XFontStyle.Bold);
            }
            else if (businessNameLength <= largeBusinessNameLength)
            {
                font = new XFont("Arial", 19, XFontStyle.Bold);
            }
            else if (businessNameLength <= xlBusinessNameLength)
            {
                font = new XFont("Arial", 14, XFontStyle.Bold);
            }
            else
            {
                font = new XFont("Arial", 13, XFontStyle.Bold);
            }

            xGraphics.DrawString(certificate.BusinessName.ToUpper(),
                           font,
                           XBrushes.Black,
                           new XRect(0, -5, pdfPage.Width, pdfPage.Height),
                           XStringFormats.Center);

            font = new XFont("Arial", 15, XFontStyle.Bold);
            xGraphics.DrawString($"RIF: {certificate.RIF.ToUpper()}",
                           font,
                           XBrushes.Black,
                           new XRect(0, 20, pdfPage.Width, pdfPage.Height),
                           XStringFormats.Center);

            #endregion

            #region Certificate type
            double certificateTypeInitialY = 48;

            font = new XFont("Arial", 15, XFontStyle.Regular);
            xGraphics.DrawString("which hereby is acknowledged as a:",
                           font,
                           XBrushes.Black,
                           new XRect(0, certificateTypeInitialY, pdfPage.Width, pdfPage.Height),
                           XStringFormats.Center);

            font = new XFont("Arial", 20, XFontStyle.Bold);
            xGraphics.DrawString(certificate.Type.ToUpper(),
                           font,
                           XBrushes.Black,
                           new XRect(0, certificateTypeInitialY + 20, pdfPage.Width, pdfPage.Height),
                           XStringFormats.Center);
            #endregion

            #region Footer
            double certificateBottomMessageInitialY = certificateTypeInitialY + 45;
            double spaceBetweenLines = 18;

            font = new XFont("Arial", 11, XFontStyle.Bold);
            xGraphics.DrawString("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod",
                          font,
                          XBrushes.Black,
                          new XRect(0, certificateBottomMessageInitialY, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            xGraphics.DrawString("tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim",
                          font,
                          XBrushes.Black,
                          new XRect(0, certificateBottomMessageInitialY + spaceBetweenLines, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            xGraphics.DrawString("veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip",
                          font,
                          XBrushes.Black,
                          new XRect(0, certificateBottomMessageInitialY + spaceBetweenLines * 2, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            xGraphics.DrawString("ex ea commodo consequat. Duis aute irure dolor in",
                          font,
                          XBrushes.Black,
                          new XRect(0, certificateBottomMessageInitialY + spaceBetweenLines * 3, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            xGraphics.DrawString("in voluptate velit esse cillum dolore eu",
                          font,
                          XBrushes.Black,
                          new XRect(0, certificateBottomMessageInitialY + spaceBetweenLines * 4, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            xGraphics.DrawString("pariatur. Excepteur sint.",
                          font,
                          XBrushes.Black,
                          new XRect(0, certificateBottomMessageInitialY + spaceBetweenLines * 5, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);
            #endregion

            #region QR

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(urlForQRCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            Stream stream = new MemoryStream();
            Stream stream2 = new MemoryStream();
            qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            stream.Position = 0;

            //To prevent runtime exception. Don't delete
            stream.CopyTo(stream2);
            stream2.Position = 0;
            //

            xImage = XImage.FromStream(() => stream2);
            xGraphics.DrawImage(xImage, 80, 575, 95, 95);

            #endregion

            #region Dates
            double datesInitialX = -75;
            double dateDataInitialY = 210;

            font = new XFont("Arial", 11, XFontStyle.Bold);
            xGraphics.DrawString(certificate.ExpeditionDate.ToString("dd-MM-yyyy"),
                          font,
                          XBrushes.Black,
                          new XRect(datesInitialX, dateDataInitialY, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            font = new XFont("Arial", 11, XFontStyle.Regular);
            xGraphics.DrawString("Expedition Date",
                          font,
                          XBrushes.Black,
                          new XRect(datesInitialX, dateDataInitialY + 10, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            font = new XFont("Arial", 11, XFontStyle.Bold);
            xGraphics.DrawString(certificate.ExpirationDate.ToString("dd-MM-yyyy"),
                          font,
                          XBrushes.Black,
                          new XRect(datesInitialX, dateDataInitialY + 30, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            font = new XFont("Arial", 11, XFontStyle.Regular);
            xGraphics.DrawString("Expiration Date",
                          font,
                          XBrushes.Black,
                          new XRect(datesInitialX, dateDataInitialY + 40, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);
            #endregion

            #region Signature

            xImage = XImage.FromFile("Assets/signature.png");
            xGraphics.DrawImage(xImage, 329, 560, 210, 120);

            int lineInitialX = 324;
            XPen signatureLine = new XPen(XColors.Black, 1);
            xGraphics.DrawLine(signatureLine, lineInitialX, 635, lineInitialX + 200, 635);

            font = new XFont("Arial", 11, XFontStyle.Regular);
            xGraphics.DrawString("Head of organization",
                          font,
                          XBrushes.Black,
                          new XRect(120, 250, pdfPage.Width, pdfPage.Height),
                          XStringFormats.Center);

            #endregion

            #region Logos

            double logosInitialX = 80;
            double logosY = 670;
            double spaceBetweenLogos = 120;

            xImage = XImage.FromFile("Assets/Logos/GenericFooterLogo.png");
            xGraphics.DrawImage(xImage, logosInitialX, logosY, 87, 38);

            xImage = XImage.FromFile("Assets/Logos/GenericFooterLogo.png");
            xGraphics.DrawImage(xImage, logosInitialX + spaceBetweenLogos, logosY, 87, 38);

            xImage = XImage.FromFile("Assets/Logos/GenericFooterLogo.png");
            xGraphics.DrawImage(xImage, logosInitialX + spaceBetweenLogos * 2, logosY, 87, 38);

            xImage = XImage.FromFile("Assets/Logos/GenericFooterLogo.png");
            xGraphics.DrawImage(xImage, logosInitialX + spaceBetweenLogos * 3, logosY, 87, 38);

            #endregion

            MemoryStream memoryStream = new MemoryStream();
            pdfDocument.Save(memoryStream, false);

            return memoryStream;
        }
    }
}
