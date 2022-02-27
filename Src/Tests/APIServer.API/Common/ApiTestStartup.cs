using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using APIServer.Configuration;
using APIServer.Persistence;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SharedCore.Aplication.Interfaces;
using SharedCore.Aplication.Services;

namespace APIServer.API.IntegrationTests
{
  public class ApiTestStartup : APIServer.Startup
  {

    public static HttpMessageHandler BackChannelHandler { get; set; }

    public ApiTestStartup(
        IWebHostEnvironment environment,
        IConfiguration configuration)
        : base(configuration, environment)
    {

    }

    public override void ConfigureServices(IServiceCollection services)
    {

      services.AddControllers();

      services.AddHttpContextAccessor();

      services.AddPooledDbContextFactory<ApiDbContext>(
          (s, o) => o
              // .UseSqlite("DataSource=:memory:"));
              .UseInMemoryDatabase(Guid.NewGuid().ToString()));

      services.AddScoped<ICurrentUser, CurrentUser>();

      services.AddGraphql(Environment);

      services.AddMapper();

      services.AddTelemetryService(Configuration, out string source);

      services.AddMediatR();

      services.AddAuthentication("token")
      .AddJwtBearer("token", options =>
      {
        options.Authority = "https://identityserver";

        options.BackchannelHttpHandler = BackChannelHandler;

        options.MapInboundClaims = true;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
          ValidateAudience = false,
          ValidTypes = new[] { "at+jwt" },

          NameClaimType = "name",
          RoleClaimType = "role"
        };
      });

      services.AddAuthorization(options =>
      {
        options.AddPolicy("ApiCaller", policy =>
              {
                policy.RequireClaim("scope", "api");
              });
      });

      JwtSecurityTokenHandler.DefaultMapInboundClaims = true;

      services.AddSingleton(Serilog.Log.Logger);
    }

    public override void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IServiceProvider serviceProvider,
        IServiceScopeFactory scopeFactory)
    {

      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
      });

      app.Use(async (context, next) =>
      {
        var header = context.Request.Headers["Authorization"];

        // You can add aditional logging or breakepoints hire

        await next();
      });

      app.UseEnsureApiContextCreated(serviceProvider, scopeFactory);

      app.UseRouting();

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers()
                  .RequireAuthorization("ApiCaller");

        endpoints.MapGraphQL()
              .WithOptions(new GraphQLServerOptions
              {
                EnableSchemaRequests = env.IsDevelopment(),
                Tool = { Enable = false },
              });

        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller}/{action=Index}/{id?}");
      });

    }
  }
}