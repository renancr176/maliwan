using MediatR;
using System.Runtime.Serialization;

namespace Maliwan.Domain.Core.Messages;

public abstract class Command<TResponse> : Message, IRequest<TResponse>
{
    [IgnoreDataMember]
    public DateTime Timestamp { get; private set; }

    [IgnoreDataMember]
    public string? RequestId { get; set; }

    protected Command()
    {
        Timestamp = DateTime.UtcNow;
    }
}

public abstract class Command : Command<bool>
{
}