namespace PaginationExample;

[ApiController]
[Route("api/[controller]")]
public class CityController : ControllerBase
{
    private readonly ICityService service;

    public CityController(ICityService service) =>
	    this.service = service;

    [HttpGet]
	public async Task<Paginged<CityDto>> Get([FromQuery] PaginationQuery pagination, CancellationToken ct) =>
        await service.Get(pagination, ct);
}