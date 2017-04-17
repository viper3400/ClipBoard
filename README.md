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

By default, your saved text snippets are stored in %APPDATA%\Clipboad\content.csv. This file will be created if it not exists, yet. If you like to use another file in another location you can pass the full filepath as first command line argument to ClipBoard.exe

``ClipBoard.exe D:\MyClipboardContent\content.csv``

You may also drag&drop your data file on ClipBoard.exe in Windows Explorer.

### Enjoy!
