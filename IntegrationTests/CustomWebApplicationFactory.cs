using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests;

public class CustomWebApplicationFactory<TProgram>
	: WebApplicationFactory<TProgram> where TProgram : class
{
	private readonly Type[] typesToMock = null!;
	private readonly object[] mockedObjects = null!;

	public CustomWebApplicationFactory(Type typeToMock, object mockedObject)
	{
		this.typesToMock = [typeToMock];
		this.mockedObjects = [mockedObject];
	}

	public CustomWebApplicationFactory(Type[] typesToMock, object[] mockedObjects)
	{
		if (typesToMock.Length != mockedObjects.Length)
			throw new ArgumentException("Types and objects count must be equal.");

		this.typesToMock = typesToMock;
		this.mockedObjects = mockedObjects;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment(Iwtd.Server.Consts.TestEnvironment);

		builder.ConfigureServices(services =>
		{
			var servicesToRemove = services.Where(s => typesToMock.Contains(s.ServiceType)).ToList();
			foreach (var serviceToRemove in servicesToRemove)
				services.Remove(serviceToRemove);
			for (var i = 0; i < typesToMock.Length; i++)
			{
				var i1 = i;
				services.TryAddTransient(typesToMock[i], _ => mockedObjects[i1]);
			}
		});
	}
}