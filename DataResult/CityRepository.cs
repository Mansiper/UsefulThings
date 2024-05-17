namespace DataResultExample.Repositories;

public class CityRepository : ICityRepository
{
	private readonly ApplicationDbContext context;

	public CityRepository(ApplicationDbContext context) =>
		this.context = context;

	public async Task<DataResult<City>> Get(Guid id, CancellationToken ct)
	{
		var entity = await context.Cities
			.AsNoTracking()
			.FirstOrDefaultAsync(e => e.Id == id, ct);
		return new(entity, RequestExceptionType.NotFound, ReposConsts.City.IsNotFound, nameof(City));
	}

	public async Task<DataResult<City>> Insert(City entity)
	{
		var exists = await context.Cities
			.AsNoTracking()
			.AnyAsync(e => e.Name.ToUpper() == entity.Name.ToUpper());
		if (exists)
			return new(RequestExceptionType.AlreadyExists, ReposConsts.City.AlreadyExists, nameof(City));

		var result = await context.Cities.AddAsync(entity);
		await context.SaveChangesAsync();

		return new(result.Entity);
	}

	public async Task<DataResult> Delete(Guid id)
	{
		var entity = await context.Cities
			.FirstOrDefaultAsync(e => e.Id == id);
		if (entity == null)
			return new (RequestExceptionType.NotFound, ReposConsts.City.IsNotFound, nameof(City));

		try
		{
			context.Cities.Remove(entity);
			await context.SaveChangesAsync();
			return DataResult.Empty;
		}
		catch
		{
			return new(RequestExceptionType.CannotDelete, ReposConsts.City.CannotDelete + $" {entity.Name}", nameof(City));
		}
	}
}