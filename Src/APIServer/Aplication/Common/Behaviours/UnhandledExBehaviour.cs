using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using SharedCore.Aplication.Interfaces;
using SharedCore.Aplication.Payload;

namespace APIServer.Aplication.Shared.Behaviours
{

  /// <summary>
  /// UnhandledExBehaviour for MediatR pipeline
  /// </summary>
  /// <typeparam name="TRequest"></typeparam>
  /// <typeparam name="TResponse"></typeparam>
  public class UnhandledExBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
  {
    private readonly ICurrentUser _currentUserService;
    private readonly ILogger _logger;
    private readonly ITelemetry _telemetry;

    public UnhandledExBehaviour(
        ICurrentUser currentUserService,
        ILogger logger,
        ITelemetry telemetry)
    {
      _currentUserService = currentUserService;
      _logger = logger;
      _telemetry = telemetry;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next
    )
    {

      var activity = GetActivity(request);

      try
      {
        activity?.Start();

        // Continue in pipe
        return await next();

      }
      catch (Exception ex)
      {

        _telemetry.SetOtelError(ex);

        // In case it is Mutation Response Payload = handled as payload error union
        // By default all unexpected errors becomes InternalServerError
        if (SharedCore.Aplication.Shared.Common.IsSubclassOfRawGeneric(
            typeof(BasePayload<,>),
            typeof(TResponse))
        )
        {
          return Common.HandleBaseCommandException<TResponse>(ex);
        }
        else
        {

          if (!ex.Data.Contains("command_failed"))
          {
            ex.Data.Add("command_failed", true);
          }

          throw;
        }

      }
      finally
      {
        activity?.Stop();
        activity?.Dispose();
      }
    }

    private Activity GetActivity(TRequest request)
    {
      return _telemetry.AppSource.StartActivity(
          String.Format(
              "UnhandledExBehaviour: Request<{0}>",
              typeof(TRequest).FullName),
              ActivityKind.Server);
    }
  }
}