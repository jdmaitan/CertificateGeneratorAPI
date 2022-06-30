using CertificateGeneratorAPI.Data;
using CertificateGeneratorAPI.Models.InputModels;
using CertificateGeneratorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateGeneratorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class CertificateTypeController : ControllerBase
    {
        private readonly ICertificateData _certificateDataService;

        public CertificateTypeController(ICertificateData certificateDataService)
        {
            _certificateDataService = certificateDataService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CertificateType>))]
        [ProducesResponseType(204)]
        public async Task<ActionResult<IEnumerable<CertificateType>>> GetCertificateTypes()
        {
            List<CertificateType> certificateTypes = await _certificateDataService.ReadCertificateTypes();
            return certificateTypes.Count == 0 ? NoContent() : Ok(certificateTypes);
        }

        [HttpGet("filter")]
        [ProducesResponseType(200, Type = typeof(List<CertificateType>))]
        [ProducesResponseType(204)]
        public async Task<ActionResult<IEnumerable<CertificateType>>> GetCertificateTypes([FromQuery] CertificateTypeInputFilter certificateTypeInputFilter)
        {
            List<CertificateType> certificateTypes = await _certificateDataService.ReadCertificateTypes(certificateTypeInputFilter);
            return certificateTypes.Count == 0 ? NoContent() : Ok(certificateTypes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CertificateType))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CertificateType>> GetCertificateType(int id)
        {
            CertificateType certificateType = await _certificateDataService.ReadCertificateType(id);
            return certificateType == null ? NotFound() : Ok(certificateType);
        }

        #endregion

        #region POST

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(int))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateCertificateType(CertificateTypeInput certificateTypeInput)
        {
            CertificateType certificateType = await _certificateDataService.ReadCertificateType(certificateTypeInput);

            if (certificateType != null)
            {
                return BadRequest($"Certificate type with description {certificateType.Description} already exists.");
            }

            int id = await _certificateDataService.CreateCertificateType(certificateTypeInput);
            return CreatedAtAction(nameof(GetCertificateType), new { id }, new { id });
        }

        #endregion

        #region PUT

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateCertificateType(int id, CertificateTypeInput certificateTypeInput)
        {
            CertificateType certificateType = await _certificateDataService.ReadCertificateType(id);

            if (certificateType == null)
            {
                return NotFound();
            }

            await _certificateDataService.UpdateCertificateType(id, certificateTypeInput);
            return Ok();
        }

        #endregion

        #region DELETE

        [HttpDelete]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteCertificateTypes()
        {
            await _certificateDataService.DeleteCertificateTypes();
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteCertificateType(int id)
        {
            CertificateType certificateType = await _certificateDataService.ReadCertificateType(id);

            if (certificateType == null)
            {
                return NotFound();
            }

            await _certificateDataService.DeleteCertificateType(id);
            return Ok();
        }

        #endregion
    }
}
