using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoverService _coverService;

    public CoversController(ILogger<CoversController> logger, ICoverService coverService)
    {
        _logger = logger;
        _coverService = coverService;
    }

    [HttpPost("compute")]
    public ActionResult ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(_coverService.ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coverService.GetAll();
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var result = await _coverService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = _coverService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

        _coverService.Create(cover);
        //_auditService.AuditCover(cover.Id, "POST");

        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _coverService.DeleteById(id);
        //_auditService.AuditCover(id, "DELETE");
        return NoContent();
    }
}
