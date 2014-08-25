JC2MapViewer 0.3.4
==================

About
-----
JC2MapViewer is a map viewer for Just Cause 2, written in C#. It loads a Just Cause 2 save game and displays missed resource/chaos items, required for 100% completion.

The source code of 0.3 was written using Visual Studio 2008 and was migrated to Visual Studio 2012, starting with 0.3.1.

This program was originally written by DerPlaya78 using BruTile as the tiling library. Since the development stopped in 2010 with 0.3 as the final version,
bugs remained and some have been fixed in this version. The JC2.Save library contains some additional information about Just Cause 2 save games (statistics, mission
state, etc.), that is not used by the viewer, so anyone interested can use it in their programs.

Instructions
------------
1. Click on "Load..." and browse to %ProgramFiles%\Steam\userdata\[steam_id]\8190\remote.
2. Open the save file of your choice.
3. Check item category and click "Refresh" to display missed items of that type.
4. Click "Toggle Settlements" to display unfinished settlements (with information about completion and missed items).

- Use mouse wheel to zoom, or hold Ctrl to zoom to a specific part.

System Requirements
-------------------
This program works on Windows XP or newer, and the only requirement is the installation of Microsoft .NET Framework 3.5 Redistributable.

Changelog
---------
- 0.3.4: Added an option to automatically reload the map.
- 0.3.3: Added taskbar icon, new zoom buttons.
- 0.3.2: Added Reload button and scrollbars for smaller screens.
- 0.3.1: Fixed some minor bugs and typos, minor GUI overhaul.
- 0.3: Added experimental support for Xbox 360 save games.
- 0.2: Added settlement information.
- 0.1a: Updated item lists.

Xbox 360 Support
----------------
Xbox 360 is experimental, so 100% compatibility can't be guaranteed. PC and Xbox 360 files should be almost identical; the only difference is that the Xbox 360 save files
contain more data at the start of the file (probably save icon, etc.) and are big endian.

The expected size of Xbox 360 save games is 1073152 bytes.

License
-------
The source code of JC2MapViewer and BruTile is licensed under LGPL 2.1 (GNU Lesser General Public License), which means that you can freely redistribute and modify the program.

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Copyright &copy; 2010 - 2014 DerPlaya78, Klemen "nNa" Košir, Peter "Zazcallabah" Hamberg