using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.SubcategoryCommands;

public class DeleteSubcategoryCommand : Command
{
    public int Id { get; set; }
}