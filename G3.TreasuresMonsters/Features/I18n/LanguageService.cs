namespace G3.TreasuresMonsters.Features.I18n;

public class LanguageService : ILanguageService
{
    public string LanguageCode { get; private set; } = "en";

    public void SetLanguage(string languageCode)
    {
        // Supported languages: "en" and "fr"
        LanguageCode = languageCode == "fr" ? "fr" : "en";
    }

    public string GetString(LanguageKey key)
    {
        return LanguageCode switch
        {
            "en" => GetEnglishString(key),
            "fr" => GetFrenchString(key),
            _ => GetEnglishString(key) // Fallback to English
        };
    }

    private static string GetEnglishString(LanguageKey key)
    {
        return key switch
        {
            LanguageKey.Level => "--- Level {0} ---",
            LanguageKey.ScoreToBeat => "Score to beat: {0}",
            LanguageKey.MovePrompt => $"Move ({Constants.MoveLeft}/{Constants.MoveDown}/{Constants.MoveRight}), H for hint, Q to quit: ",
            LanguageKey.InvalidInput => "Invalid input.",
            LanguageKey.CannotMoveUp => "You cannot move up.",
            LanguageKey.CannotMoveLeft => "You cannot change direction to the left.",
            LanguageKey.CannotMoveRight => "You cannot change direction to the right.",
            LanguageKey.CannotMoveThere => "You cannot move there.",
            LanguageKey.MonsterEncounter => "You encountered a monster! You lose {0} health points.",
            LanguageKey.TreasureFound => "You found a treasure! You gain {0} points.",
            LanguageKey.NoHintAvailable => "No hints available.",
            LanguageKey.HintUsed => "You used a hint.",
            LanguageKey.PerfectPath => "Perfect path: {0}",
            LanguageKey.LevelCompleted => "Level completed!",
            LanguageKey.YourScore => "Your score: {0}",
            LanguageKey.BeatScore => "You beat the score! You gain a hint for the next levels.",
            LanguageKey.DidNotBeatScore => "You did not beat the score.",
            LanguageKey.GameOver => "You are dead. Game over!",
            LanguageKey.ThanksForPlaying => "Thank you for playing!",
            LanguageKey.HeroStatus => "Health: {0} / 100 | Score: {1} | Hints: {2}",
            LanguageKey.LevelEnd => $"Press '{Constants.MoveDown}' to move to the next level.",
            LanguageKey.HeroIsDead => "The hero is already dead.",
            LanguageKey.NoValidPath => "There is no valid path.",
            _ => key.ToString() // Return key name if not found
        };
    }

    private static string GetFrenchString(LanguageKey key)
    {
        return key switch
        {
            LanguageKey.Level => "--- Niveau {0} ---",
            LanguageKey.ScoreToBeat => "Score à battre : {0}",
            LanguageKey.MovePrompt => $"Déplacez-vous ({Constants.MoveLeft}/{Constants.MoveDown}/{Constants.MoveRight}), H pour indice, Q pour quitter : ",
            LanguageKey.InvalidInput => "Entrée invalide.",
            LanguageKey.CannotMoveUp => "Impossible de remonter.",
            LanguageKey.CannotMoveLeft => "Vous ne pouvez pas changer de direction vers la gauche.",
            LanguageKey.CannotMoveRight => "Vous ne pouvez pas changer de direction vers la droite.",
            LanguageKey.CannotMoveThere => "Vous ne pouvez pas vous déplacer là.",
            LanguageKey.MonsterEncounter => "Vous avez rencontré un monstre ! Vous perdez {0} points de vie.",
            LanguageKey.TreasureFound => "Vous avez trouvé un trésor ! Vous gagnez {0} points.",
            LanguageKey.NoHintAvailable => "Aucun indice disponible.",
            LanguageKey.HintUsed => "Vous avez utilisé un indice.",
            LanguageKey.PerfectPath => "Chemin parfait : {0}",
            LanguageKey.LevelCompleted => "Niveau terminé !",
            LanguageKey.YourScore => "Votre score : {0}",
            LanguageKey.BeatScore => "Vous avez battu le score ! Vous gagnez un indice pour les niveaux suivants.",
            LanguageKey.DidNotBeatScore => "Vous n'avez pas battu le score.",
            LanguageKey.GameOver => "Vous êtes mort. Fin du jeu !",
            LanguageKey.ThanksForPlaying => "Merci d'avoir joué !",
            LanguageKey.HeroStatus => "Vie : {0} / 100 | Score : {1} | Indices : {2}",
            LanguageKey.LevelEnd => $"Appuyez sur '{Constants.MoveDown}' pour passer au niveau suivant.",
            LanguageKey.HeroIsDead => "Le héros est déjà mort.",
            LanguageKey.NoValidPath => "Il n'y a pas de chemin valide.",
            _ => key.ToString() // Return key name if not found
        };
    }
}