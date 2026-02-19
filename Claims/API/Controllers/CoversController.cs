using API.Contracts.Requests;
using API.Contracts.Responses;
using API.Contracts.Types;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoverService _coverService;
    private readonly IAuditService _auditService;

    public CoversController(ILogger<CoversController> logger, ICoverService coverService, IAuditService auditService)
    {
        _logger = logger;
        _coverService = coverService;
        _auditService = auditService;
    }

    /// <summary>
    /// Computes the premium for a cover based on the provided dates and cover type.
    /// </summary>
    /// <param name="request">The request containing start date, end date, and cover type.</param>
    /// <returns>The computed premium.</returns>
    [HttpPost("compute")]
    public ActionResult<decimal> ComputePremium([FromBody] ComputePremiumRequest request)
    {
        var premium = _coverService.ComputePremium(request.StartDate, request.EndDate, (Domain.Entities.CoverType)(int)request.Type);
        return Ok(premium);
    }

    /// <summary>
    /// Retrieves all covers asynchronously.
    /// </summary>
    /// <returns>A list of all covers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverResponse>>> GetAsync()
    {
        var results = await _coverService.GetAll();
        var response = results.Select(c => new CoverResponse
        {
            Id = c.Id,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            Type = (CoverType)(int)c.Type,
            Premium = c.Premium
        });
        return Ok(response);
    }

    /// <summary>
    /// Retrieves a cover by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the cover.</param>
    /// <returns>The cover with the specified identifier.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CoverResponse>> GetAsync(string id)
    {
        var result = await _coverService.GetById(id);
        if (result == null)
            return NotFound();
        var response = new CoverResponse
        {
            Id = result.Id,
            StartDate = result.StartDate,
            EndDate = result.EndDate,
            Type = (CoverType)(int)result.Type,
            Premium = result.Premium
        };
        return Ok(response);
    }

    /// <summary>
    /// Creates a new cover asynchronously.
    /// </summary>
    /// <param name="request">The cover to create.</param>
    /// <returns>The created cover.</returns>
    [HttpPost]
    public async Task<ActionResult<CoverResponse>> CreateAsync([FromBody] CreateCoverRequest request)
    {
        var cover = new Domain.Entities.Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = (Domain.Entities.CoverType)(int)request.Type,
            Premium = request.Premium
        };
        cover.Premium = _coverService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await _coverService.Create(cover);
        var response = new CoverResponse
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Type = (CoverType)(int)cover.Type,
            Premium = cover.Premium
        };
        _auditService.AuditCover(cover.Id, "POST");
        return Ok(response);
    }

    /// <summary>
    /// Deletes a cover by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the cover to delete.</param>
    /// <returns>No content if the cover was deleted.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _coverService.DeleteById(id);
        _auditService.AuditCover(id, "DELETE");
        return NoContent();
    }
}
