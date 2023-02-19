using System.Diagnostics;
using System.Text.Json;
using Endure.Models;

namespace Endure.Services;

public class MemoService : IMemoService
{
    private readonly HttpClient m_client;
    private readonly JsonSerializerOptions m_jsonSerializerOptions;

    public List<Card>? Cards { get; private set; }

    public MemoService(IHttpsClientHandlerService service)
    {
#if DEBUG
        var handler = service.GetPlatformMessageHandler();
        m_client = handler != null ? new HttpClient(handler) : new HttpClient();
#else
        m_client = new HttpClient();
#endif
        m_jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<List<Card>?> GetTasksAsync()
    {
        Cards = new List<Card>();

        var uri = new Uri(string.Format(Constants.ApiUrl, "cards"));

        try
        {
            var response = await m_client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Cards = JsonSerializer.Deserialize<List<Card>>(content, m_jsonSerializerOptions);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(@"\tERROR {0}", e.Message);
        }

        return Cards;
    }
}