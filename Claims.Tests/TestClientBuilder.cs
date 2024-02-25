using Microsoft.AspNetCore.Mvc.Testing;

namespace Claims.Tests;

public class TestClientBuilder
{
    public static TestClient CreateTestClient()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });
        return new TestClient(application.CreateClient());
    }
}