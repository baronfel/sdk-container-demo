# Ahead-of-time (AOT) compilation and containerization

This sample shows how an AOT-compiled console application could be containerized using the .NET SDK. This application is compiled for 4 OS/architecture combinations:

* Ubuntu x64
* Ubuntu ARM64
* Alpine x64
* Alpine ARM64 (note: in GitHub Actions this requires emulation - this is often flaky so it's been commented out of the workflow)

The various images are also combined into one 'multi-architecture' image so that the correct image is used for the host on which the container is running.

The sample includes:
* a simple [.NET Console application](./Program.cs) that echoes out the OS and architecture information when run
* a [GitHub Actions workflow](../../.github/workflows/aot.yml) showing how to combine the 4 architecture-specific images into a single multi-architecture image

This scenario is especially tricky because AOT compilation generally requires performing the publish on an OS and architecture that matches the target. This sample shows how to use the .NET SDK to build the AOT-compiled application for each target and then use the Docker CLI to combine them into a single image. Take note in the workflow how the actual image is published using the `mcr.microsoft.com/dotnet/sdk` container for that platform, but the generated image is pushed to an external registry for use later in the workflow.
