using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SharedCore.Aplication.Core.Commands;
using SharedCore.Aplication.Interfaces;

namespace APIServer.Aplication.Notifications.WebHooks
{

  /// <summary>
  /// Base abstract class of all IssueNotify
  /// </summary>
  public abstract class WebHookBaseNotifi : BaseNotifi
  {
    public WebHookBaseNotifi()
    {
      ActivityId = Activity.Current.Id;
    }

    public long WebHookId { get; set; }

  }

  /// <summary>
  /// Shared handler for all notifications
  /// </summary>
  public class WebHookBaseNotifi_Handler : INotificationHandler<INotificationBase>
  {

    public WebHookBaseNotifi_Handler()
    {

    }

    public async Task Handle(INotificationBase notification, CancellationToken cancellationToken)
    {
      await Task.CompletedTask;

      return;
    }
  }
}