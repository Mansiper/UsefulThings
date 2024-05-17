namespace PaginationExample;

public class CityRepository : ICityRepository
{
	private readonly ApplicationDbContext context;

	public CityRepository(ApplicationDbContext context) =>
		this.context = context;

	public async Task<List<City>> Get(int page, int size, CancellationToken ct) =>
		await context.Cities
			.AsNoTracking()
			.ORderBy(c => c.Name)
			.Skip((page - 1) * size)
			.Take(size)
			.ToListAsync(ct);

	public async Task<int> Count(CancellationToken ct) =>
		await context.Cities.CountAsync(ct);
}