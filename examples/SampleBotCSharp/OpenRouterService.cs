using System.Text;
using System.Text.Json;

namespace SampleBotCSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public class OpenRouterService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://openrouter.ai/api/v1/chat/completions";

    // История сообщений (одна сессия)
    private readonly List<ChatMessage> _history = new();
    private readonly Lock _historyLock = new();

    // Настройки
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly int _maxHistoryTokens = 4000;

    public OpenRouterService(string apiKey, string appName = "OpenRouterClient", string referer = null)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        if (!string.IsNullOrEmpty(referer))
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", referer);

        _httpClient.DefaultRequestHeaders.Add("X-Title", appName);

        _jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    // Модель сообщения
    public class ChatMessage
    {
        [JsonPropertyName("role")] public string Role { get; set; } // "system", "user", "assistant"

        [JsonPropertyName("content")] public string Content { get; set; }

        [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавить сообщение в историю
    /// </summary>
    private void AddMessage(string role, string content)
    {
        lock (_historyLock)
        {
            _history.Add(new ChatMessage
            {
                Role = role,
                Content = content,
                Timestamp = DateTime.UtcNow
            });

            // Подрезаем историю если нужно
            //TrimHistory();
        }
    }

   
    /// <summary>
    /// Установить system prompt
    /// </summary>
    public void SetSystemPrompt(string prompt)
    {
        lock (_historyLock)
        {
            // Удаляем все существующие system сообщения
            _history.RemoveAll(m => m.Role == "system");

            if (!string.IsNullOrEmpty(prompt))
            {
                _history.Insert(0, new ChatMessage
                {
                    Role = "system",
                    Content = prompt,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }

    /// <summary>
    /// Очистить историю (сохраняет system prompt)
    /// </summary>
    public void ClearHistory()
    {
        lock (_historyLock)
        {
            var systemMessage = _history.Find(m => m.Role == "system");
            _history.Clear();

            if (systemMessage != null)
                _history.Add(systemMessage);
        }
    }

    /// <summary>
    /// Отправить сообщение с учетом истории
    /// </summary>
    public async Task<string> SendMessageAsync(
        string message,
        string model = "openai/gpt-3.5-turbo",
        int maxTokens = 1000,
        double temperature = 0.7,
        CancellationToken cancellationToken = default)
    {
        // Добавляем сообщение пользователя в историю
        AddMessage("user", message);

        // Получаем копию истории для запроса
        List<ChatMessage> historyCopy;
        lock (_historyLock)
        {
            historyCopy = new List<ChatMessage>(_history.Count);
            foreach (var msg in _history)
            {
                historyCopy.Add(new ChatMessage
                {
                    Role = msg.Role,
                    Content = msg.Content
                });
            }
        }

        // Формируем запрос
        var request = new
        {
            model,
            messages = historyCopy.ConvertAll(m => new
            {
                role = m.Role,
                content = m.Content
            }),
            max_tokens = maxTokens,
            temperature
        };

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Отправляем запрос
        var response = await _httpClient.PostAsync(ApiUrl, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(responseJson);

        var assistantMessage = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        // Добавляем ответ ассистента в историю
        AddMessage("assistant", assistantMessage);

        return assistantMessage;
    }

    /// <summary>
    /// Сохранить историю в файл
    /// </summary>
    public async Task SaveAsync(string filePath, CancellationToken cancellationToken = default)
    {
        List<ChatMessage> historyCopy;
        lock (_historyLock)
        {
            historyCopy = new List<ChatMessage>(_history.Count);
            foreach (var msg in _history)
            {
                historyCopy.Add(new ChatMessage
                {
                    Role = msg.Role,
                    Content = msg.Content,
                    Timestamp = msg.Timestamp
                });
            }
        }

        var json = JsonSerializer.Serialize(historyCopy, _jsonOptions);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    /// <summary>
    /// Загрузить историю из файла
    /// </summary>
    public async Task LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            return;

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        var loadedHistory = JsonSerializer.Deserialize<List<ChatMessage>>(json, _jsonOptions);

        if (loadedHistory == null)
            return;

        lock (_historyLock)
        {
            _history.Clear();
            _history.AddRange(loadedHistory);
        }
    }

    /// <summary>
    /// Получить историю как текст
    /// </summary>
    public string GetHistoryAsText(bool includeTimestamps = true)
    {
        List<ChatMessage> historyCopy;
        lock (_historyLock)
        {
            historyCopy = new List<ChatMessage>(_history);
        }

        var sb = new StringBuilder();
        foreach (var message in historyCopy)
        {
            if (includeTimestamps)
            {
                sb.AppendLine($"[{message.Timestamp:HH:mm:ss}] {message.Role.ToUpper()}:");
            }
            else
            {
                sb.AppendLine($"{message.Role.ToUpper()}:");
            }

            sb.AppendLine(message.Content);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Получить количество сообщений в истории
    /// </summary>
    public int GetHistoryCount()
    {
        lock (_historyLock)
        {
            return _history.Count;
        }
    }
}