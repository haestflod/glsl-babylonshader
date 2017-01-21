# glsl-babylonshader
Converts glsl shader code into copy-pastable babylon shaderstore output.

This app is probably more appropriate as a gulp or grunt task instead of an executable but I wanted to experiment with .NET Core for a small project!

## How to use guide

There are 2 different modes for the application.
1. Convert a specific file / folder
2. Watch a file / folder for changes

### Convert specific file / folder
To convert either a file or folder use the action "c [arguments]"
The arguments are:
* "--r [depth]": Enables recursive folder search. 
	* Depth is optional and is the amount of folders to iterate over. 
	* Default depth is 2. 
	* Depth -1 is unlimited amount.
* "--minify": Minifies the output by not adding \n or \r\n depending on environment.
* "--editjs": When a shader file is converted also try and update the javascript files that has the shadersstore code. It will make backup of the original file in **"glsl-babylonshader-backups"** folder where the current directory is incase it wrongly updates.
* Any text that is not the arguments above will be treated as an input folder or file. If no input path is found then it will use the current directory.

#### Examples
* "c folder1 file --minify file2.vertex.fx" - Converts the files: file.vertex.fx, file.fragment.fx, file2.vertex.fx and all the files in folder1. 
* "c --r" - Converts all the files starting where the current directory is. Then continues recursively for all other folders 2 levels deep.
 
### Watch a file / folder
The watch command works the same as convert command. What the watch command does is find all .fx files and watches them for updates.
When a .fx file is edited it automatically calls convert with the given arguments. The watcher needs folders and not filepaths.
Watch commands does not search for recursive folders currently. 

It can handle deleting, renaming or editing and adding of .fx files in the folder it's watching.


## Building notes visual studio 2015 on windows:
* Go to https://www.microsoft.com/net/core#windows or https://github.com/dotnet/cli#installers-and-binaries
* Install .NET Core 1.0.0 - VS 2015 Tooling Preview 2
* Install the .NET Core SDK
* Might have to reboot to get access to "dotnet" for your cmd.
* Run "dotnet --info" to check the current version. For example "1.0.0-preview3-003223" is what we're after.
* Change the json property **sdk.version** in global.json to the "1.0.0-preview..." value if it doesn't compile 
 
 ### Publishing Visual studio
 * Goto **glsl-babylon/bin/release** folder
 * You can remove **file glsl-babylon.runtimeconfig.dev** 
 * Open cmd, navigate to folder where the dll is and run "dotnet glsl-babylon.dll" 

 ### Publishing with dotnet cli
 * Go to src/glsl-babylon with your console app.
 * Run command "dotnet publish"
 * Goto folder from bin/debug/publish
 * Run command "dotnet glsl-babylon.dll"