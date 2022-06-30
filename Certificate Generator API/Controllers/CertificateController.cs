using CertificateGeneratorAPI.Data;
using CertificateGeneratorAPI.Models.InputModels;
using CertificateGeneratorAPI.Models.ViewModels;
using CertificateGeneratorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateGeneratorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize("Bearer")]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateGenerator _certificateGenerator;
        private readonly ICertificateData _certificateDataService;

        public CertificateController(ICertificateGenerator certificateGenerator,
                                     ICertificateData certificateDataService)
        {
            _certificateGenerator = certificateGenerator;
            _certificateDataService = certificateDataService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Certificate>))]
        [ProducesResponseType(204)]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        {
            List<Certificate> certificates = await _certificateDataService.ReadCertificates();
            return certificates.Count == 0 ? NoContent() : Ok(certificates);

        }

        [HttpGet("filter")]
        [ProducesResponseType(200, Type = typeof(List<Certificate>))]
        [ProducesResponseType(204)]
        public async Task<ActionResult<IEnumerable<CertificatePDFViewModel>>> GetCertificates([FromQuery] CertificateInputFilter certificateInputFilter)
        {
            List<Certificate> certificates = await _certificateDataService.ReadCertificates(certificateInputFilter);
            return certificates.Count == 0 ? NoContent() : Ok(certificates);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Certificate))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Certificate>> GetCertificate(int id)
        {
            Certificate certificate = await _certificateDataService.ReadCertificate(id);
            return certificate == null ? NotFound() : Ok(certificate);

        }

        [HttpGet("file/{guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<ActionResult> GetCertificatePDF(string guid)
        {
            Certificate certificate = await _certificateDataService.ReadCertificate(Guid.Parse(guid));

            if(certificate == null)
            {
                return NotFound();
            }

            CertificatePDFViewModel certificateViewModel = await _certificateDataService.ReadCertificateViewModel(guid);

            return File(_certificateGenerator.GenerateCertificate(certificateViewModel,
                                                                  GenerateURLForQRCode(guid))
                        , "application/pdf"
                        , $"{certificateViewModel.RIF}-{certificateViewModel.Type}.pdf");
        }

        #endregion

        #region POST

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(int))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> CreateCertificate(CertificateInput certificateInput)
        {
            
            Holder holder = await _certificateDataService.ReadHolder(certificateInput.HolderID);

            if(holder == null)
            {
                return NotFound($"Couldn't find holder with ID {certificateInput.HolderID}");
            }

            CertificateType certificateType = await _certificateDataService.ReadCertificateType(certificateInput.TypeID);

            if (certificateType == null)
            {
                return NotFound($"Couldn't find certificate type with ID {certificateInput.TypeID}");
            }

            Certificate certificate = await _certificateDataService.ReadCertificate(certificateInput);

            if (certificate != null)
            {
                return BadRequest("Holder already has a certificate of this type.");
            }

            CreatedCertificateViewModel createdCertificateViewModel = await _certificateDataService.CreateCertificate(certificateInput);
            return CreatedAtAction(nameof(GetCertificate), new { createdCertificateViewModel.ID }, createdCertificateViewModel);
        }

        #endregion

        #region PUT

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateCertificate(int id, CertificateInput certificateInput)
        {
            Certificate certificate = await _certificateDataService.ReadCertificate(id);

            if (certificate == null)
            {
                return NotFound();
            }

            await _certificateDataService.UpdateCertificate(id, certificateInput);
            return Ok();
        }

        [HttpPut("renew/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> RenewCertificate(int id)
        {
            Certificate certificate = await _certificateDataService.ReadCertificate(id);

            if (certificate == null)
            {
                return NotFound();
            }

            await _certificateDataService.RenewCertificate(id);
            return Ok();
        }

        #endregion PUT

        #region DELETE

        [HttpDelete]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteCertificates()
        {
            await _certificateDataService.DeleteCertificates();
            return Ok();
        }

        [HttpDelete("(id)")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteCertificate(int id)
        {
            Certificate certificate = await _certificateDataService.ReadCertificate(id);

            if (certificate == null)
            {
                return NotFound();
            }

            await _certificateDataService.DeleteCertificate(id);
            return Ok();
        }

        #endregion DELETE

        private string GenerateURLForQRCode(string guid)
        {
            return Url.Action(nameof(GetCertificatePDF),
                              "Certificate",
                              new { guid },
                              HttpContext.Request.Scheme,
                              HttpContext.Request.Host.ToString());
        }
    }
}
