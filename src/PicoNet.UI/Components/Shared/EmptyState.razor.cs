using Microsoft.AspNetCore.Components;

namespace PicoNet.UI.Components.Shared;

public partial class EmptyState
{
    [Parameter,EditorRequired]
    public string Message { get; set; } = string.Empty;

    [Parameter]
    public string? ActionLabel { get; set; }

    [Parameter]
    public EventCallback OnAction { get; set; }
}