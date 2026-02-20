using API.Contracts.Requests;
using API.Contracts.Responses;
using Application.Common.Auditing;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoverService _coverService;
    private readonly IAuditSink _auditSink;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoversController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="coverService">The cover service instance.</param>
    /// <param name="auditSink">The audit sink instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public CoversController(ILogger<CoversController> logger, ICoverService coverService, IAuditSink auditSink, IMapper mapper)
    {
        _logger = logger;
        _coverService = coverService;
        _auditSink = auditSink;
        _mapper = mapper;
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
        var response = _mapper.Map<IEnumerable<CoverResponse>>(results);
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
        
        var response = _mapper.Map<CoverResponse>(result);
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
        var cover = _mapper.Map<Cover>(request);
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = _coverService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        
        await _coverService.Create(cover);
        var response = _mapper.Map<CoverResponse>(cover);
        
        await _auditSink.EnqueueAsync(new AuditEvent
        {
            Type = AuditType.Cover,
            EntityId = cover.Id,
            HttpRequestType = "POST",
            Timestamp = DateTime.UtcNow
        });
        
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
        await _auditSink.EnqueueAsync(new AuditEvent
        {
            Type = AuditType.Cover,
            EntityId = id,
            HttpRequestType = "DELETE",
            Timestamp = DateTime.UtcNow
        });
        return NoContent();
    }
}
