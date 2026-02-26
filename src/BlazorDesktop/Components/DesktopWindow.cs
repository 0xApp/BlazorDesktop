// Licensed to the .NET Extension Contributors under one or more agreements.
// The .NET Extension Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BlazorDesktop.Hosting;
using BlazorDesktop.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorDesktop.Components;

/// <summary>
/// A Blazor component that manages a desktop window declaratively.
/// When rendered, opens a new window. When removed from the render tree, closes it.
/// </summary>
public sealed class DesktopWindow : ComponentBase, IAsyncDisposable
{
    [Inject]
    private IWindowManager WindowManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the root component type for the window.
    /// </summary>
    [Parameter, EditorRequired]
    public Type ComponentType { get; set; } = default!;

    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the window width.
    /// </summary>
    [Parameter]
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the window height.
    /// </summary>
    [Parameter]
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the minimum window width.
    /// </summary>
    [Parameter]
    public int? MinWidth { get; set; }

    /// <summary>
    /// Gets or sets the minimum window height.
    /// </summary>
    [Parameter]
    public int? MinHeight { get; set; }

    /// <summary>
    /// Gets or sets the maximum window width.
    /// </summary>
    [Parameter]
    public int? MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum window height.
    /// </summary>
    [Parameter]
    public int? MaxHeight { get; set; }

    /// <summary>
    /// Gets or sets whether the window uses a standard frame.
    /// </summary>
    [Parameter]
    public bool? Frame { get; set; }

    /// <summary>
    /// Gets or sets whether the window is resizable.
    /// </summary>
    [Parameter]
    public bool? Resizable { get; set; }

    /// <summary>
    /// Gets or sets the window icon path.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets optional parameters to pass to the root component.
    /// </summary>
    [Parameter]
    public IDictionary<string, object?>? Parameters { get; set; }

    /// <summary>
    /// Called when the window is closed (either programmatically or by the user).
    /// </summary>
    [Parameter]
    public EventCallback OnClosed { get; set; }

    private DesktopWindowHandle? _handle;
    private bool _disposed;

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _handle = await WindowManager.OpenAsync(ComponentType, options =>
        {
            options.Title = Title;
            options.Width = Width;
            options.Height = Height;
            options.MinWidth = MinWidth;
            options.MinHeight = MinHeight;
            options.MaxWidth = MaxWidth;
            options.MaxHeight = MaxHeight;
            options.Frame = Frame;
            options.Resizable = Resizable;
            options.Icon = Icon;
        }, Parameters);

        _handle.Closed += OnHandleClosed;
    }

    private async void OnHandleClosed(object? sender, EventArgs e)
    {
        if (_handle is not null)
        {
            _handle.Closed -= OnHandleClosed;
        }

        await InvokeAsync(async () =>
        {
            if (OnClosed.HasDelegate)
            {
                await OnClosed.InvokeAsync();
            }
        });
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_handle is not null)
        {
            _handle.Closed -= OnHandleClosed;

            if (_handle.NativeWindow is not null)
            {
                await WindowManager.CloseAsync(_handle);
            }

            _handle = null;
        }
    }
}
