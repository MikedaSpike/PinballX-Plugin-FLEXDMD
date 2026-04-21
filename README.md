-----

# FlexDMD for PinballX
![intro-ezgif com-reverse](https://github.com/user-attachments/assets/4744a745-9010-4098-ae17-74de67dfa3ca)

This plugin was created as a high-performance workaround for long-standing issues with the XDMD API in PinballX.

### Why use this plugin?

If you have a real DMD (like PinDMDv3) and use PinballX's attract mode, you might have noticed that the DMD remains "locked" after launching a table, preventing PinMame or DMDext from displaying anything. Additionally, XDMD can cause PinballX to hang for several minutes during shutdown.

**FlexDMD V2.0** replaces the XDMD handling with the modern FlexDMD engine, providing a smoother experience, more features, and instant shutdowns.

-----

## Key Features in V2.0

  * **Complete Configuration UI:** No more "hardcoded" behavior. Fine-tune everything via the Plugin Manager.
  * **Advanced PINemHi Support:** Cycle through high scores and display earned badges (badges optimized for 256x64 panels).
  * **Custom DMD Clock:** Display a real-time clock (12h/24h) with adjustable intervals.
  * **HD Support:** Optimized for both standard (128x32) and HD (256x64 / ZeDMD) panels.
  * **Color Customization:** Full color picker support for DMD text.
  * **Challenge Countdown:** Integrated timer for the "PinballX Pinemhi Challenge Table Launcher."

-----

## Prerequisites

  * **FlexDMD 1.9 or higher:** [Download here](https://github.com/vbousquet/flexdmd/releases).
  * **Disable PinballX XDMD:** To prevent PinballX from initializing the DMD itself, edit `PinDMD.ini` in your PinballX directory (create it if it doesn't exist) and set a non-existent COM port:
    ```ini
    comport=COM7
    ```

-----

## Installation & Setup

1.  Copy `PBXFlexDMD.dll` from the zip file to your `PinballX\Plugins` directory.
2.  Run `PluginManager.exe` from your PinballX folder.
3.  Enable the **FlexDMD Plugin** by checking the box.
4.  **New in 2.0:** Click **Configure** to open the settings menu.
<img width="712" height="415" alt="image png 4c6ef71fb95ccedaae94c3197cacb66b" src="https://github.com/user-attachments/assets/7a2feea8-3abd-450c-a172-04277f818592" />

-----

## Media Handling

The plugin searches for media in your `Real DMD Videos` or `Real DMD Color Videos` folders in the following order:

**1. File Formats:** `.mp4`, `.avi`, `.wmv`, `.gif`, `.png`
**2. Search Logic:**

  * Exact Table Filename
  * Table Description
  * `- system -` placeholder
  * `\Media\Videos\No Real DMD Color.avi` (Default)

**Note on High Scores:** Unlike previous versions, V2.0 now fully supports displaying high scores (both PinballX native and PINemHi) with customizable scroll speeds and durations.

-----

## The Carousel Engine

The DMD display operates in a carousel:

1.  **Splash Screen:** Shows on startup (Standard vs HD mode).
2.  **Table Media:** Video or image loops.
3.  **PINemHi Badges:** (If enabled and supported by panel size).
4.  **High Scores:** Native PBX scores and PINemHi leaderboards.
5.  **Clock:** Can be set to interrupt the carousel at specific intervals.

-----

## Logging

A log file is created at `PinballX\Plugins\PBXFlexDMDDisplay.txt`. If you encounter issues, please check this file for debug information.

-----

## Credits

A huge thanks to [5cutters](https://github.com/5cutters) , the 'PinballX Plugin Wizard' and 'FlexDMD master'. His work on the statistics plugin and FlexDMD initialization was instrumental in the development of this project.

-----

### Support

If you have questions, feature requests, or need support, please reach out via the [GameEx Forums](https://forums.gameex.com/forums/topic/28447-plugin-flexdmd-real-dmd-video-player-version-13/).
