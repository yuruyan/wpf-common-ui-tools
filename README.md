# 通用工具库

## CommonTools 引入

首先创建 **Lib\Net8** 文件夹，然后将相应生成的 **.dll，.xml(文档，非必须)** 文件复制进去

**.csproj** 文件加入如下配置：

```xml
<ItemGroup>
  <PackageReference Include="AutoMapper" Version="13.0.1" />
  <PackageReference Include="Microsoft.Extensions.Configuration.CommandLine" Version="8.0.0" />
  <PackageReference Include="Karambolo.Extensions.Logging.File" Version="3.5.0" />
  <PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
</ItemGroup>

<ItemGroup>
  <Reference Include="CommonTools">
    <HintPath>Lib\Net8\CommonTools.dll</HintPath>
  </Reference>
</ItemGroup>
```

## CommonUITools 引入

首先创建 **Lib\Net8** 文件夹，然后将相应生成的 **.dll，.xml(文档，非必须)** 文件复制进去

1. **.csproj** 文件加入如下配置：

```xml
<ItemGroup>
  <PackageReference Include="AutoMapper" Version="13.0.1" />
  <PackageReference Include="Microsoft.Extensions.Configuration.CommandLine" Version="8.0.0" />
  <PackageReference Include="Karambolo.Extensions.Logging.File" Version="3.5.0" />
  <PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
  <PackageReference Include="ModernWpfUI" Version="0.9.6" />
  <PackageReference Include="SharpVectors" Version="1.8.1" />
  <PackageReference Include="System.Drawing.Common" Version="8.0.0" />
  <PackageReference Include="Newtonsoft.Json" Version="13.0.2" />
</ItemGroup>

<ItemGroup>
  <Reference Include="CommonTools">
    <HintPath>Lib\Net8\CommonTools.dll</HintPath>
  </Reference>
  <Reference Include="CommonUITools">
    <HintPath>Lib\Net8\CommonUITools.dll</HintPath>
  </Reference>
</ItemGroup>
```

2. **App.xaml** 文件

```xaml
xmlns:ui="http://schemas.modernwpf.com/2019"

<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <ui:ThemeResources />
      <ui:XamlControlsResources />
      <ResourceDictionary Source="/CommonUITools;component/Themes/Generic.xaml" />
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

3. 将 **Window** 继承自 **CommonUITools.Controls.BaseWindow**

4. 引用：**xmlns:tools="wpf-common-ui-tools"**
