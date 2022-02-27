using System.Linq;
using System.Threading.Tasks;
using APIServer.Aplication.Commands.WebHooks;
using APIServer.Aplication.Shared.Errors;
using APIServer.Domain.Core.Models.WebHooks;
using APIServer.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace APIServer.Application.IntegrationTests.WebHooks
{
  public class RemoveWebHookTests : BaseClassFixture
  {
    private readonly IMediator _mediator;

    private readonly IDbContextFactory<ApiDbContext> _dbcontextfactory;

    public RemoveWebHookTests(XunitFixture fixture) : base(fixture)
    {

      _mediator = this.TestServer.Services
          .GetService<IMediator>();

      _dbcontextfactory = this.TestServer.Services
          .GetService<IDbContextFactory<ApiDbContext>>();
    }

    [Fact]
    public async Task RemoveWebHook_NoError()
    {

      TestCommon.SetAndGetAuthorisedTestContext(this.TestServer);

      await using ApiDbContext dbContext =
          _dbcontextfactory.CreateDbContext();

      WebHook hook = new WebHook()
      {
        WebHookUrl = "https://testurl",
        IsActive = true
      };

      dbContext.WebHooks.Add(hook);

      await dbContext.SaveChangesAsync();

      dbContext.WebHooks.AsNoTracking()
          .Any(e => e.ID == hook.ID).Should().BeTrue();

      var response = await _mediator.Send(new RemoveWebHook()
      {
        WebHookId = hook.ID
      });

      response.Should().NotBeNull();

      response.Should().BeOfType<RemoveWebHookPayload>()
          .Subject.errors.Any().Should().BeFalse();

      dbContext.WebHooks.AsNoTracking().Any(e => e.ID == hook.ID).Should().BeFalse();

      response.removed_id.Should().Be(hook.ID);
    }

    [Fact]
    public async Task RemoveWebHook_ValidationError()
    {

      TestCommon.SetAndGetAuthorisedTestContext(this.TestServer);

      await using ApiDbContext dbContext =
          _dbcontextfactory.CreateDbContext();

      long some_unexisting_id = 999;

      dbContext.WebHooks.AsNoTracking()
          .Any(e => e.ID == some_unexisting_id).Should().BeFalse();

      var response = await _mediator.Send(new RemoveWebHook()
      {
        WebHookId = some_unexisting_id
      });

      response.Should().NotBeNull();

      response.Should().BeOfType<RemoveWebHookPayload>()
          .Subject.errors.Any().Should().BeTrue();

      response.Should().BeOfType<RemoveWebHookPayload>()
          .Subject.errors.First().Should().BeOfType<ValidationError>()
              .Subject.FieldName.Should().Be("WebHookId");
    }

    [Fact]
    public async Task RemoveWebHook_UnauthorisedError()
    {
      TestCommon.SetAndGetUnAuthorisedTestConetxt(this.TestServer);

      await using ApiDbContext dbContext =
          _dbcontextfactory.CreateDbContext();

      WebHook hook = new WebHook()
      {
        WebHookUrl = "https://testurl",
        IsActive = true
      };

      dbContext.WebHooks.Add(hook);

      await dbContext.SaveChangesAsync();

      dbContext.WebHooks.AsNoTracking()
          .Any(e => e.ID == hook.ID).Should().BeTrue();

      var response = await _mediator.Send(new RemoveWebHook()
      {
        WebHookId = hook.ID
      });

      response.Should().NotBeNull();

      response.Should().BeOfType<RemoveWebHookPayload>()
          .Subject.errors.Any().Should().BeTrue();

      response.Should().BeOfType<RemoveWebHookPayload>()
          .Subject.errors.First().Should().BeOfType<UnAuthorised>();

      dbContext.WebHooks.AsNoTracking().Any(e => e.ID == hook.ID).Should().BeTrue();
    }
  }
}