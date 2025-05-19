[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/amaletski.nanoFramework.MSBuildTasks.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/amaletski.nanoFramework.MSBuildTasks/)

# nanoFramework.MSBuildTasks

## Package Installation (Visual Studio)

 - Install the NuGet package as usual.
 - Unload the project:
   - Right-click the project in the "Solution Explorer" window.
   - Select "Unload Project".
 - Open the project file:
   - Right-click the unloaded project in the "Solution Explorer" window.
   - Select "Edit Project File".
 - Add `props` and `targets` imports in the `nfproj`:
   - Add the following `Import` after the last `props` `Import` node (somewhere at the top of the project node).
     ```xml
        <Import Project="..\packages\amaletski.nanoFramework.MSBuildTasks.[PackageVersion]\build\netnano1.0\amaletski.nanoFramework.MSBuildTasks.props" />
     ```
     Note: Replace [PackageVersion] with the appropriate version.
   - Add the following `Import` after the last `targets` `Import` node (somewhere at the bottom of the project node).
     ```xml
        <Import Project="..\packages\amaletski.nanoFramework.MSBuildTasks.[PackageVersion]\build\netnano1.0\amaletski.nanoFramework.MSBuildTasks.targets" />
     ```
     Note: Replace [PackageVersion] with the appropriate version.
   - Press `Ctrl+S` to save changes.
 - Reload the project:
   - Right click the unloaded project in the "Solution Explorer" window.
   - Select "Reload Project".

> [!IMPORTANT]
> Make sure to update the imports after updating the NuGet package version.

<details>
  <summary>Why should I do this?</summary>
  
Without these imports, the package will not work.
There are likely some issues with the nanoFramework project system because, according to this [Microsoft Docs page](https://learn.microsoft.com/en-us/nuget/concepts/msbuild-props-and-targets#packagesconfig-projects), these imports should be managed automatically.
</details>

## Use Cases

### Resource Embedding

To use this feature, `nfproj` should be updated.

Files that should be included in the embedded `resources` can be configured through user-defined `ResourcesSource` elements. All files that match will be embedded as <b>binary</b> resources.

> [!NOTE]
> If there are no `ResourcesSource` elements in the `nfproj`, the `resx` file will not be generated, and `resources` will not be embedded.

> [!TIP]
> Multiple `ResourcesSource` elements can be specified in the same `nfproj`. All matching files will be embedded into the same `resources` file.

#### Attributes

| Attribute | Required | Description |
| --- | --- | --- |
| `Include` | Required | Relative or absolute path to the folder with files to include |
| `RegexFilter` | Optional | Regular expression to filter files within the `Include` directory. |
| `SearchPattern` | Optional | Search pattern to filter files within the `Include` directory. |

> [!WARNING]
> Do not specify `RegexFilter` and `SearchPattern` in the same `ResourcesSource` as it will cause a validation error.

Optionally, the name of the generated `resx` file can be overridden through the `GeneratedResxFileName` custom property. Default value: `Resources.resx`.

> [!NOTE]
> The generated `resx` file will be deleted on project/solution clean.

Examples:

```xml
<PropertyGroup>
  <GeneratedResxFileName>CustomFileName.resx</GeneratedResxFileName>
</PropertyGroup>
```

```xml
<ItemGroup>
  <ResourcesSource Include="Settings">
    <SearchPattern>*.json</SearchPattern>
  </ResourcesSource>
  <ResourcesSource Include="..\spa\dist" />
</ItemGroup>
```
