using Microsoft.AspNetCore.Mvc;

using AutoMapper;

using API.Contracts.Requests;
using API.Contracts.Responses;
using Application.Common.Auditing;
using Application.Services;
using Domain.Entities;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoverService _coverService;
    private readonly IAuditSink _auditSink;
    private readonly IMapper _mapper;

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
        try
        {
            var premium = _coverService.ComputePremium(request.StartDate, request.EndDate, (Domain.Entities.CoverType)(int)request.Type);
            return Ok(premium);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error computing premium");
            return StatusCode(500, "An error occurred while computing the premium.");
        }
    }

    /// <summary>
    /// Retrieves all covers asynchronously.
    /// </summary>
    /// <returns>A list of all covers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverResponse>>> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var results = await _coverService.GetAll(cancellationToken);
            var response = _mapper.Map<IEnumerable<CoverResponse>>(results);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting covers");
            return StatusCode(500, "An error occurred while getting the covers.");
        }
    }

    /// <summary>
    /// Retrieves a cover by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the cover.</param>
    /// <returns>The cover with the specified identifier.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CoverResponse>> GetAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _coverService.GetById(id, cancellationToken);
            if (result == null)
                return NotFound();
        
            var response = _mapper.Map<CoverResponse>(result);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cover");
            return StatusCode(500, "An error occurred while getting the cover.");
        }
    }

    /// <summary>
    /// Creates a new cover asynchronously.
    /// </summary>
    /// <param name="request">The cover to create.</param>
    /// <returns>The created cover.</returns>
    [HttpPost]
    public async Task<ActionResult<CoverResponse>> CreateAsync([FromBody] CreateCoverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var cover = _mapper.Map<Cover>(request);
            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = _coverService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        
            await _coverService.Create(cover, cancellationToken);
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
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for cover creation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cover");
            return StatusCode(500, "An error occurred while creating the cover.");
        }
    }

    /// <summary>
    /// Deletes a cover by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the cover to delete.</param>
    /// <returns>No content if the cover was deleted.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            await _coverService.DeleteById(id, cancellationToken);
            await _auditSink.EnqueueAsync(new AuditEvent
            {
                Type = AuditType.Cover,
                EntityId = id,
                HttpRequestType = "DELETE",
                Timestamp = DateTime.UtcNow
            });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting cover");
            return StatusCode(500, "An error occurred while deleting the cover.");
        }
    }
}
