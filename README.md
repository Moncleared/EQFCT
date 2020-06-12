# EQFCT
Floating Combat Text for EverQuest using .NET C# WPF

# Developer Notes
In order to Push an Update via Squirrel
* Build Release version, incrementing AssemblyInfo.cs Versions: AssemblyVersion and AssemblyFileVersion
* Create a Nuget Package using NPE (Nuget Package Explorer)
* Releasify with Squirrel: Squirrel.exe --releasify "C:\Users\Moncs\Source\repos\EQFCT\EQFCT.2.0.0.nupkg"
* Take contents of Release folder and upload to S3 bucket
* Mark contents as Public