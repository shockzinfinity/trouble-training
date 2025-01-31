using System;
using APIServer.Aplication.GraphQL.DataLoaders;
using APIServer.Aplication.GraphQL.Extensions;
using APIServer.Aplication.GraphQL.Queries;
using APIServer.Aplication.GraphQL.Types;
using Aplication.GraphQL.DataLoaders;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Extensions;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCore.Aplication.GraphQL.Types;

namespace APIServer.Benchmark
{
  public static partial class ServiceExtension
  {
    private const string Endpoint_path = "/graphql";

    //--------------------------------------------------

    public static IServiceCollection AddGraphql(
        this IServiceCollection serviceCollection,
        IWebHostEnvironment env)
    {
      serviceCollection.AddGraphQLServer()
          .SetPagingOptions(
              new PagingOptions
              {
                IncludeTotalCount = true,
                MaxPageSize = 200
              })
          .ModifyRequestOptions(requestExecutorOptions =>
          {
            requestExecutorOptions.ExecutionTimeout = TimeSpan.FromMinutes(1);

            requestExecutorOptions.IncludeExceptionDetails = true;
          })
          .AllowIntrospection(true)

          .AddGlobalObjectIdentification()
          .AddQueryFieldToMutationPayloads()

          .AddHttpRequestInterceptor<IntrospectionInterceptor>()
          .TryAddTypeInterceptor<StreamTypeInterceptor>()

          .AddFiltering()
          .AddSorting()

          .AddQueryType<QueryType>()
              .AddTypeExtension<WebHookQueries>()

          .BindRuntimeType<DateTime, DateTimeType>()
          .BindRuntimeType<int, IntType>()
          .BindRuntimeType<long, LongType>()

          .AddType<BadRequestType>()
          .AddType<InternalServerErrorType>()
          .AddType<UnAuthorisedType>()
          .AddType<ValidationErrorType>()
          .AddType<BaseErrorType>()
          .AddType<UserDeactivatedType>()
          .AddType<BaseErrorInterfaceType>()
          .AddType<WebHookNotFoundType>()
          .AddType<WebHookRecordType>()
          .AddType<WebHookType>()
          .AddType<UserType>()

          .AddDataLoader<UserByIdDataLoader>()
          .AddDataLoader<WebHookByIdDataLoader>()
          .AddDataLoader<WebHookRecordByIdDataLoader>()

          .UseCustomPipeline();

      return serviceCollection;
    }

    //--------------------------------------------------

    public static IRequestExecutorBuilder UseCustomPipeline(
        this IRequestExecutorBuilder builder)
    {
      if (builder is null)
      {
        throw new ArgumentNullException(nameof(builder));
      }

      return builder
          .UseInstrumentations()
          .UseExceptions()
          .UseTimeout()
          .UseDocumentCache()
          .UseDocumentParser()
          .UseDocumentValidation()
          .UseRequest<StreamArgumentRewriteMiddelware>() // Temporary workeround !
          .UseOperationCache()
          .UseOperationComplexityAnalyzer()
          .UseOperationResolver()
          .UseOperationVariableCoercion()
          .UseOperationExecution();
    }

    //--------------------------------------------------

    public static GraphQLEndpointConventionBuilder MapGraphQLEndpoint(
        this IEndpointRouteBuilder builder)
    {
      var env = builder.ServiceProvider.GetService<IWebHostEnvironment>();

      return builder.MapGraphQL()
      .WithOptions(new GraphQLServerOptions
      {

        EnableSchemaRequests = env.IsDevelopment(),
        Tool = {
                    Enable = env.IsDevelopment(),
          }
      });
    }
  }
}