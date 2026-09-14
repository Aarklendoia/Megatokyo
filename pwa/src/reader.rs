//! Strip reader: page-by-page navigation through `/strips`, favorite
//! toggle, and reading-progress save/resume — the PWA counterpart of the
//! desktop GUI's `qml/ReaderScreen.qml`.

use leptos::prelude::*;
use leptos::task::spawn_local;
use megatokyo_core::domain::Strip;

use crate::daemon_client;
use crate::i18n::{t, Key, Locale};
use crate::ripple;

#[component]
pub fn Reader(
    base_url: ReadSignal<String>,
    token: ReadSignal<String>,
    locale: ReadSignal<Locale>,
    /// Set by the Gallery before switching here, to open a tapped strip
    /// instead of resuming the daemon's saved progress. Cleared back to
    /// `None` once consumed, so a later plain switch to Reader (e.g. from
    /// the tab bar) resumes progress again as usual.
    requested_strip: ReadSignal<Option<i32>>,
    set_requested_strip: WriteSignal<Option<i32>>,
) -> impl IntoView {
    let (strips, set_strips) = signal(None::<Result<Vec<Strip>, String>>);
    let (index, set_index) = signal(0usize);
    let (favorite_numbers, set_favorite_numbers) = signal(Vec::<i32>::new());

    // Runs once, when the Reader is mounted (switching screens re-creates
    // it) — `get_untracked` so it isn't re-triggered by later base_url/token
    // edits made from the Settings screen.
    Effect::new(move |_| {
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        let requested = requested_strip.get_untracked();
        set_requested_strip.set(None);
        spawn_local(async move {
            let strips_result = daemon_client::fetch_strips(&base_url, &token).await;
            if let Ok(list) = &strips_result {
                let wanted = match requested {
                    Some(number) => Some(number),
                    None => daemon_client::fetch_progress(&base_url, &token)
                        .await
                        .ok()
                        .flatten(),
                };
                if let Some(number) = wanted {
                    if let Some(pos) = list.iter().position(|s| s.number == number) {
                        set_index.set(pos);
                    }
                }
                if let Ok(favorites) = daemon_client::fetch_favorites(&base_url, &token).await {
                    set_favorite_numbers
                        .set(favorites.into_iter().map(|f| f.strip_number).collect());
                }
            }
            set_strips.set(Some(strips_result));
        });
    });

    let save_progress_for = move |i: usize| {
        if let Some(Ok(list)) = strips.get_untracked() {
            if let Some(strip) = list.get(i) {
                let base_url = base_url.get_untracked();
                let token = token.get_untracked();
                let number = strip.number;
                spawn_local(async move {
                    let _ = daemon_client::save_progress(&base_url, &token, number).await;
                });
            }
        }
    };

    // Shared by the Previous/Next buttons and the swipe gesture below, so
    // both agree on bounds-checking and progress-saving.
    let navigate = move |delta: i32| {
        let len = strips
            .get_untracked()
            .and_then(|r| r.ok())
            .map(|list| list.len())
            .unwrap_or(0);
        let new_index = index.get_untracked() as i32 + delta;
        if new_index >= 0 && (new_index as usize) < len {
            set_index.set(new_index as usize);
            save_progress_for(new_index as usize);
        }
    };

    let go_prev = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        navigate(-1);
    };

    let go_next = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        navigate(1);
    };

    // Touch/pen/mouse swipe on the strip image — Pointer Events unify all
    // three input types instead of needing separate touch/mouse handlers.
    // A short horizontal drag (more horizontal than vertical, past
    // SWIPE_THRESHOLD_PX) pages forward/back; anything else (a tap, a
    // vertical scroll attempt) is left alone.
    const SWIPE_THRESHOLD_PX: f64 = 40.0;
    let (swipe_start, set_swipe_start) = signal(None::<(f64, f64)>);

    let on_pointer_down = move |ev: web_sys::PointerEvent| {
        set_swipe_start.set(Some((ev.client_x() as f64, ev.client_y() as f64)));
    };
    let on_pointer_up = move |ev: web_sys::PointerEvent| {
        let Some((start_x, start_y)) = swipe_start.get_untracked() else {
            return;
        };
        set_swipe_start.set(None);
        let dx = ev.client_x() as f64 - start_x;
        let dy = ev.client_y() as f64 - start_y;
        if dx.abs() > SWIPE_THRESHOLD_PX && dx.abs() > dy.abs() {
            navigate(if dx < 0.0 { 1 } else { -1 });
        }
    };

    let toggle_favorite = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        let Some(Ok(list)) = strips.get_untracked() else {
            return;
        };
        let i = index.get_untracked();
        let Some(strip) = list.get(i) else {
            return;
        };
        let number = strip.number;
        let is_favorite = favorite_numbers.get_untracked().contains(&number);
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        spawn_local(async move {
            let result = if is_favorite {
                daemon_client::remove_favorite(&base_url, &token, number).await
            } else {
                daemon_client::add_favorite(&base_url, &token, number).await
            };
            if result.is_ok() {
                set_favorite_numbers.update(|favorites| {
                    if is_favorite {
                        favorites.retain(|n| *n != number);
                    } else {
                        favorites.push(number);
                    }
                });
            }
        });
    };

    let current = move || {
        strips
            .get()
            .and_then(|r| r.ok())
            .and_then(|list| list.get(index.get()).cloned())
    };

    view! {
        <main class="reader">
            <h1>{move || t(locale.get(), Key::TabReader)}</h1>
            {move || match strips.get() {
                None => view! { <p class="status">{t(locale.get(), Key::LoadingStrips)}</p> }.into_any(),
                Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                Some(Ok(list)) if list.is_empty() => {
                    view! { <p class="status">{t(locale.get(), Key::NoStripsYet)}</p> }
                        .into_any()
                }
                Some(Ok(_)) => view! {
                    <section class="card reader-strip">
                        {move || current().map(|strip| {
                            let locale = locale.get();
                            let src = daemon_client::image_url(&base_url.get(), &token.get(), strip.number);
                            let is_favorite = favorite_numbers.get().contains(&strip.number);
                            view! {
                                <p class="reader-title">{format!("#{} — {}", strip.number, strip.title)}</p>
                                <div
                                    class="reader-swipe"
                                    on:pointerdown=on_pointer_down
                                    on:pointerup=on_pointer_up
                                >
                                    <img class="reader-image" src=src alt=strip.title.clone() />
                                </div>
                                <div class="reader-nav">
                                    <button class="btn" on:click=go_prev>{t(locale, Key::Previous)}</button>
                                    <button
                                        class=move || if is_favorite {
                                            "btn btn-favorite active"
                                        } else {
                                            "btn btn-favorite"
                                        }
                                        on:click=toggle_favorite
                                    >
                                        {if is_favorite { "♥" } else { "♡" }}
                                    </button>
                                    <button class="btn" on:click=go_next>{t(locale, Key::Next)}</button>
                                </div>
                            }
                        })}
                    </section>
                }.into_any(),
            }}
        </main>
    }
}
