namespace Endure.Services;

public class TextToSpeechService : ITextToSpeechService
{
    private CancellationTokenSource? m_cts;
    private SpeechOptions m_options;

    public TextToSpeechService()
    {
        m_options = new SpeechOptions
        {
            Pitch = 1.5f,
            Volume = 0.5f
        };
    }

    public async Task SpeakNowDefaultSettingsAsync(string content)
    {
        m_cts = new CancellationTokenSource();
        await TextToSpeech.Default.SpeakAsync(content, m_options, m_cts.Token);
    }

    public void CancelSpeech()
    {
        if (m_cts?.IsCancellationRequested ?? true) return;

        m_cts.Cancel();
    }
}