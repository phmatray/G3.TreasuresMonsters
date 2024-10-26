namespace G3.TreasuresMonsters.Features.I18n;

public interface ILanguageService
{
    string GetString(LanguageKey key);
    void SetLanguage(string languageCode);
}