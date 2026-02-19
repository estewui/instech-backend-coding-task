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

    /// <summary>
    /// Initializes a new instance of the <see cref="CoversController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="coverService">The cover service instance.</param>
    public CoversController(ILogger<CoversController> logger, ICoverService coverService)
    {
        _logger = logger;
        _coverService = coverService;
    }

    /// <summary>
    /// Computes the premium for a cover based on the provided dates and cover type.
    /// </summary>
    /// <param name="startDate">The start date of the cover.</param>
    /// <param name="endDate">The end date of the cover.</param>
    /// <param name="coverType">The type of the cover.</param>
    /// <returns>The computed premium.</returns>
    [HttpPost("compute")]
    public ActionResult ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(_coverService.ComputePremium(startDate, endDate, coverType));
    }

    /// <summary>
    /// Retrieves all covers asynchronously.
    /// </summary>
    /// <returns>A list of all covers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coverService.GetAll();
        return Ok(results);
    }

    /// <summary>
    /// Retrieves a cover by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the cover.</param>
    /// <returns>The cover with the specified identifier.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var result = await _coverService.GetById(id);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new cover asynchronously.
    /// </summary>
    /// <param name="cover">The cover to create.</param>
    /// <returns>The created cover.</returns>
    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = _coverService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

        _coverService.Create(cover);
        //_auditService.AuditCover(cover.Id, "POST");

        return Ok(cover);
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
        //_auditService.AuditCover(id, "DELETE");
        return NoContent();
    }
}
