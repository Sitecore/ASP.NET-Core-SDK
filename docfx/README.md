# ASP.NET Core SDK - DocFX Documentation
The code in the `/docfx` folder is used to generate the documentation for the Sitecore ASP.NET Core SDK. The documentation is generated using [DocFX](https://dotnet.github.io/docfx/).

## Building the documentation locally
You can build and run this documentation locally by running the following commands

### First time setup
```dotnetcli
dotnet tool update -g docfx
```

### Subsequent runs
:warning: Ensure you are in the `/docfx` folder

#### Run the documentation local server
```dotnetcli
docfx docfx.json --serve
```

You will then be able to access the documentation site at [http://localhost:8080](http://localhost:8080)

#### Build the documentation
```dotnetcli
docfx docfx.json
```