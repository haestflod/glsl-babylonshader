# glsl-babylonshader
Converts glsl shader code into copy-pastable babylon shaderstore output.

This app is more appropriate as a gulp or grunt task instead of an executable but I wanted to experiment with .NET Core for a small project!

## How to use guide

There are 2 different modes for the application.
1. Convert a specific file / folder
2. Watch a file / folder for changes

### Convert specific file / folder
To convert either a file or folder use the action "c [arguments]"
The arguments are:
* "path": The filepath or folderpath. Can be used multiple times for many files / folders at same time.
* "--r [depth]": Enables recursive folder search. 
    * Depth is optional and is the amount of folders to iterate over. 
    * Default depth is 2. 
	* Depth -1 is unlimited amount.
* "--minify": "Minifies the output by not adding \n or \r\n depending on environment"

#### Examples
* "c folder1 file --minify file2.vertex.fx" - Converts the files: file.vertex.fx, file.fragment.fx, file2.vertex.fx and all the files in folder1. 
* "c --r" - Converts all the files starting where the application is. Then continues recursively for all other folders.
 
### Watch a file / folder ( NOT IMPLEMENTED )
To watch a file or folder use the action "w [arguments]"

## Building notes visual studio 2015 on windows:
* Go to https://www.microsoft.com/net/core#windows or https://github.com/dotnet/cli#installers-and-binaries
* Install .NET Core 1.0.0 - VS 2015 Tooling Preview 2
* Install the .NET Core SDK
* Might have to reboot to get access to "dotnet" for your cmd.
* Run "dotnet --info" to check the current version. For example "1.0.0-preview3-003223" is what we're after.
* Change global.json sdk.version property if it doesn't compile 
 