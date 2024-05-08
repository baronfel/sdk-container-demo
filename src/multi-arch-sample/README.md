# Multi-architecture containers

This sample illustrates how you can use a combination of the .NET SDK and the Docker CLI to create 
multi-architecture container images. These images will automatically use the correct architecture
for the host on which they are running.

The sample includes:
* a simple [.NET Console application](./Program.cs) that echoes out the OS and architecture information when run
* a [GitHub Actions workflow](../../.github/workflows/multi-arch-sample.yml) showing how to combine two architecture-specific images into a single multi-architecture image

This example currently has to use both the .NET CLI and a container CLI to create multi-image manifests because the .NET CLI doesn't natively know how to create multi-architecture images. If you'd use this functionality in the .NET CLI, please [+1 or comment on the tracking issue](https://github.com/dotnet/sdk-container-builds/issues/87).