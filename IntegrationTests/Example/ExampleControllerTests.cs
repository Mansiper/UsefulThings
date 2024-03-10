using Service;
using Method = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IntegrationTests;

[TestFixture]
public class ExampleControllerTests : BaseControllerValidations
{
	private Mock<IService> service = null!;

	[SetUp]
	public void Setup()
	{
		service = new();
		Factory = new(typeof(IService), service.Object);
	}

	#region Dto Validation

	[Test]
	public async Task CreateEntity_ShouldPassValidation() =>
		await PostDefaultSuccessTest(
			new ExampleDto
			{
				Id = 1,
				Name = "name",
			},
			"api/example",
			roles: [Role1]);

	[TestCase((string)null!)]
	[TestCase("")]
	[TestCase(" ")]
	public async Task CreateEntity_ShouldFail_WhenNameIsNullOrWhiteSpace(string? name) =>
		await RunDefaultTest(
			new ExampleDto
			{
				Id = 1,
				Name = name!,
			},
			"api/example",
			[nameof(ExampleDto.Name)],
			["required"],
			[Role1]);

	#endregion

	#region Authorization

	[Test]
	public async Task GetAnonymous_ShouldSuccess()
	{
		service.Setup(x => x.GetAnonymous(It.IsAny<int>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ExampleDto());
		await RunDefaultSuccessTest(
			$"api/example/123",
			HttpMethod.Get);
	}

	[TestCase(Method.Get, $"api/example")]
	[TestCase(Method.Delete, "api/example/123")]
	public async Task Method_ShouldFail_WhenUnauthorized(Method method, string uri) =>
		await RunNotAuthorizedTest(
			uri,
			method,
			RolesExcept(Role1));

	#endregion
}