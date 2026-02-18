using Auditing.Services;
using Business.Models;
using Business.Services;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly IAuditService _auditService;
    private readonly IDalCoversService _dalCoversService;

    public CoversController(ILogger<CoversController> logger, IAuditService auditService, IDalCoversService dalCoversService)
    {
        _logger = logger;
        _auditService = auditService;
        _dalCoversService = dalCoversService;
    }

    [HttpPost("compute")]
    public ActionResult ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(CoversService.ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _dalCoversService.GetAll();
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var result = await _dalCoversService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = CoversService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

        _dalCoversService.Create(cover);
        _auditService.AuditCover(cover.Id, "POST");

        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _dalCoversService.DeleteById(id);
        _auditService.AuditCover(id, "DELETE");
    }
}
