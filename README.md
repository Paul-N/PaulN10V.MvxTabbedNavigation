# PaulN10V.MvxTabbedNavigation

Multiple backstack navigation for MvvmCross.

## Overview

This library adds support for multiple backstack navigation to the MvvmCross framework. Recent versions of Android JetPack libraries provide this feature out of the box, but MvvmCross requires custom presenters and presentation hints to support this functionality.

The library enables each tab in a tabbed navigation to maintain its own backstack, providing a native user experience on both **iOS** and **Android** platforms.

## Features

- ✅ Multiple backstack support per tab
- ✅ Tab-specific navigation stacks
- ✅ Master/detail navigation patterns
- ✅ Cross-platform (iOS and Android)
- ✅ MvvmCross 9.x and 10.x compatible
- ✅ Single-project support (.NET Multi-platform App UI)

## Requirements

- .NET 8.0 or .NET 10.0
- MvvmCross 9.x or 10.x
- Android SDK 23+ (Minimum), 30+ (Target)
- iOS 14.2+

## Installation

This package is published to **GitHub Packages**, not nuget.org. GitHub Packages requires authentication to consume a feed even for public packages, so add an authenticated source first (a [personal access token](https://github.com/settings/tokens) with `read:packages` scope is enough):

```bash
dotnet nuget add source --username <YOUR_GITHUB_USERNAME> --password <YOUR_GITHUB_PAT> --store-password-in-clear-text --name github "https://nuget.pkg.github.com/Paul-N/index.json"
```

Then install the package:

```bash
dotnet add package PaulN10V.MvxTabbedNavigation --source github
```

## Usage

### Android Setup

In your `MainActivity.cs`:

```csharp
[Activity(Theme = "@style/MainTheme")]
public class MainActivity : MvxAndroidApplication<App, TabbedViewPresenter>
{
    public MainActivity(IntPtr handle, JniHandleOwnership ownership) 
        : base(handle, ownership) { }
}
```

### iOS Setup

In your `AppDelegate.cs`:

```csharp
[Register("AppDelegate")]
public class AppDelegate : MvxApplicationDelegate<App, TabbedViewPresenter>
{
    public override UIWindow Window { get; set; }
}
```

### Creating Tabs

Use the `TabPresentation` attribute on your view models:

```csharp
[TabPresentation(TabTitle = "Home", TabIcon = "home")]
public class HomeViewModel : MvxViewModel
{
    // Your tab content
}
```

### Presentation Hints

The library provides custom presentation hints:

- `ClearStackPresentationHint` - Clears the current tab's backstack

```csharp
await navigationService.ChangePresentation(new ClearStackPresentationHint());
```

## Demo

See the **PaulN10V.MvxTabbedNavigation.Demo** project for a complete example showing:

- Tabbed navigation with multiple tabs
- Tab-specific backstacks
- Master/detail navigation
- Tab icons and titles

## Architecture

The library uses custom presenters that extend MvvmCross's base presenters:

- `TabbedViewPresenter` (Android) - Extends `MvxAndroidViewPresenter`
- `TabbedViewPresenter` (iOS) - Extends `MvxIosViewPresenter`

Each presenter manages separate backstacks per tab and handles tab selection changes.

## Building

```bash
dotnet build
```

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## Other information

For XCode 26.3 (last XCode runnable on macOS 15) use workload:
```bash
sudo dotnet workload install ios maccatalyst maui android --version 10.0.202  
```

How to solve mess with JDK (path will be /Library/Java/JavaVirtualMachines/microsoft-21.jdk/Contents/Home):
```bash
brew install microsoft-openjdk@21 
```

## License

This project is licensed under the MIT License.
