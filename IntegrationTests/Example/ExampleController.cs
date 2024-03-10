namespace Service;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Role1)]
public class ExampleController : ControllerBase
{
    private readonly IService service;

    public ExampleController(IService service) =>
	    this.service = service;

    [HttpGet]
    public async Task<ExampleDto> Get(CancellationToken ct) =>
        await service.Get(ct);

    [HttpGet("{id:int}")]
	[AllowAnonymous]
	public async Task<ExampleDto>> GetAnonymous(int id, CancellationToken ct) =>
        await service.GetAnonymous(id, ct);

	[HttpPost]
    public async Task<IActionResult> CreateEntity(ExampleDto dto)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);

		return Ok(await service.CreateEntity(dto));
    }

	[HttpDelete("{id:int}")]
    public async Task<bool> Delete(int id) =>
        await service.Delete(id);
}