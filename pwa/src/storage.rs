//! Persists the remote daemon's base URL and auth token in `localStorage`.
//! The PWA has no way to spawn a local daemon process the way the desktop
//! GUI can, so it always operates in "remote daemon" mode — these are the
//! only two settings it needs, entered once by the user.

use gloo_storage::{LocalStorage, Storage};

const BASE_URL_KEY: &str = "megatokyo_daemon_base_url";
const TOKEN_KEY: &str = "megatokyo_daemon_token";
const LOCALE_KEY: &str = "megatokyo_locale_override";

#[derive(Debug, Clone, Default)]
pub struct DaemonLink {
    pub base_url: String,
    pub token: String,
}

pub fn load() -> DaemonLink {
    DaemonLink {
        base_url: LocalStorage::get(BASE_URL_KEY).unwrap_or_default(),
        token: LocalStorage::get(TOKEN_KEY).unwrap_or_default(),
    }
}

pub fn save(link: &DaemonLink) {
    let _ = LocalStorage::set(BASE_URL_KEY, &link.base_url);
    let _ = LocalStorage::set(TOKEN_KEY, &link.token);
}

/// A user-picked language code (e.g. `"fr"`), overriding the
/// browser-detected one — `None` means "follow the browser".
pub fn load_locale_override() -> Option<String> {
    LocalStorage::get(LOCALE_KEY).ok()
}

pub fn save_locale_override(code: Option<&str>) {
    match code {
        Some(code) => {
            let _ = LocalStorage::set(LOCALE_KEY, code);
        }
        None => LocalStorage::delete(LOCALE_KEY),
    }
}
