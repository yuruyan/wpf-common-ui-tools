# 通用工具库

## CommonTools 引入
首先创建 **Lib\Net7，Lib\Net6** 文件夹，然后将相应生成的 **.dll，.xml(文档，非必须)** 文件复制进去

**.csproj** 文件加入如下配置：
```C#
<ItemGroup>
  <PackageReference Include="AutoMapper" Version="12.0.1" />
  <PackageReference Include="Newtonsoft.Json" Version="13.0.2" />
  <PackageReference Include="NLog" Version="5.1.2" />
  <PackageReference Include="Portable.BouncyCastle" Version="1.9.0" />
</ItemGroup>

<Choose>
  <When Condition=" '$(TargetFramework.StartsWith(`net7.0`))' == 'true' ">
    <ItemGroup>
      <Reference Include="CommonTools">
        <HintPath>Lib\Net7\CommonTools.dll</HintPath>
      </Reference>
    </ItemGroup>
  </When>
  <When Condition=" '$(TargetFramework.StartsWith(`net6.0`))' == 'true' ">
    <ItemGroup>
      <Reference Include="CommonTools">
        <HintPath>Lib\Net6\CommonTools.dll</HintPath>
      </Reference>
    </ItemGroup>
  </When>
</Choose>
```

## CommonUITools 引入
首先创建 **Lib\Net7，Lib\Net6** 文件夹，然后将相应生成的 **.dll，.xml(文档，非必须)** 文件复制进去

1. **.csproj** 文件加入如下配置：
```C#
<ItemGroup>
  <PackageReference Include="ModernWpfUI" Version="0.9.6" />
  <PackageReference Include="SharpVectors" Version="1.8.1" />
  <PackageReference Include="System.Drawing.Common" Version="7.0.0" />
</ItemGroup>

<Choose>
  <When Condition=" '$(TargetFramework.StartsWith(`net7.0`))' == 'true' ">
    <ItemGroup>
      <Reference Include="CommonUITools">
        <HintPath>Lib\Net7\CommonUITools.dll</HintPath>
      </Reference>
    </ItemGroup>
  </When>
  <When Condition=" '$(TargetFramework.StartsWith(`net6.0`))' == 'true' ">
    <ItemGroup>
      <Reference Include="CommonUITools">
        <HintPath>Lib\Net6\CommonUITools.dll</HintPath>
      </Reference>
    </ItemGroup>
  </When>
</Choose>
```

2. **App.xaml** 文件
```xaml
<ResourceDictionary Source="/CommonUITools;component/Themes/Generic.xaml" />
```
