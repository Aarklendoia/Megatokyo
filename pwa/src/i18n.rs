//! The PWA's own UI chrome text (button labels, headings, status
//! messages) — not comic/rant *content*, which is translated server-side
//! via DeepL (`/rant?lang=`, see `rants.rs`). A small compile-time-checked
//! key table rather than a Fluent/ICU setup: the string count and
//! grammar needs (no plurals/genders beyond a plain count) don't justify
//! the extra dependency weight in a bundle a phone has to download.

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum Locale {
    En,
    Fr,
}

impl Locale {
    pub fn code(self) -> &'static str {
        match self {
            Locale::En => "en",
            Locale::Fr => "fr",
        }
    }

    pub fn from_code(code: &str) -> Option<Self> {
        match code {
            "en" => Some(Locale::En),
            "fr" => Some(Locale::Fr),
            _ => None,
        }
    }

    /// The language's own name, for the picker in Settings — always
    /// shown in that language regardless of the currently active one
    /// (a French speaker still recognizes "English", same the other way).
    pub fn own_name(self) -> &'static str {
        match self {
            Locale::En => "English",
            Locale::Fr => "Français",
        }
    }

    pub const ALL: [Locale; 2] = [Locale::En, Locale::Fr];
}

/// `navigator.language`'s primary subtag (e.g. `"fr"` from `"fr-FR"`),
/// falling back to English for anything not yet translated.
pub fn detect() -> Locale {
    web_sys::window()
        .and_then(|w| w.navigator().language())
        .and_then(|lang| {
            let primary = lang.split('-').next().unwrap_or(&lang).to_ascii_lowercase();
            Locale::from_code(&primary)
        })
        .unwrap_or(Locale::En)
}

/// The locale actually in effect: a stored manual override if there is
/// one, else the browser-detected one.
pub fn effective_locale(stored_override: Option<&str>) -> Locale {
    stored_override
        .and_then(Locale::from_code)
        .unwrap_or_else(detect)
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum Key {
    TabHome,
    TabReader,
    TabGallery,
    TabRants,
    TabSettings,
    Loading,
    ContinueReading,
    StartReading,
    NotStartedReading,
    SearchStripsAndRants,
    NoMatches,
    LatestRant,
    FilterAll,
    FilterFavorites,
    Back,
    SearchRants,
    LoadingStrips,
    NoStripsYet,
    Previous,
    Next,
    DaemonSettingsHeading,
    BaseUrlLabel,
    TokenLabel,
    Save,
    LoadChapters,
    EnableNotifications,
    DisableNotifications,
    Done,
    TranslationHeading,
    DeeplKeyLabel,
    PollIntervalLabel,
    Saved,
    ChaptersHeading,
    NotLoadedYet,
    LanguageHeading,
    LanguageAuto,
    CheckingDaemon,
    NeverCheckedYet,
    CheckNow,
    Checking,
}

/// Every key, in both locales — a new `Locale` variant forces the
/// compiler to flag every arm below as non-exhaustive, so a half-added
/// language can't ship silently missing strings.
pub fn t(locale: Locale, key: Key) -> &'static str {
    use Key::*;
    match key {
        TabHome => match locale {
            Locale::En => "Home",
            Locale::Fr => "Accueil",
        },
        TabReader => match locale {
            Locale::En => "Reader",
            Locale::Fr => "Lecture",
        },
        TabGallery => match locale {
            Locale::En => "Gallery",
            Locale::Fr => "Galerie",
        },
        TabRants => match locale {
            Locale::En => "Rants",
            Locale::Fr => "Billets",
        },
        TabSettings => match locale {
            Locale::En => "Settings",
            Locale::Fr => "Réglages",
        },
        Loading => match locale {
            Locale::En => "Loading...",
            Locale::Fr => "Chargement...",
        },
        ContinueReading => match locale {
            Locale::En => "Continue reading",
            Locale::Fr => "Continuer la lecture",
        },
        StartReading => match locale {
            Locale::En => "Start reading",
            Locale::Fr => "Commencer la lecture",
        },
        NotStartedReading => match locale {
            Locale::En => "You haven't started reading yet.",
            Locale::Fr => "Tu n'as pas encore commencé à lire.",
        },
        SearchStripsAndRants => match locale {
            Locale::En => "Search strips and rants...",
            Locale::Fr => "Rechercher des planches et des billets...",
        },
        NoMatches => match locale {
            Locale::En => "No matches.",
            Locale::Fr => "Aucun résultat.",
        },
        LatestRant => match locale {
            Locale::En => "Latest rant",
            Locale::Fr => "Dernier billet",
        },
        FilterAll => match locale {
            Locale::En => "All",
            Locale::Fr => "Tout",
        },
        FilterFavorites => match locale {
            Locale::En => "Favorites",
            Locale::Fr => "Favoris",
        },
        Back => match locale {
            Locale::En => "Back",
            Locale::Fr => "Retour",
        },
        SearchRants => match locale {
            Locale::En => "Search rants...",
            Locale::Fr => "Rechercher des billets...",
        },
        LoadingStrips => match locale {
            Locale::En => "Loading strips...",
            Locale::Fr => "Chargement des planches...",
        },
        NoStripsYet => match locale {
            Locale::En => "No strips yet — the daemon hasn't backfilled anything.",
            Locale::Fr => "Aucune planche pour l'instant — le démon n'a encore rien récupéré.",
        },
        Previous => match locale {
            Locale::En => "Previous",
            Locale::Fr => "Précédent",
        },
        Next => match locale {
            Locale::En => "Next",
            Locale::Fr => "Suivant",
        },
        DaemonSettingsHeading => match locale {
            Locale::En => "Daemon settings",
            Locale::Fr => "Réglages du démon",
        },
        BaseUrlLabel => match locale {
            Locale::En => "Base URL ",
            Locale::Fr => "URL de base ",
        },
        TokenLabel => match locale {
            Locale::En => "Token ",
            Locale::Fr => "Jeton ",
        },
        Save => match locale {
            Locale::En => "Save",
            Locale::Fr => "Enregistrer",
        },
        LoadChapters => match locale {
            Locale::En => "Load chapters",
            Locale::Fr => "Charger les chapitres",
        },
        EnableNotifications => match locale {
            Locale::En => "Enable notifications",
            Locale::Fr => "Activer les notifications",
        },
        DisableNotifications => match locale {
            Locale::En => "Disable notifications",
            Locale::Fr => "Désactiver les notifications",
        },
        Done => match locale {
            Locale::En => "Done.",
            Locale::Fr => "Terminé.",
        },
        TranslationHeading => match locale {
            Locale::En => "Translation and polling",
            Locale::Fr => "Traduction et synchronisation",
        },
        DeeplKeyLabel => match locale {
            Locale::En => "DeepL API key ",
            Locale::Fr => "Clé API DeepL ",
        },
        PollIntervalLabel => match locale {
            Locale::En => "Poll interval (minutes) ",
            Locale::Fr => "Intervalle de vérification (minutes) ",
        },
        Saved => match locale {
            Locale::En => "Saved.",
            Locale::Fr => "Enregistré.",
        },
        ChaptersHeading => match locale {
            Locale::En => "Chapters",
            Locale::Fr => "Chapitres",
        },
        NotLoadedYet => match locale {
            Locale::En => "Not loaded yet.",
            Locale::Fr => "Pas encore chargé.",
        },
        LanguageHeading => match locale {
            Locale::En => "Language",
            Locale::Fr => "Langue",
        },
        LanguageAuto => match locale {
            Locale::En => "Auto",
            Locale::Fr => "Auto",
        },
        CheckingDaemon => match locale {
            Locale::En => "Checking daemon...",
            Locale::Fr => "Vérification du démon...",
        },
        NeverCheckedYet => match locale {
            Locale::En => "Never checked yet",
            Locale::Fr => "Jamais vérifié",
        },
        CheckNow => match locale {
            Locale::En => "Check now",
            Locale::Fr => "Vérifier maintenant",
        },
        Checking => match locale {
            Locale::En => "Checking...",
            Locale::Fr => "Vérification...",
        },
    }
}

