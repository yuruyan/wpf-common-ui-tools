# coding=utf-8
# ºÏ²¢ Generic MergedDictionaries
import xml.etree.ElementTree as ET
import os.path as path

ET.register_namespace("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation")
ET.register_namespace("x", "http://schemas.microsoft.com/winfx/2006/xaml")
ET.register_namespace("ui", "http://schemas.modernwpf.com/2019")
ET.register_namespace("utils", "clr-namespace:CommonUITools.Utils")
ET.register_namespace("controls", "clr-namespace:CommonUITools.Controls")
ET.register_namespace("system", "clr-namespace:System;assembly=System.Runtime")
ET.register_namespace("converter", "clr-namespace:CommonUITools.Converters")

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

Root.set("xmlns:utils", "clr-namespace:CommonUITools.Utils")
GenericDom.write(GenericResourcePath)
