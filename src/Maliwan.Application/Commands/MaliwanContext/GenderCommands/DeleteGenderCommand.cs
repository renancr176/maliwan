using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.GenderCommands;

public class DeleteGenderCommand : Command
{
    public int Id { get; set; }
}