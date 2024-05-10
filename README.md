# .NET SDK Containerization Samples

This repository showcases a few different ways that the .NET SDK can be used to build and publish containerized applications. 

The overall layout is: 

* specific projects (in directories under the `src` directory) that demonstrate different kinds of apps, with
* matching GitHub Actions Workflows to illustrate how to build and publish the apps for different scenarios

So far we have the following samples:

* [Publishing a web application to various container registries](./src/sdk-container-demo/)
* [Using MSBuild to automatically apply versioning tags to containers](./src/msbuild-versioning-sample/)
* [Using a custom base image container instead of the Microsoft-provided base images](./src/custom-base-image/)
* [Building a 'multi-architecture' container image](./src/multi-arch-sample/)
* [Containerizing AOT applications](./src/aot-sample/)
* [Creating a container with a custom EntryPoint script](./src/custom-entrypoint/)

Please log issues to suggest more examples!
