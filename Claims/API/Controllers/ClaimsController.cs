using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Application.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimService _claimService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="claimService">The claim service instance.</param>
        public ClaimsController(ILogger<ClaimsController> logger, IClaimService claimService)
        {
            _logger = logger;
            _claimService = claimService;
        }

        /// <summary>
        /// Retrieves all claims asynchronously.
        /// </summary>
        /// <returns>A list of all claims.</returns>
        [HttpGet]
        public async Task<List<Claim>> GetAsync()
        {
            return await _claimService.GetClaimsAsync();
        }

        /// <summary>
        /// Creates a new claim asynchronously.
        /// </summary>
        /// <param name="claim">The claim to create.</param>
        /// <returns>The created claim.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            var createdClaim = await _claimService.CreateClaimAsync(claim);
            //_auditService.AuditClaim(createdClaim.Id, "POST");
            return Ok(createdClaim);
        }

        /// <summary>
        /// Deletes a claim by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the claim to delete.</param>
        /// <returns>Ok if the claim was deleted.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteAsync(string id)
        {
            _claimService.DeleteClaimById(id);
            //_auditService.AuditClaim(id, "DELETE");
            return Ok();
            
        }

        /// <summary>
        /// Retrieves a claim by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the claim.</param>
        /// <returns>The claim with the specified identifier.</returns>
        [HttpGet("{id}")]
        public async Task<Claim> GetAsync(string id)
        {
            var claim = await _claimService.GetClaimByIdAsync(id);
            return claim;
        }
    }
}
