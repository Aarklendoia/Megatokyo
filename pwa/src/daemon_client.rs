//! Thin client for the `megatokyo-daemon` HTTP API, mirroring the
//! `x-megatokyo-daemon-token` header convention the desktop GUI already uses
//! (see `gui/src/background.rs`'s `fetch_status`).

use megatokyo_core::domain::{Chapter, Favorite, Rant, Strip};
use serde::Deserialize;

const TOKEN_HEADER: &str = "x-megatokyo-daemon-token";

async fn ok_or_status(response: &gloo_net::http::Response) -> Result<(), String> {
    if response.ok() {
        Ok(())
    } else {
        Err(format!("daemon returned {}", response.status()))
    }
}

pub async fn fetch_chapters(base_url: &str, token: &str) -> Result<Vec<Chapter>, String> {
    let url = format!("{}/chapters", base_url.trim_end_matches('/'));
    let response = gloo_net::http::Request::get(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;

    if !response.ok() {
        return Err(format!("daemon returned {}", response.status()));
    }

    response
        .json::<Vec<Chapter>>()
        .await
        .map_err(|err| err.to_string())
}

pub async fn fetch_strips(base_url: &str, token: &str) -> Result<Vec<Strip>, String> {
    let url = format!("{}/strips", base_url.trim_end_matches('/'));
    let response = gloo_net::http::Request::get(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;

    if !response.ok() {
        return Err(format!("daemon returned {}", response.status()));
    }

    response
        .json::<Vec<Strip>>()
        .await
        .map_err(|err| err.to_string())
}

/// `<img>`-style URL for a strip's art — `/image` is the one route that
/// accepts the token as a `?token=` query param instead of the header,
/// specifically so it can be used directly as an `img src` (see
/// `daemon/src/control.rs`'s `is_authorized`, mirrored by `Api.qml`'s
/// `imageUrl`).
pub fn image_url(base_url: &str, token: &str, number: i32) -> String {
    format!(
        "{}/image?number={number}&token={}",
        base_url.trim_end_matches('/'),
        js_sys::encode_uri_component(token),
    )
}

pub async fn fetch_favorites(base_url: &str, token: &str) -> Result<Vec<Favorite>, String> {
    let url = format!("{}/favorites", base_url.trim_end_matches('/'));
    let response = gloo_net::http::Request::get(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;

    if !response.ok() {
        return Err(format!("daemon returned {}", response.status()));
    }

    response
        .json::<Vec<Favorite>>()
        .await
        .map_err(|err| err.to_string())
}

pub async fn add_favorite(base_url: &str, token: &str, number: i32) -> Result<(), String> {
    let url = format!(
        "{}/favorites?number={number}",
        base_url.trim_end_matches('/')
    );
    let response = gloo_net::http::Request::post(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;
    ok_or_status(&response).await
}

pub async fn remove_favorite(base_url: &str, token: &str, number: i32) -> Result<(), String> {
    let url = format!(
        "{}/favorites?number={number}",
        base_url.trim_end_matches('/')
    );
    let response = gloo_net::http::Request::delete(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;
    ok_or_status(&response).await
}

#[derive(Deserialize)]
struct ProgressResponse {
    strip_number: Option<i32>,
}

pub async fn fetch_progress(base_url: &str, token: &str) -> Result<Option<i32>, String> {
    let url = format!("{}/progress", base_url.trim_end_matches('/'));
    let response = gloo_net::http::Request::get(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;

    if !response.ok() {
        return Err(format!("daemon returned {}", response.status()));
    }

    response
        .json::<ProgressResponse>()
        .await
        .map(|body| body.strip_number)
        .map_err(|err| err.to_string())
}

pub async fn save_progress(base_url: &str, token: &str, number: i32) -> Result<(), String> {
    let url = format!(
        "{}/progress?number={number}",
        base_url.trim_end_matches('/')
    );
    let response = gloo_net::http::Request::post(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;
    ok_or_status(&response).await
}

#[derive(Deserialize)]
struct VapidPublicKeyResponse {
    vapid_public_key: String,
}

pub async fn fetch_vapid_public_key(base_url: &str, token: &str) -> Result<String, String> {
    let url = format!("{}/push/vapid-public-key", base_url.trim_end_matches('/'));
    let response = gloo_net::http::Request::get(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;

    if !response.ok() {
        return Err(format!("daemon returned {}", response.status()));
    }

    response
        .json::<VapidPublicKeyResponse>()
        .await
        .map(|body| body.vapid_public_key)
        .map_err(|err| err.to_string())
}

/// `endpoint`/`p256dh`/`auth` are exactly what the browser's
/// `PushSubscription.toJSON()` gives — see `push::subscribe`. Only
/// `endpoint` needs percent-encoding: `p256dh`/`auth` are already
/// URL-safe base64.
pub async fn push_subscribe(
    base_url: &str,
    token: &str,
    endpoint: &str,
    p256dh: &str,
    auth: &str,
) -> Result<(), String> {
    let encoded_endpoint = js_sys::encode_uri_component(endpoint);
    let url = format!(
        "{}/push/subscribe?endpoint={encoded_endpoint}&p256dh={p256dh}&auth={auth}",
        base_url.trim_end_matches('/'),
    );
    let response = gloo_net::http::Request::post(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;

    if !response.ok() {
        return Err(format!("daemon returned {}", response.status()));
    }
    Ok(())
}

pub async fn fetch_rants(base_url: &str, token: &str) -> Result<Vec<Rant>, String> {
    let url = format!("{}/rants", base_url.trim_end_matches('/'));
    let response = gloo_net::http::Request::get(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;

    if !response.ok() {
        return Err(format!("daemon returned {}", response.status()));
    }

    response
        .json::<Vec<Rant>>()
        .await
        .map_err(|err| err.to_string())
}

/// `lang: None` (or `Some("en")`) gets the original content back; any other
/// language code asks the daemon to DeepL-translate it. The response is a
/// superset of `Rant`'s fields (it also carries `lang`), which `Rant`'s
/// `Deserialize` happily ignores.
pub async fn fetch_rant(
    base_url: &str,
    token: &str,
    number: i32,
    lang: Option<&str>,
) -> Result<Rant, String> {
    let url = match lang {
        Some(lang) => format!(
            "{}/rant?number={number}&lang={lang}",
            base_url.trim_end_matches('/')
        ),
        None => format!("{}/rant?number={number}", base_url.trim_end_matches('/')),
    };
    let response = gloo_net::http::Request::get(&url)
        .header(TOKEN_HEADER, token)
        .send()
        .await
        .map_err(|err| err.to_string())?;

    if !response.ok() {
        return Err(format!("daemon returned {}", response.status()));
    }

    response.json::<Rant>().await.map_err(|err| err.to_string())
}
