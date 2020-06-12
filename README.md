# EQFCT
Floating Combat Text for EverQuest using .NET C# WPF

# How It Works
EQFCT will monitor your EverQuest log file much like other applications such as GINA or Gamparse. As the logs are scanned, damage done or taken is found and displayed in transparent/clickthrough windows. You can adjust where these windows are, font size and font color. Over time, more customization will be added.

# What is currently supported
* Melee Damage Taken & Melee Damage Done (criticals with increased font size)
* Spell Damage Taken & Spell Damage Done
* Misses/Resists can be included or excluded

# What's coming?
* Additional support for damage taken, done such as DOT or PET damage
* Healing support
* GUI/Combat scrolling enhancements, smooth out the drawing and add radius/arcs

# Installation
There are two primary methods to installing:
* Downloading source code and compiling yourself OR
* [Download from my S3 Bucket](https://opendkp-publisher.s3.us-east-2.amazonaws.com/eqfct/Setup.exe)

If you install using the S3 bucket option above, you'll recieve automatic updates when they're published

# Contribution
Project is 100% open source, will accept merge/pull requests!

# Donations
If you appreciate the time, work and effort that goes into the development of this project and would like to show it:
[PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=ECQ5J8H82HWT8&source=url)
Donations are absolutely not required but appreciated!

# Developer Notes
In order to Push an Update via Squirrel
* Build Release version, incrementing AssemblyInfo.cs Versions: AssemblyVersion and AssemblyFileVersion
* Create a Nuget Package using NPE (Nuget Package Explorer)
* Releasify with Squirrel: Squirrel.exe --releasify "C:\Users\Moncs\Source\repos\EQFCT\EQFCT.2.0.0.nupkg"
* Take contents of Release folder and upload to S3 bucket
* Mark contents as Public