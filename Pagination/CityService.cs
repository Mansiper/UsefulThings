namespace PaginationExample;

public class CityService : ICityService
{
	private readonly ICityRepository repository;
	
	public CityService(ICityRepository repository) =>
		this.repository = repository;

	public async Task<Paginged<CityDto>> Get(PaginationQuery pagination, CancellationToken ct) =>
		new(pagination)
		{
			Data = (await repository.Get(pagination.Page!.Value, pagination.Size!.Value, ct))
				.Select(CityMapper.MapToDto)
				.ToArray(),
			Count = await repository.Count(ct),
		};
}