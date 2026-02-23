using Microsoft.AspNetCore.Mvc;

using AutoMapper;

using API.Contracts.Requests;
using API.Contracts.Responses;
using Application.Common.Auditing;
using Application.Services;
using Domain.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimService _claimService;
        private readonly IAuditSink _auditSink;
        private readonly IMapper _mapper;

        public ClaimsController(ILogger<ClaimsController> logger, IClaimService claimService, IAuditSink auditSink, IMapper mapper)
        {
            _logger = logger;
            _claimService = claimService;
            _auditSink = auditSink;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all claims asynchronously.
        /// </summary>
        /// <returns>A list of all claims.</returns>
        [HttpGet]
        public async Task<ActionResult<List<ClaimResponse>>> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                var claims = await _claimService.GetClaimsAsync(cancellationToken);
                return _mapper.Map<List<ClaimResponse>>(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting claims");
                return StatusCode(500, "An error occurred while getting claims.");
            }
        }

        /// <summary>
        /// Creates a new claim asynchronously.
        /// </summary>
        /// <param name="request">The claim to create.</param>
        /// <returns>The created claim.</returns>
        [HttpPost]
        public async Task<ActionResult<ClaimResponse>> CreateAsync([FromBody] CreateClaimRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var claim = _mapper.Map<Claim>(request);
                claim.Id = Guid.NewGuid().ToString();
            
                var createdClaim = await _claimService.CreateClaimAsync(claim, cancellationToken);
                var response = _mapper.Map<ClaimResponse>(createdClaim);
            
                await _auditSink.EnqueueAsync(new AuditEvent
                {
                    Type = AuditType.Claim,
                    EntityId = createdClaim.Id,
                    HttpRequestType = "POST",
                    Timestamp = DateTime.UtcNow
                });
            
                return Ok(response);
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for claim creation");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating claim");
                return StatusCode(500, "An error occurred while creating the claim.");
            }
        }

        /// <summary>
        /// Deletes a claim by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the claim to delete.</param>
        /// <returns>Ok if the claim was deleted.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                _claimService.DeleteClaimById(id, cancellationToken);
                await _auditSink.EnqueueAsync(new AuditEvent
                {
                    Type = AuditType.Claim,
                    EntityId = id,
                    HttpRequestType = "DELETE",
                    Timestamp = DateTime.UtcNow
                });
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting claim");
                return StatusCode(500, "An error occurred while deleting the claim.");
            }
        }

        /// <summary>
        /// Retrieves a claim by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the claim.</param>
        /// <returns>The claim with the specified identifier.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimResponse>> GetAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                var claim = await _claimService.GetClaimByIdAsync(id, cancellationToken);
                if (claim == null)
                    return NotFound();
            
                var response = _mapper.Map<ClaimResponse>(claim);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting claim");
                return StatusCode(500, "An error occurred while getting the claim.");
            }
        }
    }
}
