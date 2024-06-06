This plugin is created as work around for issues with the XDMD API that is used within PinballX. 
See : https://forums.gameex.com/forums/topic/27873-realdmd-still-in-use-by-pinballx-after-table-is-launched-when-attract-mode-was-active/ 
In short : I have a real DMD (PinDMDv3) and after PinballX running attract mode (screensaver  mode) in PinballX and launches a table that uses the DMD (via Pinmame, or DMDext) the display is still in use and nothing is showed during game.
Exiting the game will sometimes release the DMD again and next launch of table will sometimes works fine. Or I need to relaunch PinballX.

Side effect is that exiting PinballX could take up minutes to close the XDMD dll.

This plugin will use FLEXDMD (as this is part of Visual Pinball now) and will show the videos from your realDMD directory.

Pre-requests for this plugin :

Working Flex DMD 1.9 or higher (https://github.com/vbousquet/flexdmd/actions For non public version, you need to login to Github)
Turn off XDMD to initialize the real DMD during start
This can be accomplished, by editing the PinDMD.ini in your PinballX directory (if not exists create the file) 
add/modify the next line :
comport=COM7
(the comport bust be a non existing comport of your real DMD)

 

Copy from the attached zip file the PBXFlexDMD.dll to your PinballX\Plugin directory

Starts from your PinballX folder (in your start menu) the option Plugins (or from your PinballX directory PluginManager.exe)
![image](https://github.com/MikedaSpike/PinballX-Plugin-FLEXDMD/assets/48748234/79679c32-4a64-40c6-9f6f-37d4c131d523)
 

From the Plugins program, enable FlexDMD Plugin by checking the checkbox and click exit (nothing to configure)

![image](https://github.com/MikedaSpike/PinballX-Plugin-FLEXDMD/assets/48748234/4ecb6525-cf5e-44fb-9db4-81114df79932)


Launch PinballX 
If FlexDMD is configured correctly you will see on your DMD display :

![image](https://github.com/MikedaSpike/PinballX-Plugin-FLEXDMD/assets/48748234/0accc0ca-648b-4e65-8266-a67278704210)


When PinballX is started , you will see the corresponding video for your table.

The videos are searched in the following order :

Extension in priority:
MP4
AVI
WMV
GIF
PNG
Name :
Filename (tablename)
Table description
- system -
\Media\Videos\No Real DMD Color.avi
This means it will first search for the exact filename (tablename) with extension .MP4. If not found, it will search for the AVI, still not found for a WMV file.
Not matched, it will do the same on table description (with the extension).
Still no  match, it will check if there is a file - system -.MP4 (avi and WMV)  
Still no match , it will display the default \Media\Videos\No Real DMD Color.avi

The same as PinballX does, the plugin will look in your XML to the  HideDMD. If this is set to false, it will display the current video during game play.
As I could not get a good scrolling with FlexDMD, the highscores check in settings.exe will be ignored by now.

In the plugin directory a log file will be created : PBXFlexDMDDisplay.txt
The log file will show all the events that are happening.

If you got any question, requests or need support, just ask and I'm willing to help.

Most important thing: A big thanks to @scutters who is the 'PinballX Plugin Wizard' and the overal 'FLEXDMD master'.
I used parts of his statistics plugin for this plugin, as I was too lazy to do the initialize of FLEXDMD.
Thanks buddy !
