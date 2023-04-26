# ºÏ²¢ Generic MergedDictionaries
import xml.etree.ElementTree as ET
import os.path as path

TagPrefix = '{http://schemas.microsoft.com/winfx/2006/xaml/presentation}'
RootPath = path.join(path.dirname(__file__), '../')
GenericResourcePath = path.join(RootPath, "Themes/Generic.xaml")
GenericDom = ET.parse(GenericResourcePath)
Root = GenericDom.getroot()
GenericMergedDictionaries = Root.find(
    TagPrefix+'ResourceDictionary.MergedDictionaries'
)
nonThemeSources = []
nonThemeResource = []

for res in GenericMergedDictionaries.findall(TagPrefix+"ResourceDictionary"):
    source = res.attrib['Source']
    if 'component/Themes/LightThemeResources' not in source:
        nonThemeResource.append(res)
        nonThemeSources.append(
            path.join(RootPath, source.replace('/CommonUITools;component/', '')))
# remove non theme resources
for res in nonThemeResource:
    GenericMergedDictionaries.remove(res)

# append non theme resources
for resPath in nonThemeSources:
    doc = ET.parse(resPath)
    for node in doc.getroot():
        # skip MergedDictionaries
        if 'ResourceDictionary.MergedDictionaries' in node.tag:
            continue
        Root.append(node)
    print(f'process {path.basename(resPath)} done')

GenericDom.write(GenericResourcePath)
