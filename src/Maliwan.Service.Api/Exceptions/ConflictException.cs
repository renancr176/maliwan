using System.Net;

namespace Maliwan.Service.Api.Exceptions;

public class ConflictException : CustomException
{
    public ConflictException(string message)
        : base(message, null, HttpStatusCode.Conflict) { }
}