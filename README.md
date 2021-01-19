# SetVersion
Sets the next version of the projects.

## Usage

By running the SetVersion executable at a (CSharp) solution location, it will scan for all .NET Standard/Core projects to determine the current version.
It will then increment the patch version and update all project files (csproj) to be the new version.
