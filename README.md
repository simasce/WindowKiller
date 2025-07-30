# WindowKiller

**WindowKiller** is a utility for Windows that allows you to visually target and terminate running processes by clicking on their windows.<br> 
It provides a transparent, full-screen overlay that highlights the window under your mouse cursor and displays its process name and current mouse coordinates.<br>
When you click on a window, it prompts for confirmation and then kills the targeted process.

## Features

- **Visual Targeting:** Highlights the window under the mouse cursor with a red rectangle.
- **Process Info:** Displays the process name (`.exe`) and current mouse coordinates at the bottom right.
- **Transparent Overlay:** Uses a transparent, borderless, topmost form that covers the entire screen.
- **Safe Termination:** Prompts for confirmation before killing the process.
- **Smooth UI:** Double-buffered drawing for flicker-free visuals.
- **No Taskbar Icon:** The overlay does not appear in the Windows taskbar.

## How It Works

1. Launch the application. Administrator rights are highly suggested.
2. The application will reside in your notification bar.
3. Start the overlay either by pressing `Ctrl+Home` or clicking on the taskbar icon directly.
4. The screen is covered by a transparent overlay.
5. Move your mouse to highlight the window you want to target.
6. The process name and mouse coordinates are shown at the bottom right.
7. Click to select the window. A confirmation dialog appears.
8. Confirm to terminate the process. The overlay closes automatically.

Note: You can cancel the overlay any time by pressing `ESC` or `Right Mouse` button.

To exit the program, right-click the application icon in the navigation bar and select the option `Quit`.

## Requirements

- **.NET 8.0** or later
- **Windows OS**
- Visual Studio 2022 (for building from source)

## Running the project
1. Clone the repository
2. Open `WindowKiller.sln` in Visual Studio 2022.
3. Build the solution (`Ctrl+Shift+B`).
4. Run the project (`F5`).

## Building the project

1. Clone the repository.
2. Open a terminal in the project directory.
3. Run the following command to build and publish a framework-dependent, single-file executable:
`dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true`
4. The published files will be in the `bin\Release\net8.0-windows\win-x64\publish` directory.
5. Run `WindowKiller.exe` from the publish directory. Administrator rights are highly suggested.

## Code Overview

- **Form1.cs:** Main overlay logic, drawing, and process termination.
- **WindowData:** Holds information about the targeted window and its process.
- **WindowTargetParser:** Responsible for detecting the window under the cursor.
- **GlobalKeyboardHook:** Responsible for handling hotkeys such as `Ctrl+Home` and `ESC`.
- **MainWindowHandler:** Handles form opening and application icon retrieval.

## Customization

- **Overlay Color:** Change `Color.LightSalmon` in `Form1.cs` for a different transparency key.
- **Font/Style:** Modify the font or drawing logic in `OnPaint` for custom visuals.

## Safety

- The application prompts before terminating any process.
- Use with caution: Killing system or critical processes may destabilize your system.

## License

This project is licensed under the [MIT License](LICENSE.txt).

## Disclaimer

This tool is intended for advanced users. Use at your own risk.