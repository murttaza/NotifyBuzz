# NotifyBuzz

A Logi Options+ plugin that vibrates your MX Master 4 mouse whenever a Windows notification arrives.

- **Normal notifications** trigger a `knock` haptic
- **Urgent/alarm notifications** trigger an `angry_alert` haptic

## Prerequisites

- [Logi Options+](https://www.logitech.com/software/logi-options-plus.html) v1.95 or later
- MX Master 4 connected via Bluetooth or USB receiver
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11

## Build & Install

```bash
cd NotifyBuzzPlugin
dotnet build
```

The build automatically:
1. Compiles the plugin
2. Creates a `.link` file to register it with Logi Options+
3. Sends a reload command to the plugin service

Open Logi Options+ and enable **NotifyBuzz** under your MX Master 4's haptic plugins.

## First-Time Setup

Windows will prompt you to grant notification listener access the first time the plugin runs. You can also enable this manually:

**Windows Settings > System > Notifications > Notification access**

Make sure the Logi Plugin Service has access.

## Project Structure

```
NotifyBuzzPlugin/
├── src/
│   ├── NotifyBuzzPlugin.cs             # Main plugin (notification listener + haptic events)
│   ├── NotifyBuzzApplication.cs        # Application wrapper (required by SDK)
│   ├── Actions/
│   │   └── TestHapticCommand.cs        # Manual test button for development
│   ├── Helpers/
│   │   ├── PluginLog.cs
│   │   └── PluginResources.cs
│   ├── package/
│   │   ├── metadata/
│   │   │   ├── LoupedeckPackage.yaml   # Plugin config (HasHapticMapping)
│   │   │   └── Icon256x256.png
│   │   └── events/
│   │       ├── DefaultEventSource.yaml # Event definitions
│   │       └── extra/
│   │           └── eventMapping.yaml   # Waveform mappings
│   └── NotifyBuzzPlugin.csproj
└── NotifyBuzzPlugin.sln
```

## Available Waveforms

You can change the waveforms in `eventMapping.yaml`:

| Category | Waveforms |
|----------|-----------|
| Precision | `sharp_collision`, `damp_collision`, `subtle_collision`, `damp_state_change` |
| Progress | `sharp_state_change`, `completed`, `happy_alert`, `angry_alert`, `wave`, `square`, `firework`, `mad` |
| Incoming | `knock`, `ringing`, `jingle` |

## License

MIT
