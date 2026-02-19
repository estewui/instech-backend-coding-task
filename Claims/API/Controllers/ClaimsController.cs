using Microsoft.AspNetCore.Mvc;
using Application.Services;
using API.Contracts.Requests;
using API.Contracts.Responses;
using API.Contracts.Types;

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
        public async Task<List<ClaimResponse>> GetAsync()
        {
            var claims = await _claimService.GetClaimsAsync();
            return claims.Select(c => new ClaimResponse
            {
                Id = c.Id,
                CoverId = c.CoverId,
                Created = c.Created,
                Name = c.Name,
                Type = (ClaimType)(int)c.Type,
                DamageCost = c.DamageCost
            }).ToList();
        }

        /// <summary>
        /// Creates a new claim asynchronously.
        /// </summary>
        /// <param name="request">The claim to create.</param>
        /// <returns>The created claim.</returns>
        [HttpPost]
        public async Task<ActionResult<ClaimResponse>> CreateAsync([FromBody] CreateClaimRequest request)
        {
            var claim = new Domain.Entities.Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = request.CoverId,
                Created = request.Created,
                Name = request.Name,
                Type = (Domain.Entities.ClaimType)(int)request.Type,
                DamageCost = request.DamageCost
            };
            var createdClaim = await _claimService.CreateClaimAsync(claim);
            var response = new ClaimResponse
            {
                Id = createdClaim.Id,
                CoverId = createdClaim.CoverId,
                Created = createdClaim.Created,
                Name = createdClaim.Name,
                Type = (ClaimType)(int)createdClaim.Type,
                DamageCost = createdClaim.DamageCost
            };
            return Ok(response);
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
            return Ok();
        }

        /// <summary>
        /// Retrieves a claim by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the claim.</param>
        /// <returns>The claim with the specified identifier.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimResponse>> GetAsync(string id)
        {
            var claim = await _claimService.GetClaimByIdAsync(id);
            if (claim == null)
                return NotFound();
            var response = new ClaimResponse
            {
                Id = claim.Id,
                CoverId = claim.CoverId,
                Created = claim.Created,
                Name = claim.Name,
                Type = (ClaimType)(int)claim.Type,
                DamageCost = claim.DamageCost
            };
            return Ok(response);
        }
    }
}
