using System.Reflection;
using APIServer.Aplication.Commands.Internall.Hooks;
using APIServer.Aplication.Commands.WebHooks;
using APIServer.Aplication.Shared.Behaviours;
using APIServer.Domain.Core.Models.Events;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Aplication.Services;

namespace APIServer.Configuration
{
  public static partial class ServiceExtension
  {
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {

      services.AddMediatR(cfg => cfg.Using<AppMediator>(), typeof(CreateWebHook).GetTypeInfo().Assembly);

      services.AddTransient<SharedCore.Aplication.Interfaces.IPublisher, Publisher>();

      services.AddValidatorsFromAssembly(typeof(CreateWebHookValidator).GetTypeInfo().Assembly);

      services.AddTransient<IRequestHandler<EnqueSaveEvent<WebHookCreated>, Unit>, EnqueSaveEventHandler<WebHookCreated>>();

      services.AddTransient<IRequestHandler<EnqueSaveEvent<WebHookUpdated>, Unit>, EnqueSaveEventHandler<WebHookUpdated>>();

      services.AddTransient<IRequestHandler<EnqueSaveEvent<WebHookRemoved>, Unit>, EnqueSaveEventHandler<WebHookRemoved>>();

      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingBehaviour<,>));

      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));

      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExBehaviour<,>));

      return services;
    }
  }
}