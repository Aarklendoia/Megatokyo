//! Strip reader: page-by-page navigation through `/strips`, favorite
//! toggle, and reading-progress save/resume — the PWA counterpart of the
//! desktop GUI's `qml/ReaderScreen.qml`.

use leptos::prelude::*;
use leptos::task::spawn_local;
use megatokyo_core::domain::Strip;

use crate::daemon_client;
use crate::ripple;

#[component]
pub fn Reader(base_url: ReadSignal<String>, token: ReadSignal<String>) -> impl IntoView {
    let (strips, set_strips) = signal(None::<Result<Vec<Strip>, String>>);
    let (index, set_index) = signal(0usize);
    let (favorite_numbers, set_favorite_numbers) = signal(Vec::<i32>::new());

    // Runs once, when the Reader is mounted (switching screens re-creates
    // it) — `get_untracked` so it isn't re-triggered by later base_url/token
    // edits made from the Settings screen.
    Effect::new(move |_| {
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        spawn_local(async move {
            let strips_result = daemon_client::fetch_strips(&base_url, &token).await;
            if let Ok(list) = &strips_result {
                if let Ok(Some(saved)) = daemon_client::fetch_progress(&base_url, &token).await {
                    if let Some(pos) = list.iter().position(|s| s.number == saved) {
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

    let go_prev = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        let i = index.get_untracked();
        if i > 0 {
            set_index.set(i - 1);
            save_progress_for(i - 1);
        }
    };

    let go_next = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        let len = strips
            .get_untracked()
            .and_then(|r| r.ok())
            .map(|list| list.len())
            .unwrap_or(0);
        let i = index.get_untracked();
        if i + 1 < len {
            set_index.set(i + 1);
            save_progress_for(i + 1);
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
            <h1>"Reader"</h1>
            {move || match strips.get() {
                None => view! { <p class="status">"Loading strips..."</p> }.into_any(),
                Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                Some(Ok(list)) if list.is_empty() => {
                    view! { <p class="status">"No strips yet — the daemon hasn't backfilled anything."</p> }
                        .into_any()
                }
                Some(Ok(_)) => view! {
                    <section class="card reader-strip">
                        {move || current().map(|strip| {
                            let src = daemon_client::image_url(&base_url.get(), &token.get(), strip.number);
                            let is_favorite = favorite_numbers.get().contains(&strip.number);
                            view! {
                                <p class="reader-title">{format!("#{} — {}", strip.number, strip.title)}</p>
                                <img class="reader-image" src=src alt=strip.title.clone() />
                                <div class="reader-nav">
                                    <button class="btn" on:click=go_prev>"Previous"</button>
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
                                    <button class="btn" on:click=go_next>"Next"</button>
                                </div>
                            }
                        })}
                    </section>
                }.into_any(),
            }}
        </main>
    }
}
