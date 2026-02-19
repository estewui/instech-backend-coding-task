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

        public ClaimsController(ILogger<ClaimsController> logger, IClaimService claimService)
        {
            _logger = logger;
            _claimService = claimService;
        }

        [HttpGet]
        public async Task<List<Claim>> GetAsync()
        {
            return await _claimService.GetClaimsAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            var createdClaim = await _claimService.CreateClaimAsync(claim);
            //_auditService.AuditClaim(createdClaim.Id, "POST");
            return Ok(createdClaim);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAsync(string id)
        {
            _claimService.DeleteClaimById(id);
            //_auditService.AuditClaim(id, "DELETE");
            return Ok();
            
        }

        [HttpGet("{id}")]
        public async Task<Claim> GetAsync(string id)
        {
            var claim = await _claimService.GetClaimByIdAsync(id);
            return claim;
        }
    }
}
