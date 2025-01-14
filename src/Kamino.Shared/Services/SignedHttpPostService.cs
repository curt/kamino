using System.Net.Http.Headers;
using System.Security.Cryptography;
using SevenKilo.HttpSignatures;

namespace Kamino.Shared.Services;

public class SignedHttpPostService(
    IHttpClientFactory httpClientFactory,
    LocalKeyProvider localKeyProvider,
    IdentifierProvider identifierProvider
)
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions =
        new(JsonSerializerDefaults.Web);

    public async Task PostAsync<T>(Uri uri, T payload)
    {
        using var httpClient = GetHttpClient();

        var bytes = JsonSerializer.SerializeToUtf8Bytes(payload, s_jsonSerializerOptions);
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/activity+json");
        var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
        request.Headers.Add("Date", DateTime.UtcNow.ToString("R"));
        request.Headers.Add("Digest", $"sha-256={Convert.ToBase64String(SHA256.HashData(bytes))}");
        var signatureRequest = new OutboundSignatureRequest(
            uri,
            HttpMethod.Post,
            request.Headers,
            identifierProvider
        );
        var signer = new Signature(localKeyProvider);
        var result = await signer.SignAsync(signatureRequest);
        request.Headers.Add("Signature", signer.SignatureComposed);
        await httpClient.SendAsync(request);
    }

    private HttpClient GetHttpClient(int timeout = 10)
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(timeout);

        return httpClient;
    }
}
