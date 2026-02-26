// Licensed to the .NET Extension Contributors under one or more agreements.
// The .NET Extension Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using BlazorDesktop.Hosting;
using BlazorDesktop.Wpf;
using Microsoft.AspNetCore.Components;
using WpfDispatcher = System.Windows.Threading.Dispatcher;

namespace BlazorDesktop.Services;

/// <summary>
/// Internal implementation of <see cref="IWindowManager"/>.
/// </summary>
internal sealed class WindowManager : IWindowManager
{
    private readonly ConcurrentDictionary<string, DesktopWindowHandle> _windows = new();
    private readonly IServiceProvider _services;
    private readonly IWebHostEnvironment _environment;
    private DesktopWindowHandle? _mainWindow;
    private WpfDispatcher? _dispatcher;

    /// <inheritdoc/>
    public DesktopWindowHandle MainWindow => _mainWindow ?? throw new InvalidOperationException("The main window has not been registered yet.");

    /// <inheritdoc/>
    public IReadOnlyList<DesktopWindowHandle> Windows => _windows.Values.ToList().AsReadOnly();

    /// <inheritdoc/>
    public event EventHandler<DesktopWindowHandle>? WindowOpened;

    /// <inheritdoc/>
    public event EventHandler<DesktopWindowHandle>? WindowClosed;

    public WindowManager(IServiceProvider services, IWebHostEnvironment environment)
    {
        _services = services;
        _environment = environment;
    }

    /// <inheritdoc/>
    public Task<DesktopWindowHandle> OpenAsync<TComponent>(Action<WindowOptions>? configure = null, IDictionary<string, object?>? parameters = null) where TComponent : IComponent
    {
        return OpenAsync(typeof(TComponent), configure, parameters);
    }

    /// <inheritdoc/>
    public async Task<DesktopWindowHandle> OpenAsync(Type componentType, Action<WindowOptions>? configure = null, IDictionary<string, object?>? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(componentType);

        if (!typeof(IComponent).IsAssignableFrom(componentType))
        {
            throw new ArgumentException($"The type '{componentType.Name}' must implement {nameof(IComponent)} to be used as a root component.", nameof(componentType));
        }

        var dispatcher = _dispatcher ?? throw new InvalidOperationException("The WPF dispatcher has not been set. Ensure the main window is registered first.");

        var options = new WindowOptions();
        configure?.Invoke(options);

        var handle = new DesktopWindowHandle(Guid.NewGuid().ToString("N"), isMainWindow: false);

        var rootComponents = new RootComponentMappingCollection();
        if (parameters is not null)
        {
            rootComponents.Add(componentType, "#app", ParameterView.FromDictionary(parameters));
        }
        else
        {
            rootComponents.Add(componentType, "#app");
        }

        await dispatcher.InvokeAsync(() =>
        {
            var window = new BlazorDesktopWindow(_services, _environment, options, rootComponents);

            if (_mainWindow?.NativeWindow is not null)
            {
                window.Owner = _mainWindow.NativeWindow;
            }

            handle.NativeWindow = window;

            window.Closed += (_, _) => OnChildWindowClosed(handle);

            window.Show();
        });

        _windows.TryAdd(handle.Id, handle);
        WindowOpened?.Invoke(this, handle);

        return handle;
    }

    /// <inheritdoc/>
    public async Task CloseAsync(DesktopWindowHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle);

        if (handle.IsMainWindow)
        {
            throw new InvalidOperationException("The main window cannot be closed via IWindowManager. Use application shutdown instead.");
        }

        var dispatcher = _dispatcher ?? throw new InvalidOperationException("The WPF dispatcher has not been set.");

        await dispatcher.InvokeAsync(() =>
        {
            handle.NativeWindow?.Close();
        });
    }

    /// <summary>
    /// Registers the main window and stores the WPF dispatcher. Called by <see cref="BlazorDesktopService"/>.
    /// </summary>
    internal void RegisterMainWindow(BlazorDesktopWindow window, WpfDispatcher dispatcher)
    {
        _dispatcher = dispatcher;

        var handle = new DesktopWindowHandle("main", isMainWindow: true)
        {
            NativeWindow = window
        };

        _mainWindow = handle;
        _windows.TryAdd(handle.Id, handle);
    }

    private void OnChildWindowClosed(DesktopWindowHandle handle)
    {
        _windows.TryRemove(handle.Id, out _);
        handle.NativeWindow = null;
        handle.OnClosed();
        WindowClosed?.Invoke(this, handle);
    }
}
