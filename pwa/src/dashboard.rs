//! Home screen: strip/favorite counts and a "continue reading" shortcut
//! driven by `/progress`. A minimal slice of the desktop GUI's Dashboard
//! (`qml/DashboardScreen.qml`) — the last-rant card and search land once
//! the Rants screen exists (tracked separately).

use leptos::prelude::*;
use leptos::task::spawn_local;
use megatokyo_core::domain::Strip;

use crate::daemon_client;
use crate::ripple;
use crate::Screen;

#[derive(Clone)]
struct Summary {
    strip_count: usize,
    favorite_count: usize,
    current: Option<Strip>,
}

#[component]
pub fn Dashboard(
    base_url: ReadSignal<String>,
    token: ReadSignal<String>,
    set_screen: WriteSignal<Screen>,
) -> impl IntoView {
    let (summary, set_summary) = signal(None::<Result<Summary, String>>);

    Effect::new(move |_| {
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        spawn_local(async move {
            let result = async {
                let strips = daemon_client::fetch_strips(&base_url, &token).await?;
                let favorites = daemon_client::fetch_favorites(&base_url, &token).await?;
                let progress = daemon_client::fetch_progress(&base_url, &token).await?;
                let current =
                    progress.and_then(|number| strips.iter().find(|s| s.number == number).cloned());
                Ok(Summary {
                    strip_count: strips.len(),
                    favorite_count: favorites.len(),
                    current,
                })
            }
            .await;
            set_summary.set(Some(result));
        });
    });

    let open_reader = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        set_screen.set(Screen::Reader);
    };

    view! {
        <main>
            <h1>"Megatokyo"</h1>
            {move || match summary.get() {
                None => view! { <p class="status">"Loading..."</p> }.into_any(),
                Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                Some(Ok(data)) => {
                    let continue_label = if data.current.is_some() {
                        "Continue reading"
                    } else {
                        "Start reading"
                    };
                    let subtitle = match &data.current {
                        Some(strip) => format!("Last read: #{} — {}", strip.number, strip.title),
                        None => "You haven't started reading yet.".to_string(),
                    };
                    view! {
                        <section class="card dashboard-continue">
                            <p>{subtitle}</p>
                            <button class="btn btn-primary" on:click=open_reader>
                                {continue_label}
                            </button>
                        </section>
                        <section class="card dashboard-stats">
                            <p>{format!("{} strips", data.strip_count)}</p>
                            <p>{format!("{} favorites", data.favorite_count)}</p>
                        </section>
                    }
                    .into_any()
                }
            }}
        </main>
    }
}
