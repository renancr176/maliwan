using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.BrandCommands;

public class DeleteBrandCommand : Command
{
    public int Id { get; set; }
}