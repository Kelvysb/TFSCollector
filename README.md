# TFSCollector
Tool for collecting workitens data from a TFS server.

# Install:

dotnet tool install --global TFSCollector

https://www.nuget.org/packages/TFSCollector/

Requires .net core 2.2 or newer

https://dotnet.microsoft.com/download/dotnet-core/2.2

# Usage:

```
Update workitens:
 --update [Query name] (if query name ommited will the default query)
     --csv <csvName> (optional generate CSV)

Save all in csv:
 --csv <csv file Name>

Create or update a query in my queries folder:
 --query <query Name>

Open data base location:
 --open

Configure connection:
 --config

 Get current dir:
     -env or --env

 Program Version:
     -v or --version
 Help:
     -h or --help
```
