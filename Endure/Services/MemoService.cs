using System.Diagnostics;
using System.Text.Json;
using Endure.Models;

namespace Endure.Services;

public class MemoService : IMemoService
{
    private readonly HttpClient m_client;
    private readonly JsonSerializerOptions m_jsonSerializerOptions;

    public List<Memo>? Memos { get; private set; }

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

    public async Task<List<Memo>?> GetMemoAsync()
    {
        Memos = new List<Memo>();

        var uri = new Uri($"{Constants.ApiUrl}/memo");

        try
        {
            var response = await m_client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Memos = JsonSerializer.Deserialize<List<Memo>>(content, m_jsonSerializerOptions);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(@"\tERROR {0}", e.Message);
        }

        return Memos;
    }

    public Task SaveMemoAsync(Memo memo, bool isNew)
    {
        var uri = new Uri($"{Constants.ApiUrl}/memo");
        throw new NotImplementedException();
    }

    public Task DeleteMemoAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}