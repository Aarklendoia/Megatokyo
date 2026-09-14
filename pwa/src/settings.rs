//! Daemon settings screen: base URL/token entry, chapter list smoke test,
//! and push notification opt-in. Split out of `main.rs` so the app shell
//! can route between this and other screens while sharing the same
//! `base_url`/`token` signals.

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

    let save_settings = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        crate::storage::save(&crate::storage::DaemonLink {
            base_url: base_url.get(),
            token: token.get(),
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
                {move || match notifications_status.get() {
                    None => ().into_any(),
                    Some(Ok(())) => view! { <p class="status">"Notifications enabled."</p> }.into_any(),
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
