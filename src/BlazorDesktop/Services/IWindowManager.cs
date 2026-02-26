// Licensed to the .NET Extension Contributors under one or more agreements.
// The .NET Extension Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BlazorDesktop.Hosting;
using Microsoft.AspNetCore.Components;

namespace BlazorDesktop.Services;

/// <summary>
/// Manages multiple desktop windows. Safe to call from any thread.
/// </summary>
public interface IWindowManager
{
    /// <summary>
    /// Gets the main application window handle.
    /// </summary>
    DesktopWindowHandle MainWindow { get; }

    /// <summary>
    /// Gets all currently open window handles.
    /// </summary>
    IReadOnlyList<DesktopWindowHandle> Windows { get; }

    /// <summary>
    /// Opens a new window hosting the specified component.
    /// </summary>
    /// <typeparam name="TComponent">The root Blazor component for the window.</typeparam>
    /// <param name="configure">Optional configuration for window options.</param>
    /// <param name="parameters">Optional parameters to pass to the root component.</param>
    /// <returns>A handle to the opened window.</returns>
    Task<DesktopWindowHandle> OpenAsync<TComponent>(Action<WindowOptions>? configure = null, IDictionary<string, object?>? parameters = null) where TComponent : IComponent;

    /// <summary>
    /// Opens a new window hosting the specified component type.
    /// </summary>
    /// <param name="componentType">The root Blazor component type for the window.</param>
    /// <param name="configure">Optional configuration for window options.</param>
    /// <param name="parameters">Optional parameters to pass to the root component.</param>
    /// <returns>A handle to the opened window.</returns>
    Task<DesktopWindowHandle> OpenAsync(Type componentType, Action<WindowOptions>? configure = null, IDictionary<string, object?>? parameters = null);

    /// <summary>
    /// Closes the specified window.
    /// </summary>
    /// <param name="handle">The window handle to close.</param>
    Task CloseAsync(DesktopWindowHandle handle);

    /// <summary>
    /// Occurs when a window is opened.
    /// </summary>
    event EventHandler<DesktopWindowHandle>? WindowOpened;

    /// <summary>
    /// Occurs when a window is closed.
    /// </summary>
    event EventHandler<DesktopWindowHandle>? WindowClosed;
}
