ClipBoard
=========

[![](https://img.shields.io/github/downloads/ralphite/ClipBoard/total.svg)](https://github.com/ralphite/ClipBoard/releases)

Cannot find a perfect clipboard manager so I'm writing my own. This 
tool is written with .Net 4.5 and runs on Windows.

The tool monitors system keyboard events and copies **text** from system 
clipboard when a "Copy" is performed.

### Usage

- Click on an item to copy the text to system clipboard and trigger `Ctrl+V`
- Right click an item to save it to the frequently used items list on the top
- Right click and click remove to remove the selected item
- Minimize to hide the tool to system tray
- ``Ctrl+```(ctrl+backtick) or Ctrl+Space to bring the tool up
- Clicking on an item also hide the tool to system tray


![screenshot](https://raw.githubusercontent.com/MrCull/ClipBoard/base/Screenshot/ClipBoard.png)

### Data Storage

By default, your saved text snippets are stored in %APPDATA%\Clipboard\content.csv. This file will be created if it not exists, yet. If you like to use another file in another location see *Configuration*.

### RunOnStartup

You can configure ClipBoard Manager start on Windows startup. See *Configuration*.

### Configuration

To open the configuration dialog right click on the window minimize icon (top right). The configuration dialog will show. By default settings are stored in  %APPDATA%\Clipboard\clipboard.settings. This file will be created if it not exists. If you like to use another location and file you can pass it as the first parameter when calling ClipBoard.exe

``ClipBoard.exe D:\MyClipboard\custom.settings``

**Note:** This is a breaking change from former versions. First command line argmunent has been used to select a differnt content file. If you like to continue to use a content file in a custom location you now have to set this via configuration dialog.

#### Option "ContentFile"
*Default:  %APPDATA%\Clipboard\content.csv*

Path and filename of the file where your ClipBoard content will be saved.

#### Option "MaxItemsInFrequentList"
*Default: 10*

Number of items which are shown in the frequent list. Set this to 0 to disable this feature.

#### Option "HotKey"
*Default: Space*

CTRL + HotKey will open ClipBoard Manager window when minimized. See https://msdn.microsoft.com/de-de/library/system.windows.forms.keys(v=vs.110).aspx for available keys (case sensitive!)

**Note:** CTRL key is given at this time and not configurable. You may only choose the additional key.

#### Option "RunOnStartup"
*Default: false*

Set true if Clipboard Manager should run on Windows startup.

#### Option "MaxCopyTextLength"
*Default: false*
Text with more chars than this value won't be handled by ClipBoard.

### Enjoy!