// Strings with interpolated data get their own function instead of a
// `Key` variant — a plain &'static str can't carry a runtime value.

pub fn last_read_subtitle(locale: Locale, number: i32, title: &str) -> String {
    match locale {
        Locale::En => format!("Last read: #{number} — {title}"),
        Locale::Fr => format!("Dernière lecture : #{number} — {title}"),
    }
}

pub fn strip_result(locale: Locale, number: i32, title: &str) -> String {
    match locale {
        Locale::En => format!("Strip #{number} — {title}"),
        Locale::Fr => format!("Planche #{number} — {title}"),
    }
}

pub fn rant_result(locale: Locale, title: &str) -> String {
    match locale {
        Locale::En => format!("Rant — {title}"),
        Locale::Fr => format!("Billet — {title}"),
    }
}

pub fn strip_count(locale: Locale, count: usize) -> String {
    match locale {
        Locale::En => format!("{count} strips"),
        Locale::Fr => format!("{count} planches"),
    }
}

pub fn rant_count(locale: Locale, count: usize) -> String {
    match locale {
        Locale::En => format!("{count} rants"),
        Locale::Fr => format!("{count} billets"),
    }
}

pub fn favorite_count(locale: Locale, count: usize) -> String {
    match locale {
        Locale::En => format!("{count} favorites"),
        Locale::Fr => format!("{count} favoris"),
    }
}

pub fn backfilling_status(locale: Locale, strip_number: i32, rant_number: i32) -> String {
    match locale {
        Locale::En => format!("Backfilling... (up to strip #{strip_number}, rant #{rant_number})"),
        Locale::Fr => format!(
            "Récupération en cours... (jusqu'à la planche #{strip_number}, le billet #{rant_number})"
        ),
    }
}

pub fn up_to_date_status(
    locale: Locale,
    strip_number: i32,
    rant_number: i32,
    checked_at: &str,
) -> String {
    match locale {
        Locale::En => format!(
            "Up to date (strip #{strip_number}, rant #{rant_number}) — checked {checked_at}"
        ),
        Locale::Fr => format!(
            "À jour (planche #{strip_number}, billet #{rant_number}) — vérifié {checked_at}"
        ),
    }
}
