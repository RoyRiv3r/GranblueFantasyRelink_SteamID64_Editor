# GranBlue Fantasy Relink - SteamID64 Editor
Allows users to edit the SteamID64 stored in the save files of GranBlue Fantasy: Relink.

## Features
* Converts the stored SteamID64 to a human-readable format.
* Steam Community profile links for SteamID64 validation.
* No dependency on any existing save file.

## Requirement

* .NET Framework 4.8

## Usage
1. Download the editor from [the release section]([https://github.com/Idearum/NieRAutomata-SteamID64-Editor/releases](https://github.com/RoyRiv3r/GranblueFantasyRelink_SteamID64_Editor/releases/tag/1.0.0)).

2. Run the tool and open one of the save files (\*.dat) of GBFR:

   - **SaveData1.dat** stores general game progression, most obviously the main menu background.
   - **SystemData.dat** stores graphics settings.
   
3. Use https://steamid.xyz/, https://steamidfinder.com/, or https://steamid.co/ to locate the new SteamID64.

4. Copy/paste the new SteamID64 into the tool. Click on the 'Steam Community Profile' link to verify that the ID is correct.

5. Click on 'Update' to save the new ID to the file.

## Preview
![Screenshot of the editor](https://i.imgur.com/TefvkOF.png")

## Credits
-  [Aemony](https://github.com/Aemony) [jimmyazrael](https://github.com/jimmyazrael) for his initial [NierAutoModSave](https://github.com/jimmyazrael/NierAutoModSave) that showed that this was possible.

## License
See [LICENSE](LICENSE).
