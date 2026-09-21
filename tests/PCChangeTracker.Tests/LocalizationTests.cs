using PCChangeTracker.App.Localization;
using PCChangeTracker.Core;
using Xunit;

namespace PCChangeTracker.Tests;

public sealed class LocalizationTests
{
    [Fact]
    public void InstalledCatalogsHaveCompleteKeysAndValidPlaceholders()
    {
        var english = UiText.ReadCatalog("en");
        foreach (var language in UiText.Languages)
        {
            var catalog = UiText.ReadCatalog(language.Code);
            Assert.NotEmpty(catalog);
            Assert.Equal(english.Keys.Order(), catalog.Keys.Order());
            foreach (var key in english.Keys)
            {
                Assert.False(string.IsNullOrWhiteSpace(catalog[key]), language.Code + ": " + key);
                var expected = System.Text.CompositeFormat.Parse(english[key]);
                var actual = System.Text.CompositeFormat.Parse(catalog[key]);
                Assert.Equal(expected.MinimumArgumentCount, actual.MinimumArgumentCount);
            }
        }
    }

    [Fact]
    public void CatalogHasTwentyExplicitLanguagesAndSafeFallback()
    {
        Assert.Equal(20, UiText.Languages.Count);
        Assert.Equal(20, UiText.Languages.Select(language => language.Code).Distinct().Count());
        Assert.Equal(new[] { "ar", "ur", "arz" }, UiText.Languages.Where(language => language.RightToLeft).Select(language => language.Code));
        var text = new UiText("unknown");
        Assert.Equal("en", text.Language.Code);
        Assert.Equal("Check now", text["CheckNow"]);
        Assert.Equal("3 changes to review", text.Format("ReviewCount", 3));
        Assert.Equal("Machine-wide / Standard access", text.Context(CollectionScope.Machine, false));
        Assert.Equal("English (English)", UiText.Languages[0].Display);
        foreach (var language in UiText.Languages) Assert.NotNull(System.Globalization.CultureInfo.GetCultureInfo(language.CultureName));
    }
}