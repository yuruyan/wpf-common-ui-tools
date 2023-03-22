using System.Windows.Markup;

[assembly: XmlnsDefinition("wpf-common-ui-tools", "CommonUITools.Utils")]
[assembly: XmlnsDefinition("wpf-common-ui-tools", "CommonUITools.Widget")]
[assembly: XmlnsDefinition("wpf-common-ui-tools", "CommonUITools.Model")]
[assembly: XmlnsDefinition("wpf-common-ui-tools", "CommonUITools.Converter")]
[assembly: XmlnsPrefix("wpf-common-ui-tools", "tools")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]
