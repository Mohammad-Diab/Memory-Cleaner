# Memory Cleaner

A small Windows tool that frees physical memory (RAM). It fills the memory for a short time so Windows moves the idle memory of the other programs out of RAM, then releases everything at once and shows how much free memory you gained. Written in C# with WPF on .NET Framework 4.0.

## Features

- Shows the total memory, the free memory and the memory used by the app.
- **Clean** takes a screenshot and keeps adding copies of it in memory (they are never saved to the disk), with a progress bar, until about 98.5% of the memory has been in use for about half a minute or the memory runs out. Then it releases all the copies and tells you how much memory was freed.
- **Stop** ends the clean early and releases the memory.

## Requirements

- Windows with .NET Framework 4.0 or later. Windows 10 and 11 already include it.
- Windows can be slow while a clean runs, so save your work first.

## Usage

Download the latest `Memory-Cleaner-v<version>.7z` from [Releases](https://github.com/Mohammad-Diab/Memory-Cleaner/releases), extract it, and run `Memory Cleaner.exe`. There is no installer.

The exe is not signed, so Windows SmartScreen may warn the first time you run it: click **More info**, then **Run anyway**.

## Building

```
git clone https://github.com/Mohammad-Diab/Memory-Cleaner.git
```

Open `Memory Cleaner.sln` in Visual Studio and build, or run:

```
MSBuild "Memory Cleaner.sln" /p:Configuration=Release
```

The exe is written to `Memory Cleaner\bin\Release`.

The project targets .NET Framework 4.0. Recent Visual Studio versions don't ship its reference assemblies; if the build stops with `MSB3644`, extract the [Microsoft.NETFramework.ReferenceAssemblies.net40](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net40) NuGet package and point MSBuild at its `build` folder:

```
MSBuild "Memory Cleaner.sln" /p:Configuration=Release /p:TargetFrameworkRootPath=<package>\build
```

## Author

Mohammad Diab

## License

[Apache 2.0](LICENSE)
