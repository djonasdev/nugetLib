# NuGetLib

A command line tool to perform additional operations with nuget packages

It is written in C# and needs the .Net-Framework 4.6.1 to run.
On Linux it should also be executable using Mono (not tested yet).

## Usage

### nugetlib about

```shell
nugetlib about
```

Outputs the current version informations.


### nugetlib add

```shell
nugetlib add [-t <path to the target file or folder>] [-f <path to the file/folder to add>]
```

Adds a file or a folder to an existing nuget package file (***.nupkg**). If you submit a folder as target, the newest ***.nupkg** file is automatically taken.

NuGetLib is using `7zip` to manipulate the packages. The `-f` parameter is passed directly to the `7zip` library: 

> ##add command##
> Adds files to archive.
> 
> Examples

> `nugetlib add -t example.nupkg subdir\`
> 
> adds all files and subfolders from folder subdir to archive example.nupkg. The filenames in archive will contain subdir\ prefix.
> 
> `nugetlib add -t example.nupkg .\subdir\*`
> 
> adds all files and subfolders from folder subdir to archive example.nupkg. The filenames in archive will not contain subdir\ prefix.
>
> `nugetlib add -t example.nupkg .\subdir\tools\`
> 
> adds the tools folder to archive example.nupkg. The filenames in archive will not contain subdir\ prefix.
> 
> 
> `cd /D c:\dir1\`
>
> `nugetlib add -t example.nupkg dir2\dir3\`
> 
> The filenames in archive example.nupkg will contain dir2\dir3\ prefix, but they will not contain c:\dir1\ prefix.

example usage: https://stackoverflow.com/questions/47075041/vs2017-set-build-action-to-content-in-a-nuget-package

### nugetlib frameworkassemblies

```shell
nugetlib frameworkassemblies [-c <path to the project file>] [-n <path to the nuspec file>] [-p <path to the (nuget) packages file>] [-r <replace existing framework assemblies in the nuspec file>]
```

Modify/add the framework assemblies of the given project into the nuspec file. If you don't supply any parameter, then the executing directory is used/scanned for a *.csproj and  *.nuspec file.
