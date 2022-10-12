# .NET SDK Containers CI Pipeline Example

This repo is a simple ASP.NET Core application that demonstrates
a CI pipeline for building the app, packaging the app into a container,
and pushing that container to a registry.

The most interesting parts of this repo are

* The [workflow definition](.\.github\workflows\containerize.yml)
* An [example Action run](https://github.com/baronfel/sdk-container-demo/runs/7888742074?check_suite_focus=true) showing the container push to Azure Container Registry