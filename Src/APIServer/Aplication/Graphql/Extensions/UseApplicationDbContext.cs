using System.Reflection;
using System.Runtime.CompilerServices;
using APIServer.Persistence;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APIServer.Aplication.GraphQL.Extensions
{

  public class UseApiDbContextAttribute : UseDbContextAttribute
  {
    public UseApiDbContextAttribute(
        [CallerLineNumber] int order = 0) : base(typeof(ApiDbContext))
    {
      Order = order;
    }

    public override void OnConfigure(
        IDescriptorContext context,
        IObjectFieldDescriptor descriptor,
        MemberInfo member)
    {
      descriptor.UseDbContext<ApiDbContext>();
    }
  }

  public static class ObjectFieldDescriptorExtensions
  {
    public static IObjectFieldDescriptor UseDbContext<TDbContext>(
    this IObjectFieldDescriptor descriptor)
    where TDbContext : DbContext
    {
      return descriptor.UseScopedService<TDbContext>(
          create: s => s.GetRequiredService<IDbContextFactory<TDbContext>>().CreateDbContext(),
          disposeAsync: (s, c) => c.DisposeAsync());
    }
  }


  // public class UseApiDbContextAttribute : ObjectFieldDescriptorAttribute
  // {
  //     public override void OnConfigure(
  //         IDescriptorContext context,
  //         IObjectFieldDescriptor descriptor,
  //         MemberInfo member)
  //     {
  //         descriptor.UseDbContext<ApiDbContext>();
  //     }
  // }

}