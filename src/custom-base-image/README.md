# Using a custom base image container

This sample demonstrates how to take control of the base image used by the SDK tooling to build your application.

It includes 

* A Dockerfile with some customizations (in this case, installing a package into the runtime environment)
* A Console app that needs that package in order to execute

To build and run the sample, follow these steps:
* build the base image and push it to a registry
  * `docker build -f Dockerfile -t <my-registry>/<my-image-name>:<my-tag> .`
  * `docker push <my-registry>/<my-image-name>:<my-tag>`
* publish the console application as a container, specifying your base image
  * `dotnet publish -t:PublishContainer -p ContainerBaseImage=<my-registry>/<my-image-name>:<my-tag>`

> [!IMPORTANT]
> Pushing images to a registry may require authentication, both on the push and pull side. Make sure you have the necessary credentials to access the registry you are using.

You can see an end-to-end example of using this sample in [these GitHub Actions](https://github.com/baronfel/sdk-container-demo/actions/workflows/custom-base-image.yml) - see the [workflow](./../../.github/workflows/custom-base-image.yml) in this registry