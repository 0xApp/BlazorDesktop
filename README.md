[![GitHub license](https://img.shields.io/github/license/DotNetExtension/BlazorDesktop?style=for-the-badge&color=00bb00)](https://github.com/DotNetExtension/BlazorDesktop/blob/main/LICENSE.txt)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.0-4baaaa?style=for-the-badge)](CODE_OF_CONDUCT.md)
[![GitHub issues](https://img.shields.io/github/issues/DotNetExtension/BlazorDesktop?style=for-the-badge)](https://github.com/DotNetExtension/BlazorDesktop/issues)

# Blazor Desktop
Blazor Desktop allows you to create desktop apps using Blazor. Apps run inside of a .NET generic host with a WPF window thats fully managed using a similar template to Blazor WASM.
![preview](https://github.com/DotNetExtension/BlazorDesktop/assets/2308261/7d025b49-e2f8-4b07-a57d-35f9a319d859)

# Getting Started
The easiest way to get started with Blazor Desktop is to install the templates, you can do so using the dotnet cli as follows:

```powershell
dotnet new install BlazorDesktop.Templates::9.0.2
```

Once you have the templates installed, you can either create a new project from the template either in Visual Studio in the template picker:
![create](https://github.com/DotNetExtension/BlazorDesktop/assets/2308261/5ac50c95-9b90-4d5f-bb4f-7aa8a242d823)

Or, you can create a new project using the cli as follows:
```powershell
dotnet new blazordesktop -n MyBlazorApp
```

# Tips & Tricks
The Blazor Desktop template is set up very similar to the Blazor WASM template, you can see the `Program.cs` file here:

```csharp
using BlazorDesktop.Hosting;
using HelloWorld.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = BlazorDesktopHostBuilder.CreateDefault(args);

builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

if (builder.HostEnvironment.IsDevelopment())
{
    builder.UseDeveloperTools();
}

await builder.Build().RunAsync();
```

You can add root components just the same as well as add additional services your app may need just the same.

There are however a few additional APIs and services that have been made available to help when working in the context of a WPF window.

## Browser Dev Tools
Dev tools are setup for use out of the box in `Program.cs` with the following snippet:


```csharp
if (builder.HostEnvironment.IsDevelopment())
{
    builder.UseDeveloperTools();
}
```

This means you can press `CTRL` + `SHIFT` + `I` to open the dev tools so long as you are in a development environment.

It is highly advised to review over the [WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/), [Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-9.0), and [Blazor Hybrid](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/?view=aspnetcore-9.0) docs when using Blazor Desktop as it uses all three of those technologies combined.

## Window Utilities
The window can have most of its common configuration done through the `BlazorDesktopHostBuilder.Window` APIs before you build your app in `Program.cs`.

To change your window title:
```csharp
builder.Window.UseTitle("Hello");
```

Window size:
```csharp
builder.Window
    .UseWidth(1920)
    .UseHeight(1080)
    .UseMinWidth(1280)
    .UseMinHeight(720)
    .UseMaxWidth(2560)
    .UseMaxHeight(1440);
```

Disable window resizing:
```csharp
builder.Window.UseResizable(false);
```

Disable the window frame (allows you to use your own window chrome inside of Blazor):
```csharp
builder.Window.UseFrame(false);
```

And change your window icon (uses `favicon.ico` as the default, base directory is `wwwroot`):
```csharp
builder.Window.UseIcon("myicon.ico");
```

It is also possible to configure these values through `appsettings.json` like so:
```json
{
  "Window": {
    "Title": "Hello Blazor",
    "Height": 1080,
    "Width": 1920,
    "MinHeight": 720,
    "MinWidth": 1280,
    "MaxHeight": 1440,
    "MaxWidth": 2560,
    "Frame": false,
    "Resizable": false,
    "Icon": "hello.ico"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
> [!NOTE]
> The main window can be accessed through the `IWindowManager` service available in the DI container. Use `IWindowManager.MainWindow` to get a handle to it.
> The underlying `BlazorDesktopWindow` inherits from the WPF `Window` class, as such you use WPF apis to manipulate it. WPF documentation for the Window class can be found [here](https://learn.microsoft.com/en-us/dotnet/api/system.windows.window?view=windowsdesktop-9.0).
> Examples of usage can be found below.

## Custom Window Chrome & Draggable Regions
It is possible to make your own window chrome for Blazor Desktop apps. As an example base setup you could do the following:

Set up the window to have no frame in `Program.cs`:
```csharp
builder.Window.UseFrame(false);
```

Using the base template, if you were to edit `MainLayout.razor` and add a `-webkit-app-region: drag;` style to the top bar like so:
```razor
@inherits LayoutComponentBase

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4" style="-webkit-app-region: drag;">
            <a href="https://learn.microsoft.com/aspnet/core/" target="_blank">About</a>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>
```
The top bar becomes draggable, applying the `-webkit-app-region: drag;` property to anything will make it able to be used to drag the window.

In terms of handling things such as the close button, you can inject `IWindowManager` into any page and interact with the main window from there.

Here is an example changing `MainLayout.razor`:
```razor
@using BlazorDesktop.Services

@inherits LayoutComponentBase
@inject IWindowManager WindowManager

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4" style="-webkit-app-region: drag;">
            <a href="https://learn.microsoft.com/aspnet/core/" target="_blank">About</a>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

@code {
    void CloseWindow()
    {
        WindowManager.MainWindow.NativeWindow?.Close();
    }
}
```
To support fullscreen mode, you should also hide your custom window chrome when in fullscreen. You can check the current fullscreen status using the `IsFullscreen` property on the window. You can also monitor for it changing using the `OnFullscreenChanged` event.

## Changing Window Properties During Startup
It is possible to customize window startup behaviors for Blazor Desktop apps. As an example base setup you could do the following:

Using the base template, if you were to edit `MainLayout.razor` and inject `IWindowManager` you can have the window be maximized on launch using Blazor's `OnInitialized` lifecycle method:
```razor
@using BlazorDesktop.Services
@using System.Windows
@inherits LayoutComponentBase

@inject IWindowManager WindowManager

...

@code {
    protected override void OnInitialized()
    {
        if (WindowManager.MainWindow.NativeWindow is { } window)
        {
            window.WindowState = WindowState.Maximized;
        }
    }
}
```

## Multi-Window Support
Blazor Desktop supports opening multiple windows, each with its own independent Blazor component tree. There are two ways to create child windows: a **service-based API** for programmatic control and a **component-based API** for declarative Razor usage.

### Service-based API
Inject `IWindowManager` and call `OpenAsync` to open a new window programmatically:

```razor
@using BlazorDesktop.Services
@inject IWindowManager WindowManager

<button @onclick="OpenSettings">Open Settings</button>

@code {
    private async Task OpenSettings()
    {
        var handle = await WindowManager.OpenAsync<SettingsPanel>(options =>
        {
            options.Title = "Settings";
            options.Width = 600;
            options.Height = 400;
        });

        // React when the window is closed (user clicks X or closed programmatically)
        handle.Closed += (sender, args) =>
        {
            // Handle cleanup
        };
    }
}
```

You can also close a window programmatically:
```csharp
await WindowManager.CloseAsync(handle);
```

### Component-based API
Use the `<DesktopWindow>` component to open and close windows declaratively. The window opens when the component is rendered and closes when it is removed from the render tree:

```razor
@using BlazorDesktop.Components

<button @onclick="() => showSettings = !showSettings">Toggle Settings</button>

@if (showSettings)
{
    <DesktopWindow ComponentType="typeof(SettingsPanel)"
                   Title="Settings" Width="600" Height="400"
                   OnClosed="@(() => { showSettings = false; InvokeAsync(StateHasChanged); })" />
}

@code {
    private bool showSettings = false;
}
```

The `OnClosed` callback fires when the user closes the window (e.g. clicks the X button), allowing you to keep your state in sync.

### Window Options
Both APIs accept the same set of window options:

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Title` | `string?` | App name | Window title |
| `Width` | `int?` | `1366` | Window width in pixels |
| `Height` | `int?` | `768` | Window height in pixels |
| `MinWidth` | `int?` | `0` | Minimum width |
| `MinHeight` | `int?` | `0` | Minimum height |
| `MaxWidth` | `int?` | unlimited | Maximum width |
| `MaxHeight` | `int?` | unlimited | Maximum height |
| `Frame` | `bool?` | `true` | Use standard window frame |
| `Resizable` | `bool?` | `true` | Allow resizing |
| `Icon` | `string?` | `favicon.ico` | Icon path (relative to `wwwroot`) |

### IWindowManager Reference
The `IWindowManager` service is available in the DI container and provides:

| Member | Description |
|--------|-------------|
| `MainWindow` | Handle to the main application window |
| `Windows` | List of all currently open window handles |
| `OpenAsync<TComponent>(...)` | Open a new window with a Blazor component |
| `OpenAsync(Type, ...)` | Open a new window with a component type |
| `CloseAsync(handle)` | Close a child window |
| `WindowOpened` | Event fired when any window is opened |
| `WindowClosed` | Event fired when any window is closed |

### Behavior Notes
- **Child window ownership**: All child windows are owned by the main window, so they stack and minimize together following standard WPF behavior.
- **Shutdown policy**: Closing the main window closes the entire application and all child windows. Closing a child window only removes that window.
- **DI scoping**: Each window runs its own `BlazorWebView` which creates an independent Blazor DI scope. Scoped services are per-window; singleton services are shared across all windows.
- **Thread safety**: `IWindowManager` is safe to call from any thread. All WPF operations are automatically marshaled to the UI thread.
- **Component parameters**: You can pass parameters to child window root components via the `parameters` argument on `OpenAsync` or the `Parameters` property on `<DesktopWindow>`.

### Full Example
The `BlazorDesktop.Sample` project includes a working multi-window demo. Navigate to the **Multi-Window** page to try both APIs. The sample shows:

- Opening child windows with the service-based API via `IWindowManager.OpenAsync<T>()`
- Toggling child windows with the component-based API via `<DesktopWindow>`
- Each child window running its own independent counter
- Tracking the number of open windows in real time

## Issues
Under the hood, Blazor Desktop uses WebView2 which has limitations right now with composition. Due to this, if you disable the window border through the `Window.UseFrame(false)` API, the top edge of the window is unusable as a resizing zone for the window. However all the other corners and edges work.
