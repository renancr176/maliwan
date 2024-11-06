using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.CategoryCommands;

public class DeleteCategoryCommand : Command
{
    public int Id { get; set; }
}