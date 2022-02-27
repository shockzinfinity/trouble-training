using System.Threading;
using System.Threading.Tasks;
using APIServer.Domain.Core.Models.WebHooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace APIServer.Aplication.Interfaces
{

  /// <summary>Main DBContext Interface </summary>
  public interface IApiDbContext
  {
    DbSet<WebHook> WebHooks { get; set; }

    DbSet<WebHookRecord> WebHooksHistory { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    DatabaseFacade Database { get; }
  }
}
