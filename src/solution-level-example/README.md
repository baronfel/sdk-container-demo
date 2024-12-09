# Solution-level containerization example

This example shows how to containerize multiple projects in a solution efficiently
when you are unable to run a command like `dotnet publish -r linux-x64` on the solution level.
This might be because of different RID requirements, or other configuration changes.

# Method

This example uses a `Directory.Solution.targets` file to add more Targets to the build process of the solution. The target declares the projects to build, as well as any MSBuild
properties that are necessary per-project. 
Then, those projects are Restored to ensure their required dependencies are available 
(this is something that `dotnet publish` would typically do for you). It's important to do 
this Restore because it is done with the RuntimeIdentifier property set, which tells 
Restore to get certain runtime-specific dependencies.
After this restore is complete, the projects are containerized in parallel for best 
performance.

```xml
    <Target Name="ContainerizeMyApps">
        <!-- Declare the apps to be containerized -->
        <ItemGroup>
            <App Include="App1/App1.csproj" AdditionalProperties="RuntimeIdentifier=linux-x64" />
            <App Include="App2/App2.csproj" AdditionalProperties="RuntimeIdentifier=linux-arm64" />
        </ItemGroup>
        
        <!-- Make sure we restore the projects for the correct RID to get 
             and runtimes/apphosts/etc that are required -->
        <MSBuild 
            Projects="@(App)"
            Properties="Configuration=Release;"
            Targets="Restore"
             />

        <!-- Do the publish - Note that PublishContainer doesn't chain publish itself,
             so we have to do it. Note also that we are setting the gross _IsPublishing
             hack to keep behavior with the CLI publish command. -->
        <MSBuild 
            Projects="@(App)"
            Properties="Configuration=Release;_IsPublishing=true"
            Targets="Publish;PublishContainer"
            BuildInParallel="true"
             />
            
    </Target>
```