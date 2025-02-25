using Microsoft.AspNetCore.Http;
using SevenKilo.HttpSignatures;

namespace Kamino.Shared.Services;

public class InboundVerificationRequest(IHttpContextAccessor accessor) : IVerificationRequest
{
    private HttpRequest? Request => accessor.HttpContext?.Request;

    public string Signature => Request!.Headers["signature"].FirstOrDefault() ?? string.Empty;

    public IEnumerable<string> GetHeaderValues(string key)
    {
        return Request!.Headers[key];
    }

    public Result Preverify(
        SignatureModel signatureModel,
        IEnumerable<KeyValuePair<string, string>> headerPairs
    )
    {
        return new Result();
    }
}
