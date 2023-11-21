using CommonUITools.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Windows;

namespace CommonUIToolsTests.Converter;

[TestClass()]
public class VisibilityIncludesConverterTests {
    [TestMethod()]
    public void Convert() {
        var converter = new VisibilityIncludesConverter();
        Assert.AreEqual(
            converter.Convert("0", typeof(Visibility), "[a b c]|Visible", CultureInfo.InvariantCulture),
            Visibility.Visible
        );
        Assert.AreEqual(
            converter.Convert("a", typeof(Visibility), "[a b c]|Visible", CultureInfo.InvariantCulture),
            Visibility.Visible
        );
        Assert.AreEqual(
            converter.Convert("a", typeof(Visibility), "[a b c]|Collapsed", CultureInfo.InvariantCulture),
            Visibility.Collapsed
        );
        Assert.AreEqual(
            converter.Convert("", typeof(Visibility), "[a b c]|Collapsed", CultureInfo.InvariantCulture),
            Visibility.Visible
        );
        Assert.AreEqual(
            converter.Convert(" ", typeof(Visibility), "[a b c]|Collapsed", CultureInfo.InvariantCulture),
            Visibility.Visible
        );
        Assert.AreEqual(
            converter.Convert(".......", typeof(Visibility), "[a b c]|Collapsed", CultureInfo.InvariantCulture),
            Visibility.Visible
        );
        Assert.AreEqual(
            converter.Convert(" ", typeof(Visibility), "[a    b      c]|Collapsed", CultureInfo.InvariantCulture),
            Visibility.Visible
        );
    }
}