# Portabilizer
An app to create portable versions of other apps using symbolic links and registry tweaks. This does not tamper with any apps inner code to keep things safe.

## The Configuration File
Create a config.json besides the portable app to configure it ^_^. 
```json
{
	"Title" : "My Portalbe App",
	"Width" : 512,
	"Height": 115,
	"ClearSymLinks": true,
	"Executable" : "bin\\MyApp.exe",
	"DataFolder" : "Portable",
	"DataPaths":
		[
			{
				"GroupName": "Group folder Name 1",
				"Paths": {
					"Folder1": "%LocalAppData%\\TestFolderName1",
					"Folder2": "%LocalAppData%\\Test\\TestFolderName1"
				}
			},
			{
				"GroupName": "Group folder Name 2",
				"Paths": {
					"CustomFolder1": "%LocalAppData%\\TestFolderName2",
					"MyFolder2": "%LocalAppData%\\Test\\TestFolderName2"
				}
			}
		]
}
```

* __Title__ - This is the title of your app, it will be shown in the UI when the app runs.
* __Width__ - The width of the apps GUI.
* __Height__ - The height of the apps GUI.
* __ClearSymLink__ - If true, symbolic links created by the app will be deleted when it closes.
* __Executable__ - The executable file to run after all fixes are applied.
* __DataFolder__ - This is where copied data files and folders will be stored. It will contain target folders for Symlink creation.
	* You can create a folder named "Registries" here and put all your registry files there. Change the file extension from __.reg__ to __.preg__.
* __DataPaths__ - A collection of paths that will be copied (if not already existing in datafolder) and will serve as the target for symlink creation.
	* __GroupName__ - This will be another folder inside DataFolder to contain all folders of "__Paths__".
	* __Paths__ - This contains all the Source paths that will be copied (if not already existing in datafolder).
		* Notice from the example that "__Paths__" is a __Key Value Pair__. The "__Key__" will serve as the new folder name of the copied folder from the "__Value__". This is to prevent 2 folders having the same name merging.

Use [Windows Environment Path Variables](https://superuser.com/a/217506) on DataPaths for a more dynamic path detection and to keep it shorter.

# The Preg File(s)
The preg files must be placed in "__\*YOUR_DATA_FOLDER\*__\Registries". The app will scan for these files and will be integrated to the System Registry.

Your registry file may contain the following blocks: (*only 2 at the moment. I can't think of any other useful blocks*)
* __{{StartLocation}}__ - This block will be replaced with the starting location of the portable app.
* __{{OSBit}}__ - This block will be replaced with whatever your system's architecture is. 32 for 32Bit Systems and 64 for 64Bit Systems.
	__Note:__ Make sure no spaces are in the curly braces as it may fail detection.
	
# Plugin (Portable.dll)
If your app needs more work to go portable, create a "__Portable.dll__" for it. 
(Will update this document soon.)
