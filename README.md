# DungeonPath
Platform game developing with Godot Engine and C# 

C# is one of multipurpose high level languages. Godot engine is an open source game engine for creating 2D and 3D games and developed based on object-oriented design. It provides C# API to nicely organize its user-friendly features in creating your awesome world.

GDScript API is the primary language that Godot engine supports and there are a lot of examples and tutorials using GDScript available on the Internet, while C# API has been introduced since Godot engine v3.0 and still has some features not fully implemented yet. However, if you are familiar with C#, it is not difficult to use Godot engine with C# API and no GDScript is needed for your project.

DungeonPath is a platform game for showing how to use Godot engine with C# API and testing its limits in implementing a game world.

Development Environment

	Ubuntu 18.04.2 LTS
	Mono JIT compiler version 5.12.0.301
	Godot game engine v3.06

[![](https://github.com/inthelamp/dungeonpath/blob/master/screenshot.png)](https://www.youtube.com/watch?v=uri3mZ_ihxI)

Click the image to watch its YouTube video.

For Windows, in DungeonPath.csproj, replace the following lines
```
    <Reference Include="System" />
    <Reference Include="Newtonsoft.Json" />
  </ItemGroup>
```
with
```
    <Reference Include="System" />
    <Reference Include="Newtonsoft.Json" >
       <HintPath>$(ProjectDir)/.mono/assemblies/Newtonsoft.Json.dll</HintPath>
       <Private>False</Private>
    </Reference>   
  </ItemGroup>
```
.<br />

Then, copy "Newtonsoft.Json.dll" into the directory indicated with "\<HintPath\>" above.
