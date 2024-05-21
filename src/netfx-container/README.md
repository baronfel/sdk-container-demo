# Containerizing a .NET Framework Application for Windows

The SDK can create Windows containers as well, but it takes a bit more explicit information.

This project contains a .NET Framework Hello World application. The primary changes required to containerize this application are:

* Specifying a Windows container explicitly, since the .NET SDK has no inference knowledge for Windows .NET Framework containers:

```xml
<PropertyGroup>
  <ContainerBaseImage>mcr.microsoft.com/dotnet/framework/runtime:4.7.2</ContainerBaseImage>
</PropertyGroup>
```

* Fixing the start command for the application, since the .NET SDK incorrectly infers a `dotnet` launch command (issue to be filed)

```xml
<ItemGroup>
  <ContainerEntrypoint Include="C:\app\netfx-container.exe" />
</ItemGroup>
```

* Publishing the container using the `win10-x64` Runtime Identifier

```terminal
> dotnet publish -t:PublishContainer -r win10-x64
```

If you do all of these things, you will get a Windows Container with the .NET Framework installed and your application running:

```terminal
> dotnet publish -t:PublishContainer -bl -r win10-x64 -tl:off
  Determining projects to restore...
  All projects are up-to-date for restore.
C:\Program Files\dotnet\sdk\9.0.100-preview.3.24204.13\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.RuntimeIdentifierInference.targets(314,5): message NETSDK1057: You are using a preview
 version of .NET. See: https://aka.ms/dotnet-support-policy [E:\Code\sdk-container-demo\src\netfx-container\netfx-container.csproj]
  netfx-container -> E:\Code\sdk-container-demo\artifacts\bin\netfx-container\debug_win10-x64\netfx-container.exe
  netfx-container -> E:\Code\sdk-container-demo\artifacts\publish\netfx-container\debug_win10-x64\
  Building image 'netfx-container' with tags 'latest' on top of base image 'mcr.microsoft.com/dotnet/framework/runtime:4.7.2'.
  Pushed image 'netfx-container:latest' to local registry via 'docker'.
> docker run netfx-container:latest
Hello, World!
```