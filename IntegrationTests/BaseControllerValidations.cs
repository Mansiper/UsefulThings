using Service;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Method = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace IntegrationTests;

public class BaseControllerValidations
{
	protected CustomWebApplicationFactory<Program> Factory = null!;
	protected readonly IList<string> AdminRole = [Roles.AdminRole];

	private const string DefaultEmail = "test@example.com";
	private readonly IdentityUser defaultUser = new (DefaultEmail){Email = DefaultEmail};
	private readonly List<string> defaultRoles = Roles.UsersRoles.Append(Roles.AdminRole).ToList();

	protected List<string> RolesExcept(string role) =>
		defaultRoles.Where(r => r != role).ToList();

	protected async Task RunDefaultSuccessTest(string url, HttpMethod method, object? model = null, Action? setup = null,
		IList<string>? roles = null)
	{
		// Arrange
		var client = Factory.CreateClient();
		var request = CreateRequest(url, method, model, roles);

		setup?.Invoke();

		// Act
		var response = await client.SendAsync(request);

		// Assert
		response.IsSuccessStatusCode.Should().BeTrue();
	}

	protected async Task RunTest(object model, string url, HttpMethod method, string[] keys, string[] values,
		IList<string>? roles = null)
	{
		// Arrange
		var client = Factory.CreateClient();
		var request = CreateRequest(url, method, model, roles);

		// Act
		var response = await client.SendAsync(request);

		// Assert
		var errors = await GetValidationResults(response);
		DefaultAssert(response, errors, keys, values);
	}

	protected async Task RunNotAuthorizedTest(string url, Method method, IList<string>? roles = null) =>
		await RunNotAuthorizedTest(url, new HttpMethod(method.ToString()), roles);

	protected async Task RunNotAuthorizedTest(string url, HttpMethod method, IList<string>? roles = null)
	{
		// Arrange
		var client = Factory.CreateClient();
		var request = CreateRequest(url, method, new object(), roles);

		// Act
		var response = await client.SendAsync(request);

		// Assert
		using var _ = new AssertionScope();
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
	}

	private HttpRequestMessage CreateRequest(string url, HttpMethod method, object? model, IList<string>? roles)
	{
		var request = new HttpRequestMessage(method, url)
		{
			Content = model is null ? null : GetJsonContent(model),
		};

		//roles is null - check anonymous request
		//roles is empty - check all roles
		if (roles is not null)
		{
			if (!roles.Any()) roles = defaultRoles;
			var jwtTokenService = Factory.Services.GetService<IJwtTokenService>();
			var jwtToken = jwtTokenService!.BuildToken(defaultUser, roles);
			request.Headers.Add("Authorization", $"bearer {jwtToken}");
		}

		return request;
	}

	private static StringContent GetJsonContent(object model)
	{
		var content = new StringContent(JsonSerializer.Serialize(model));
		content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		return content;
	}

	private async Task<Dictionary<string, string[]>> GetValidationResults(HttpResponseMessage response)
	{
		var content = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<ValidationError>(content)!.Errors!;
	}

	private static void DefaultAssert(HttpResponseMessage response, Dictionary<string, string[]> errors,
		string[] keys, string[] values)
	{
		using var _ = new AssertionScope();
		response.IsSuccessStatusCode.Should().BeFalse();
		errors.Count.Should().Be(keys.Length);
		errors.Keys.ToArray().Should().BeEquivalentTo(keys);
		if (values.Any())
		{
			var i = 0;
			foreach (var value in errors.Values.SelectMany(v => v))
			{
				value.Should().Contain(values[i]);
				i++;
			}
		}
	}
}