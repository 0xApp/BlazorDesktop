// Licensed to the .NET Extension Contributors under one or more agreements.
// The .NET Extension Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BlazorDesktop.Wpf;

namespace BlazorDesktop.Hosting;

/// <summary>
/// A lightweight, thread-safe handle to a managed desktop window.
/// </summary>
public sealed class DesktopWindowHandle
{
    /// <summary>
    /// Gets the unique identifier for this window.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets whether this is the main application window.
    /// </summary>
    public bool IsMainWindow { get; }

    /// <summary>
    /// Gets the native WPF window. Internal use only.
    /// </summary>
    internal BlazorDesktopWindow? NativeWindow { get; set; }

    /// <summary>
    /// Occurs when the window is closed.
    /// </summary>
    public event EventHandler? Closed;

    /// <summary>
    /// Creates a new <see cref="DesktopWindowHandle"/>.
    /// </summary>
    /// <param name="id">The unique window identifier.</param>
    /// <param name="isMainWindow">Whether this is the main window.</param>
    internal DesktopWindowHandle(string id, bool isMainWindow)
    {
        Id = id;
        IsMainWindow = isMainWindow;
    }

    /// <summary>
    /// Raises the <see cref="Closed"/> event.
    /// </summary>
    internal void OnClosed()
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }
}
