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
    public class HolderController : ControllerBase
    {
        private readonly ICertificateData _certificateDataService;

        public HolderController(ICertificateData certificateDataService)
        {
            _certificateDataService = certificateDataService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Holder>))]
        [ProducesResponseType(204)]
        public async Task<ActionResult<IEnumerable<Holder>>> GetHolders()
        {
            List<Holder> holders = await _certificateDataService.ReadHolders();
            return holders.Count == 0 ? NoContent() : Ok(holders);

        }

        [HttpGet("filter")]
        [ProducesResponseType(200, Type = typeof(List<Holder>))]
        [ProducesResponseType(204)]
        public async Task<ActionResult<IEnumerable<Holder>>> GetHolders([FromQuery] HolderInputFilter holderInputFilter)
        {
            List<Holder> holders = await _certificateDataService.ReadHolders(holderInputFilter);
            return holders.Count == 0 ? NoContent() : Ok(holders);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Holder))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Holder>> GetHolder(int id)
        {
            Holder holder = await _certificateDataService.ReadHolder(id);
            return holder == null ? NotFound() : Ok(holder);

        }

        #endregion

        #region POST

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(int))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateHolder(HolderInput holderInput)
        {
            Holder holder = await _certificateDataService.ReadHolder(holderInput);

            if (holder != null)
            {
                return BadRequest($"Holder with RIF {holderInput.RIF} already exists.");
            }

            int id = await _certificateDataService.CreateHolder(holderInput);
            return CreatedAtAction(nameof(GetHolder), new { id }, new { id });
        }

        #endregion

        #region PUT

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateHolder(int id, HolderInput holderInput)
        {
            Holder holder = await _certificateDataService.ReadHolder(id);

            if (holder == null)
            {
                return NotFound();
            }

            await _certificateDataService.UpdateHolder(id, holderInput);
            return Ok();
        }

        #endregion

        #region DELETE

        [HttpDelete]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteHolders()
        {
            await _certificateDataService.DeleteHolders();
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteHolder(int id)
        {
            Holder holder = await _certificateDataService.ReadHolder(id);

            if (holder == null)
            {
                return NotFound();
            }

            await _certificateDataService.DeleteHolder(id);
            return Ok();
        }

        #endregion
    }
}
