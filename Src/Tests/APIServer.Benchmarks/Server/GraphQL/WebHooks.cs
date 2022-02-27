using System.Linq;
using System.Threading.Tasks;
using APIServer.Aplication.GraphQL.DTO;
using APIServer.Aplication.GraphQL.Extensions;
using APIServer.Aplication.GraphQL.Types;
using APIServer.Aplication.Queries;
using APIServer.Persistence;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedCore.Aplication.Interfaces;

namespace APIServer.Aplication.GraphQL.Queries
{
  /// <summary>
  ///  Webhook Queries
  /// </summary>
  [ExtendObjectType(OperationTypeNames.Query)]
  public class WebHookQueries
  {

    // Handled by downstream handler
    [UseConnection(typeof(WebHookType))]
    public async Task<Connection<GQL_WebHook>> WebhooksA(
    IResolverContext ctx,
    [Service] IMediator mediator)
    {
      var command = new GetWebHooks()
      {
        arguments = ctx.GetPaggingArguments()
      };

      var response = await mediator.Send(command);

      return response.connection;
    }

    // Handled by HC middelware
    [UseApiDbContextAttribute]
    [UsePaging(typeof(WebHookType))]
    [UseFiltering]
    public IQueryable<GQL_WebHook> WebhooksB(
    [Service] ICurrentUser current,
    [ScopedService] ApiDbContext context)
    {

      if (!current.Exist)
      {
        return null;
      }

      return context.WebHooks
      .AsNoTracking()
      .Select(e => new GQL_WebHook
      {
        ID = e.ID,
        WebHookUrl = e.WebHookUrl,
        ContentType = e.ContentType,
        IsActive = e.IsActive,
        LastTrigger = e.LastTrigger,
        ListeningEvents = e.HookEvents
      })
      .AsQueryable();
    }
  }
}