//! Thumbnail grid of strips, filterable by category or favorites — the
//! PWA counterpart of the desktop GUI's `qml/GalleryScreen.qml`. Tapping a
//! thumbnail opens it in the Reader.

use leptos::prelude::*;
use leptos::task::spawn_local;
use megatokyo_core::domain::Strip;

use crate::daemon_client;
use crate::ripple;
use crate::Screen;

#[derive(Debug, Clone, PartialEq, Eq)]
enum Filter {
    All,
    Favorites,
    Category(String),
}

#[component]
pub fn Gallery(
    base_url: ReadSignal<String>,
    token: ReadSignal<String>,
    set_screen: WriteSignal<Screen>,
    set_requested_strip: WriteSignal<Option<i32>>,
) -> impl IntoView {
    let (strips, set_strips) = signal(None::<Result<Vec<Strip>, String>>);
    let (favorite_numbers, set_favorite_numbers) = signal(Vec::<i32>::new());
    let (filter, set_filter) = signal(Filter::All);

    Effect::new(move |_| {
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        spawn_local(async move {
            let strips_result = daemon_client::fetch_strips(&base_url, &token).await;
            if strips_result.is_ok() {
                if let Ok(favorites) = daemon_client::fetch_favorites(&base_url, &token).await {
                    set_favorite_numbers
                        .set(favorites.into_iter().map(|f| f.strip_number).collect());
                }
            }
            set_strips.set(Some(strips_result));
        });
    });

    let toggle_favorite = move |ev: web_sys::MouseEvent, number: i32| {
        // Don't let the tap also bubble up to the thumbnail's own
        // click handler, which would open the Reader as well.
        ev.stop_propagation();
        ripple::spawn(&ev);
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

    let open_strip = move |number: i32| {
        set_requested_strip.set(Some(number));
        set_screen.set(Screen::Reader);
    };

    view! {
        <main>
            <h1>"Gallery"</h1>
            {move || match strips.get() {
                None => view! { <p class="status">"Loading..."</p> }.into_any(),
                Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                Some(Ok(list)) => {
                    let mut categories = Vec::new();
                    for strip in &list {
                        if !categories.contains(&strip.category) {
                            categories.push(strip.category.clone());
                        }
                    }

                    view! {
                        <div class="gallery-filters">
                            <button
                                class=move || if filter.get() == Filter::All { "btn tab active" } else { "btn tab" }
                                on:click=move |ev| { ripple::spawn(&ev); set_filter.set(Filter::All); }
                            >
                                "All"
                            </button>
                            <button
                                class=move || if filter.get() == Filter::Favorites { "btn tab active" } else { "btn tab" }
                                on:click=move |ev| { ripple::spawn(&ev); set_filter.set(Filter::Favorites); }
                            >
                                "Favorites"
                            </button>
                            {categories.into_iter().map(|category| {
                                let for_class = category.clone();
                                let for_click = category.clone();
                                view! {
                                    <button
                                        class=move || if filter.get() == Filter::Category(for_class.clone()) {
                                            "btn tab active"
                                        } else {
                                            "btn tab"
                                        }
                                        on:click=move |ev| {
                                            ripple::spawn(&ev);
                                            set_filter.set(Filter::Category(for_click.clone()));
                                        }
                                    >
                                        {category.clone()}
                                    </button>
                                }
                            }).collect_view()}
                        </div>
                        <div class="gallery-grid">
                            {list.into_iter().filter(move |strip| match filter.get() {
                                Filter::All => true,
                                Filter::Favorites => favorite_numbers.get().contains(&strip.number),
                                Filter::Category(category) => strip.category == category,
                            }).map(|strip| {
                                let number = strip.number;
                                let src = daemon_client::image_url(
                                    &base_url.get_untracked(),
                                    &token.get_untracked(),
                                    number,
                                );
                                let is_favorite = move || favorite_numbers.get().contains(&number);
                                view! {
                                    <div class="gallery-item" on:click=move |_| open_strip(number)>
                                        <img
                                            class="gallery-thumb"
                                            src=src
                                            loading="lazy"
                                            alt=strip.title.clone()
                                        />
                                        <button
                                            class=move || if is_favorite() {
                                                "gallery-favorite active"
                                            } else {
                                                "gallery-favorite"
                                            }
                                            on:click=move |ev| toggle_favorite(ev, number)
                                        >
                                            {move || if is_favorite() { "♥" } else { "♡" }}
                                        </button>
                                    </div>
                                }
                            }).collect_view()}
                        </div>
                    }
                    .into_any()
                }
            }}
        </main>
    }
}
