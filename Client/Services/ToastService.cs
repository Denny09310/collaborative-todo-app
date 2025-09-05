using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace Client.Services;

public class ToastService(IJSRuntime runtime) : IAsyncDisposable
{
    private const string Module = """
        data: text/javascript,
        export const show = (options) => Toastify(options).showToast();
        """;

    private readonly Lazy<Task<IJSObjectReference>> _task = new(() =>
        runtime.InvokeAsync<IJSObjectReference>("import", Module).AsTask());

    public async ValueTask DisposeAsync()
    {
        if (_task.IsValueCreated)
        {
            var module = await _task.Value;
            await module.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }

    public Task ErrorAsync(string message, ToastOptions? options = null)
    {
        options ??= new ToastOptions();
        options.ClassName = ToastStyles.Error;
        return ShowAsync(message, options);
    }

    public Task InfoAsync(string message, ToastOptions? options = null)
    {
        options ??= new ToastOptions();
        options.ClassName = ToastStyles.Info;
        return ShowAsync(message, options);
    }

    public Task SuccessAsync(string message, ToastOptions? options = null)
    {
        options ??= new ToastOptions();
        options.ClassName = ToastStyles.Success;
        return ShowAsync(message, options);
    }

    public Task WarningAsync(string message, ToastOptions? options = null)
    {
        options ??= new ToastOptions();
        options.ClassName = ToastStyles.Warning;
        return ShowAsync(message, options);
    }

    private async Task ShowAsync(string title, ToastOptions options)
    {
        options.Text = title;

        var module = await _task.Value;
        await module.InvokeVoidAsync("show", options);
    }
}

public class ToastOptions
{
    /// <summary>Optional custom CSS class for styling</summary>
    [JsonPropertyName("className")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClassName { get; set; }

    /// <summary>Whether the toast should close on click</summary>
    [JsonPropertyName("close")]
    public bool Close { get; set; } = true;

    /// <summary>Duration in ms before toast disappears</summary>
    [JsonPropertyName("duration")]
    public double Duration { get; set; } = 3000;

    /// <summary>Optional position ("left" | "right" | "center")</summary>
    [JsonPropertyName("gravity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Gravity { get; set; } = "top";

    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Position { get; set; } = "right";

    /// <summary>Text to be displayed in the toast</summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public static class ToastStyles
{
    public const string Error = "!bg-red-500 !text-white !font-medium !px-4 !py-2 !rounded !shadow-md !min-w-sm";
    public const string Info = "!bg-blue-500 !text-white !font-medium !px-4 !py-2 !rounded !shadow-md !min-w-sm";
    public const string Success = "!bg-green-500 !text-white !font-medium !px-4 !py-2 !rounded !shadow-md !min-w-sm";
    public const string Warning = "!bg-yellow-400 !text-black !font-medium !px-4 !py-2 !rounded !shadow-md !min-w-sm";
}
