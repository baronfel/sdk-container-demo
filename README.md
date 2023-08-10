# .NET SDK Containers CI Pipeline Example

This repo is a simple ASP.NET Core application that demonstrates
a CI pipeline for building the app, packaging the app into a container,
and pushing that container to a registry.

The most interesting parts of this repo are

* The [workflow definition](.github/workflows/containerize.yml)
* An [example Action run](https://github.com/baronfel/sdk-container-demo/runs/7888742074?check_suite_focus=true) showing the container push to Azure Container Registry

## Global prerequisites

The commands below will probably require

* [Docker CLI](https://docs.docker.com/get-docker/)
* [jq](https://jqlang.github.io/jq/download/)
* [.NET SDK 8.0.100-preview7](https://dotnet.microsoft.com/download/dotnet/8.0)

## Running the app locally

The different publish profiles in the [publish_profiles](./Properties/PublishProfiles/) directory target different use cases, which are described below.

### Publish to local Docker

Make sure your docker deamon is running (you can verify with `docker ps`). Then it should be very simple:

```bash
dotnet publish --os linux --arch x64 -p PublishProfile=DefaultContainer
```

After that you can run the app with `docker run -p 8080:8080 sdk-container-demo`

### AWS Private

This profile is used to publish the app to a private AWS ECR registry. It requires the following software to be installed and resources to be configured:

* An [AWS Account](https://portal.aws.amazon.com/billing/signup)
* An [Administrative user](https://docs.aws.amazon.com/AmazonECR/latest/userguide/get-set-up-for-amazon-ecr.html) to use instead of the root user
* [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
* An [AWS Elastic Container Registry](https://docs.aws.amazon.com/AmazonECR/latest/userguide/getting-started-cli.html) to store images
* A [Private AWS Elastic Container Repository](https://docs.aws.amazon.com/AmazonECR/latest/userguide/getting-started-cli.html#cli-create-repository) to push the image to

Once this is setup, you [Authenticate via Docker to the AWS ECR](https://docs.aws.amazon.com/AmazonECR/latest/userguide/getting-started-cli.html#cli-authenticate-registry).

NOTE: If you are receiving error during storing of your credentials (`Error saving credentials: error storing credentials ...`), you might be hitting the [wincred limitations in support for long tokens](https://github.com/danieljoos/wincred/issues/18) - in such case you might workaround the problem by bypassing wincred ([SO post](https://stackoverflow.com/questions/60807697/docker-login-error-storing-credentials-the-stub-received-bad-data)).
Then, push the image to the AWS ECR:

```bash
# get your ECR url
$ecr_url="<aws_account_id>.dkr.ecr.<aws_region>.amazonaws.com"
# authenticate by getting a password from AWS and logging in
aws ecr get-login-password --region <aws_region> | docker login --username AWS --password-stdin #ecr_url

# push the image to your personal
dotnet publish --os linux --arch x64 -p PublishProfile=aws-private -p ContainerRegistry=$ecr_url
```

### AWS Public

This has the same prerequisites as above, but you login and push in slightly different ways.
Note that AWS ECR Public registry [requires authentication in `us-east-1`](https://docs.aws.amazon.com/AmazonECR/latest/public/getting-started-cli.html#cli-authenticate-registry) region, regardless of your other services location.

```bash
# authenticate to the public ECR
aws ecr-public get-login-password --region us-east-1 | docker login --username AWS --password-stdin public.ecr.aws

# get your 'namespace' for the public ECR
$namespace=$(aws ecr-public describe-registries --region us-east-1 --output=json | jq -r '.registries[0].aliases[0].name')

# publish the app to the public ECR with that namespace

dotnet publish --os linux --arch x64 -p PublishProfile=aws-public -p ContainerImageName=$namespace/sdk-container-demo
```

### Azure (using managed identity)

Testing for Azure requires installing the [`az` CLI](https://learn.microsoft.com/en-us/cli/azure/get-started-with-azure-cli) and configuring it. You'll also need Docker for login purposes.

Once you have these, [follow these reproduction instructions](https://github.com/hotfix-houdini/bug-dotnet-publish-containerize-docker-auth/blob/main/README.md#prerequisite---creating-an-azure-container-registry-and-service-principal-with-permissions-for-pushing) to create an ACR as well as a service.

For convenience, they are reproduced here:

```powershell
az login
$chars = 'abcdefghijklmnopqrstuvwxyz'

# set up a bunch of random variables
$RANDOM_SUFFIX = ''
for ($i = 0; $i -lt 15; $i++) {
    $random = Get-Random -Minimum 0 -Maximum $chars.Length
    $RANDOM_SUFFIX += $chars[$random]
}
$RESOURCE_GROUP_NAME = "acr-docker-dotnetcontainers-bug"
$ACR_NAME = "testacr$RANDOM_SUFFIX"
$SERVICE_PRINCIPAL_NAME = "acr-docker-dotnetcontainers-bug-user"
$LOCATION = "centralus" # change to whatever is closest to you, check `az account list-locations -o table` to see the list of locations.
$SUBSCRIPTION_ID = (az account show --query id --output tsv)

# create an Azure Resource group to contain the cloud items
az group create --name $RESOURCE_GROUP_NAME --location $LOCATION

# create an ACR
az acr create --name $ACR_NAME --resource-group $RESOURCE_GROUP_NAME --sku Basic --location $LOCATION
$ACR_ID=$(az acr show --name $ACR_NAME --query id --output tsv)

# Create a service principal user that can log into azure for this resource group
$servicePrincipal = az ad sp create-for-rbac --name $SERVICE_PRINCIPAL_NAME --role Contributor --scopes /subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP_NAME
$appId = $servicePrincipal | ConvertFrom-Json | Select-Object -ExpandProperty appId
$password = $servicePrincipal | ConvertFrom-Json | Select-Object -ExpandProperty password
$tenant = $servicePrincipal | ConvertFrom-Json | Select-Object -ExpandProperty tenant

#Grant push access to ACR to the service principal
az role assignment create --assignee $appId --scope $ACR_ID --role AcrPush

# Document some values for later use:
Write-Host "ACR Registry Name: $ACR_NAME.azurecr.io"
Write-Host "Service Principal ClientId: $appId"
Write-Host "Service Principal ClientSecret (Password): $password"
Write-Host "Service Principal Tenant (Password): $tenant"

```

At this point you've set up an ACR to hold images and a Service Principal to push with. Now let's actually push some images

```powershell
# logout of your current account
az logout
# login as the service principal
az login --service-principal -u $appId -p $password -t $tenant
# login to the ACR (this will use the service principal's credentials and run `docker login` for your ACR for you)
az acr login --name $ACR_NAME

# now push the app
docker publish --os linux --arch x64 -p PublishProfile=azure -p ContainerRegistry=$ACR_NAME.azurecr.io
```

When you're done logout with `az logout` to prevent messing with the other authentication mechanisms for Azure.

### Azure (using admin credentials)

With the ACR from above, you can also use admin credentials to push to the ACR. This requires the same prerequisites as above, but you login and push in slightly different ways:

```bash
# you'll need your ACR's name from above, which should still be in $ACR_NAME

# login as yourself again
az login

# get the admin credential for your ACR
$ACR_ADMIN_PASSWORD=$(az acr credential show --name $ACR_NAME --query "passwords[0].value")

# login via docker using admin credentials
docker login --username $ACR_NAME --password $ACR_ADMIN_PASSWORD $ACR_NAME.azurecr.io

# publish the app
docker publish --os linux --arch x64 -p PublishProfile=azure -p ContainerRegistry=$ACR_NAME.azurecr.io
```

### GitHub packages

In order to push packages to GitHub packages you'll need to fork this repo.

You'll also need a [Personal Access Token (classic)](https://github.com/settings/tokens/new) with the 'write:packages' permission.

Then, login using your GitHub username and this token:

```bash
docker login ghcr.io -u <github_username> -p <personal_access_token>
```

Then, publish the app:

```bash
# Note the GitHub username is required (since there's no 'namespacing' in the registry URL)
dotnet publish --os linux --arch x64 -p PublishProfile=github -p ContainerImageName=<github_username>/sdk-container-demo
```

### Docker Hub

You'll need an [account on Docker Hub](https://hub.docker.com/signup), and you'll need to login with a [Personal Access Token for Docker Hub](https://hub.docker.com/settings/security?generateToken=true) with the 'Write' permission.

Then, login using your Docker Hub username and this token:

```bash
docker login -u <docker_hub_username> -p <personal_access_token>
```

Then, publish the app:

```bash
docker publish --os linux --arch x64 -p PublishProfile=dockerhub -p ContainerImageName=<docker_hub_username>/sdk-container-demo
```

