using System.Threading.Tasks;
using MediatR;
using SharedCore.Aplication.Services;

namespace SharedCore.Aplication.Interfaces
{

  public interface ICommandHandler
  {

    Task<Unit> ExecuteCommand(MediatorSerializedObject mediatorSerializedObject);

    Task ExecuteCommand(string reuest);
  }
}