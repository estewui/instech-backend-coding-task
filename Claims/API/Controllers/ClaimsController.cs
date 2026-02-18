using Auditing.Services;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;
using Business.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IAuditService _auditService;
        private readonly IDalClaimsService _dalClaimsService;

        public ClaimsController(ILogger<ClaimsController> logger, IAuditService auditService, IDalClaimsService dalClaimsService)
        {
            _logger = logger;
            _auditService = auditService;
            _dalClaimsService = dalClaimsService;
        }

        [HttpGet]
        public async Task<List<Claim>> GetAsync()
        {
            return await _dalClaimsService.GetAll();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            var createdClaim = await _dalClaimsService.Create(claim);
            _auditService.AuditClaim(createdClaim.Id, "POST");
            return Ok(createdClaim);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await _dalClaimsService.DeleteById(id);
            _auditService.AuditClaim(id, "DELETE");
            
        }

        [HttpGet("{id}")]
        public async Task<Claim> GetAsync(string id)
        {
            var claim = await _dalClaimsService.GetById(id);
            return claim;
        }
    }
}
