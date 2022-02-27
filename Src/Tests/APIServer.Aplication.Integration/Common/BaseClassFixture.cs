using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace APIServer.Application.IntegrationTests
{
  public class BaseClassFixture : IClassFixture<XunitFixture>
  {
    protected readonly TestServer TestServer;

    public BaseClassFixture(XunitFixture fixture)
    {
      TestServer = fixture.TestServer;
    }
  }
}