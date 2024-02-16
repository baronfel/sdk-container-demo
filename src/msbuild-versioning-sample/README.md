# Container Tag Versioning through MSBuild logic

This directory contains a sample project that demonstrates how to use MSBuild to automatically version container images.

We're using [Ionide.KeepAChangelog.Tasks](https://github.com/ionide/KeepAChangelog) to handle versioning the app. It reads the [`CHANGELOG.md`](./CHANGELOG.md) file and populates a few MSBuild properties that build/pack/etc use.

These properties are set _during the build_, so we can't simply add

```xml
<ContainerImageTags>blah;foo</ContainerImageTags>
```

to the `.csproj` file. This is because project file properties are read in as part of _evaluation_, but the versioning logic from Ionide.KeepAChangelog.Tasks (and similar tools like NerdBank.GitVersioning) runs during _execution_.

To fix this, we need to set the property sometime _after_ Ionide.KeepAChangelog.Tasks has run and before the `ContainerImageTags` are computed by the SDK. We can do this by adding a target to the `.csproj` file that runs immediately before the SDK's `ComputeContainerConfig` Target.

```xml
  <Target Name="SetContainerTags" BeforeTargets="ComputeContainerConfig">
    <PropertyGroup>
      <ContainerImageTags>latest;$(Version)</ContainerImageTags>
    </PropertyGroup>
  </Target>
```

Now, when we publish, we'll make containers with tags like `msbuild-versioning-sample:latest` and `msbuild-versioning-sample:0.1.0`.