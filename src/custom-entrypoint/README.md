# Setting a custom entrypoint on the container

This sample shows how to use a [custom entrypoint script](./my-entrypoint) in your container.
An entrypoint is a script that you can use to configure your app at runtime before your application is launched.

This sample contains a simple console project with a custom entrypoint script that prints a message before the application starts. The project file contains modifications that tell the SDK how to create a container image that will invoke the custom entrypoint script.

To build and run the sample, use the following commands:

```sh
> dotnet publish -t:PublishContainer -r linux-x64
Determining projects to restore...
  All projects are up-to-date for restore.
C:\Program Files\dotnet\sdk\9.0.100-preview.3.24204.13\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.RuntimeIdentifierInference.targets(314,5): message NETSDK1057: You are using a preview version of .NET. See: https://aka.ms/dotnet-support-policy [E:\Code\sdk-container-demo\src\custom-entrypoi
nt\custom-entrypoint.csproj]
  custom-entrypoint -> E:\Code\sdk-container-demo\artifacts\bin\custom-entrypoint\release_linux-x64\custom-entrypoint.dll
  custom-entrypoint -> E:\Code\sdk-container-demo\artifacts\publish\custom-entrypoint\release_linux-x64\
  Building image 'custom-entrypoint' with tags 'latest' on top of base image 'mcr.microsoft.com/dotnet/runtime-deps:9.0.0-preview.3'.
  Pushed image 'custom-entrypoint:latest' to local registry via 'docker'.
> docker run custom-entrypoint
running the custom entrypoint
Hello, World!
```
