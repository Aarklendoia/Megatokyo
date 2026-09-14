//! Small daemon health strip shown across every screen — the PWA
//! counterpart of the desktop shell's backfill/up-to-date indicator
//! (`qml/main.qml`) — plus a manual "Check now" button.

use std::time::Duration;

use gloo_timers::future::sleep;
use leptos::prelude::*;
use leptos::task::spawn_local;

use crate::daemon_client::{self, Status};
use crate::format::human_datetime;
use crate::ripple;

const REFRESH_INTERVAL: Duration = Duration::from_secs(20);

#[component]
pub fn StatusBar(base_url: ReadSignal<String>, token: ReadSignal<String>) -> impl IntoView {
    let (status, set_status) = signal(None::<Result<Status, String>>);
    let (checking, set_checking) = signal(false);

    let refresh = move || {
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        spawn_local(async move {
            let result = daemon_client::fetch_status(&base_url, &token).await;
            set_status.set(Some(result));
        });
    };

    // Refreshes once immediately, then every REFRESH_INTERVAL for as long
    // as the app is open — there's only ever one App/StatusBar instance,
    // so this loop naturally runs for the page's whole lifetime.
    Effect::new(move |_| {
        refresh();
        spawn_local(async move {
            loop {
                sleep(REFRESH_INTERVAL).await;
                refresh();
            }
        });
    });

    let check_now = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        set_checking.set(true);
        spawn_local(async move {
            let _ = daemon_client::trigger_check(&base_url, &token).await;
            // The daemon's poll loop runs the actual check asynchronously
            // in the background — this just gives it a moment before we
            // refresh, rather than reflecting nothing yet.
            sleep(Duration::from_secs(2)).await;
            let result = daemon_client::fetch_status(&base_url, &token).await;
            set_status.set(Some(result));
            set_checking.set(false);
        });
    };

    view! {
        <div class="status-bar">
            {move || match status.get() {
                None => view! { <span class="status-bar-text">"Checking daemon..."</span> }.into_any(),
                Some(Err(err)) => {
                    view! { <span class="status-bar-text error">{err}</span> }.into_any()
                }
                Some(Ok(s)) => {
                    let text = if s.backfilling {
                        format!(
                            "Backfilling... (up to strip #{}, rant #{})",
                            s.last_strip_number, s.last_rant_number
                        )
                    } else {
                        match &s.last_check {
                            Some(when) => format!(
                                "Up to date (strip #{}, rant #{}) — checked {}",
                                s.last_strip_number,
                                s.last_rant_number,
                                human_datetime(when)
                            ),
                            None => "Never checked yet".to_string(),
                        }
                    };
                    view! { <span class="status-bar-text">{text}</span> }.into_any()
                }
            }}
            <button
                class="btn status-bar-check"
                on:click=check_now
                disabled=move || checking.get()
            >
                {move || if checking.get() { "Checking..." } else { "Check now" }}
            </button>
        </div>
    }
}
