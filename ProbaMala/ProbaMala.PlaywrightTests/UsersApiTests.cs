using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

[Collection("api")]
public class UsersApiTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public UsersApiTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var resp = await Api.GetAsync("/api/users");
        Assert.Equal(200, resp.Status);
        Assert.Equal(JsonValueKind.Array, (await resp.JsonElementAsync()).ValueKind);
    }

    [Fact]
    public async Task Crud_Roundtrip()
    {
        int id = 0;
        var tag = ApiFixture.Tag();
        try
        {
            var create = await Api.PostAsync("/api/users", new() { DataObject = new { firstName = "PW", lastName = $"User{tag}", email = $"pw{tag}@example.com" } });
            Assert.Equal(201, create.Status);
            id = await create.IdAsync();

            Assert.Equal(200, (await Api.GetAsync($"/api/users/{id}")).Status);

            var put = await Api.PutAsync($"/api/users/{id}", new() { DataObject = new { firstName = "PW", lastName = $"Renamed{tag}", email = $"pw{tag}@example.com" } });
            Assert.Equal(200, put.Status);
            Assert.Equal($"Renamed{tag}", (await put.JsonElementAsync()).GetProperty("lastName").GetString());

            Assert.Equal(204, (await Api.DeleteAsync($"/api/users/{id}")).Status);
            Assert.Equal(404, (await Api.GetAsync($"/api/users/{id}")).Status);
            id = 0;
        }
        finally { if (id != 0) await _fixture.TryDeleteAsync($"/api/users/{id}"); }
    }

    [Fact]
    public async Task MissingId_Returns404()
    {
        const int missing = 999999;
        Assert.Equal(404, (await Api.GetAsync($"/api/users/{missing}")).Status);
        Assert.Equal(404, (await Api.PutAsync($"/api/users/{missing}", new() { DataObject = new { firstName = "x", lastName = "y", email = "x@y.com" } })).Status);
        Assert.Equal(404, (await Api.DeleteAsync($"/api/users/{missing}")).Status);
    }

    [Fact]
    public async Task Post_InvalidEmail_Returns400()
    {
        var resp = await Api.PostAsync("/api/users", new() { DataObject = new { firstName = "PW", lastName = "Bad", email = "not-an-email" } });
        Assert.Equal(400, resp.Status);
    }
}
