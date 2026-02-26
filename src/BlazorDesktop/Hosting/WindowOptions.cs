// Licensed to the .NET Extension Contributors under one or more agreements.
// The .NET Extension Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace BlazorDesktop.Hosting;

/// <summary>
/// Per-window configuration options.
/// </summary>
public class WindowOptions
{
    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the window height.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the window width.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the minimum window height.
    /// </summary>
    public int? MinHeight { get; set; }

    /// <summary>
    /// Gets or sets the minimum window width.
    /// </summary>
    public int? MinWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum window height.
    /// </summary>
    public int? MaxHeight { get; set; }

    /// <summary>
    /// Gets or sets the maximum window width.
    /// </summary>
    public int? MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets whether the window uses a standard frame.
    /// </summary>
    public bool? Frame { get; set; }

    /// <summary>
    /// Gets or sets whether the window is resizable.
    /// </summary>
    public bool? Resizable { get; set; }

    /// <summary>
    /// Gets or sets the window icon path.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Creates a <see cref="WindowOptions"/> from the existing <see cref="IConfiguration"/> keys.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>A populated <see cref="WindowOptions"/>.</returns>
    public static WindowOptions FromConfiguration(IConfiguration config)
    {
        return new WindowOptions
        {
            Title = config.GetValue<string?>(WindowDefaults.Title),
            Height = config.GetValue<int?>(WindowDefaults.Height),
            Width = config.GetValue<int?>(WindowDefaults.Width),
            MinHeight = config.GetValue<int?>(WindowDefaults.MinHeight),
            MinWidth = config.GetValue<int?>(WindowDefaults.MinWidth),
            MaxHeight = config.GetValue<int?>(WindowDefaults.MaxHeight),
            MaxWidth = config.GetValue<int?>(WindowDefaults.MaxWidth),
            Frame = config.GetValue<bool?>(WindowDefaults.Frame),
            Resizable = config.GetValue<bool?>(WindowDefaults.Resizable),
            Icon = config.GetValue<string?>(WindowDefaults.Icon)
        };
    }
}
