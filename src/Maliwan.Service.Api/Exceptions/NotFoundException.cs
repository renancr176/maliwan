using System.Net;

namespace Maliwan.Service.Api.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string message)
        : base(message, null, HttpStatusCode.NotFound)
    {
    }
}