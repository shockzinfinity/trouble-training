using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace APIServer.Aplication.GraphQL.Extensions
{

  public class StreamRequestInterceptor : DefaultHttpRequestInterceptor
  {
    private readonly IWebHostEnvironment _env;

    public StreamRequestInterceptor(
        [Service] IWebHostEnvironment env
    )
    {
      _env = env;
    }

    public override ValueTask OnCreateAsync(HttpContext context,
        IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
      // This is part of separate workshop and is not presen hire!

      return base.OnCreateAsync(context, requestExecutor, requestBuilder,
          cancellationToken);
    }
  }
}
