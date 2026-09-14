//! Daemon settings screen: base URL/token entry, chapter list smoke test,
//! push notification opt-in/out, and daemon-side config (DeepL key, poll
//! interval). Split out of `main.rs` so the app shell can route between
//! this and other screens while sharing the same `base_url`/`token`
//! signals.

use leptos::prelude::*;
use leptos::task::spawn_local;

use crate::daemon_client;
use crate::push;
use crate::ripple;

#[component]
pub fn Settings(
    base_url: ReadSignal<String>,
    set_base_url: WriteSignal<String>,
    token: ReadSignal<String>,
    set_token: WriteSignal<String>,
) -> impl IntoView {
    let (chapters, set_chapters) = signal(None::<Result<Vec<String>, String>>);
    let (notifications_status, set_notifications_status) = signal(None::<Result<(), String>>);
    let (deepl_key, set_deepl_key) = signal(String::new());
    let (poll_interval, set_poll_interval) = signal(String::new());
    let (config_status, set_config_status) = signal(None::<Result<(), String>>);

    // Prefills the daemon-config fields, matching the desktop Settings
    // screen's own auto-load — skipped on a true first run (no base
    // URL/token yet) since there's nothing to fetch from.
    Effect::new(move |_| {
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        if base_url.trim().is_empty() || token.trim().is_empty() {
            return;
        }
        spawn_local(async move {
            if let Ok(config) = daemon_client::fetch_config(&base_url, &token).await {
                set_deepl_key.set(config.deepl_api_key);
                set_poll_interval.set(config.poll_interval_minutes.to_string());
            }
        });
    });

    let save_settings = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        crate::storage::save(&crate::storage::DaemonLink {
            base_url: base_url.get(),
            token: token.get(),
        });
    };

    let save_config = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        let base_url = base_url.get();
        let token = token.get();
        let deepl_key = deepl_key.get();
        let poll_interval = poll_interval.get();
        set_config_status.set(None);
        spawn_local(async move {
            let minutes = poll_interval.trim().parse::<u64>().ok();
            let result = daemon_client::update_config(&base_url, &token, Some(&deepl_key), minutes)
                .await
                .map(|_| ());
            set_config_status.set(Some(result));
        });
    };

    let enable_notifications = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        let base_url = base_url.get();
        let token = token.get();
        set_notifications_status.set(None);
        spawn_local(async move {
            let result = push::subscribe(&base_url, &token).await;
            set_notifications_status.set(Some(result));
        });
    };

    let disable_notifications = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        let base_url = base_url.get();
        let token = token.get();
        set_notifications_status.set(None);
        spawn_local(async move {
            let result = push::unsubscribe(&base_url, &token).await;
            set_notifications_status.set(Some(result));
        });
    };

    let load_chapters = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        let base_url = base_url.get();
        let token = token.get();
        set_chapters.set(None);
        spawn_local(async move {
            let result = daemon_client::fetch_chapters(&base_url, &token)
                .await
                .map(|chs| {
                    chs.into_iter()
                        .map(|c| format!("#{} — {}", c.number, c.title))
                        .collect()
                });
            set_chapters.set(Some(result));
        });
    };

    view! {
        <main>
            <h1>"Megatokyo"</h1>
            <section class="card">
                <h2>"Daemon settings"</h2>
                <label>
                    "Base URL "
                    <input
                        type="text"
                        placeholder="http://127.0.0.1:8420"
                        prop:value=move || base_url.get()
                        on:input=move |ev| set_base_url.set(event_target_value(&ev))
                    />
                </label>
                <label>
                    "Token "
                    <input
                        type="password"
                        prop:value=move || token.get()
                        on:input=move |ev| set_token.set(event_target_value(&ev))
                    />
                </label>
                <button class="btn btn-primary" on:click=save_settings>"Save"</button>
                <button class="btn" on:click=load_chapters>"Load chapters"</button>
                <button class="btn" on:click=enable_notifications>"Enable notifications"</button>
                <button class="btn" on:click=disable_notifications>"Disable notifications"</button>
                {move || match notifications_status.get() {
                    None => ().into_any(),
                    Some(Ok(())) => view! { <p class="status">"Done."</p> }.into_any(),
                    Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                }}
            </section>
            <section class="card">
                <h2>"Translation and polling"</h2>
                <label>
                    "DeepL API key "
                    <input
                        type="password"
                        prop:value=move || deepl_key.get()
                        on:input=move |ev| set_deepl_key.set(event_target_value(&ev))
                    />
                </label>
                <label>
                    "Poll interval (minutes) "
                    <input
                        type="text"
                        inputmode="numeric"
                        prop:value=move || poll_interval.get()
                        on:input=move |ev| set_poll_interval.set(event_target_value(&ev))
                    />
                </label>
                <button class="btn btn-primary" on:click=save_config>"Save"</button>
                {move || match config_status.get() {
                    None => ().into_any(),
                    Some(Ok(())) => view! { <p class="status">"Saved."</p> }.into_any(),
                    Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                }}
            </section>
            <section class="card">
                <h2>"Chapters"</h2>
                {move || match chapters.get() {
                    None => view! { <p class="status">"Not loaded yet."</p> }.into_any(),
                    Some(Ok(list)) => view! {
                        <ul>
                            {list.into_iter().map(|c| view! { <li>{c}</li> }).collect_view()}
                        </ul>
                    }.into_any(),
                    Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                }}
            </section>
        </main>
    }
}
