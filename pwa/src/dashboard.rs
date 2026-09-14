//! Home screen: strip/rant/favorite counts, "continue reading" and
//! "latest rant" shortcuts, and a search across strips and rants — the PWA
//! counterpart of the desktop GUI's `qml/DashboardScreen.qml`.

use leptos::prelude::*;
use leptos::task::spawn_local;
use megatokyo_core::domain::{Rant, Strip};

use crate::daemon_client;
use crate::i18n::{self, t, Key, Locale};
use crate::ripple;
use crate::Screen;

#[derive(Clone)]
struct Summary {
    strips: Vec<Strip>,
    rants: Vec<Rant>,
    favorite_count: usize,
    current_strip: Option<Strip>,
    latest_rant: Option<Rant>,
}

#[component]
pub fn Dashboard(
    base_url: ReadSignal<String>,
    token: ReadSignal<String>,
    locale: ReadSignal<Locale>,
    set_screen: WriteSignal<Screen>,
    set_requested_strip: WriteSignal<Option<i32>>,
    set_requested_rant: WriteSignal<Option<i32>>,
) -> impl IntoView {
    let (summary, set_summary) = signal(None::<Result<Summary, String>>);
    let (search, set_search) = signal(String::new());

    Effect::new(move |_| {
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        spawn_local(async move {
            let result = async {
                let strips = daemon_client::fetch_strips(&base_url, &token).await?;
                let rants = daemon_client::fetch_rants(&base_url, &token).await?;
                let favorites = daemon_client::fetch_favorites(&base_url, &token).await?;
                let progress = daemon_client::fetch_progress(&base_url, &token).await?;
                let current_strip =
                    progress.and_then(|number| strips.iter().find(|s| s.number == number).cloned());
                let latest_rant = rants.iter().max_by_key(|r| r.number).cloned();
                Ok(Summary {
                    favorite_count: favorites.len(),
                    current_strip,
                    latest_rant,
                    strips,
                    rants,
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
    let open_strip = move |number: i32| {
        set_requested_strip.set(Some(number));
        set_screen.set(Screen::Reader);
    };
    let open_rant = move |number: i32| {
        set_requested_rant.set(Some(number));
        set_screen.set(Screen::Rants);
    };

    view! {
        <main>
            <h1>"Megatokyo"</h1>
            {move || match summary.get() {
                None => view! { <p class="status">{t(locale.get(), Key::Loading)}</p> }.into_any(),
                Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                Some(Ok(data)) => {
                    let locale = locale.get();
                    let continue_label = if data.current_strip.is_some() {
                        t(locale, Key::ContinueReading)
                    } else {
                        t(locale, Key::StartReading)
                    };
                    let continue_subtitle = match &data.current_strip {
                        Some(strip) => i18n::last_read_subtitle(locale, strip.number, &strip.title),
                        None => t(locale, Key::NotStartedReading).to_string(),
                    };
                    let query = search.get().to_ascii_lowercase();
                    let matching_strips: Vec<Strip> = if query.is_empty() {
                        Vec::new()
                    } else {
                        data.strips
                            .iter()
                            .filter(|s| {
                                s.title.to_ascii_lowercase().contains(&query)
                                    || s.number.to_string().contains(&query)
                            })
                            .take(10)
                            .cloned()
                            .collect()
                    };
                    let matching_rants: Vec<Rant> = if query.is_empty() {
                        Vec::new()
                    } else {
                        data.rants
                            .iter()
                            .filter(|r| {
                                r.title.to_ascii_lowercase().contains(&query)
                                    || r.number.to_string().contains(&query)
                            })
                            .take(10)
                            .cloned()
                            .collect()
                    };

                    view! {
                        <input
                            type="text"
                            class="dashboard-search"
                            placeholder=t(locale, Key::SearchStripsAndRants)
                            prop:value=move || search.get()
                            on:input=move |ev| set_search.set(event_target_value(&ev))
                        />
                        {(!query.is_empty()).then(|| view! {
                            <section class="card dashboard-search-results">
                                {(matching_strips.is_empty() && matching_rants.is_empty()).then(|| {
                                    view! { <p class="status">{t(locale, Key::NoMatches)}</p> }
                                })}
                                <ul>
                                    {matching_strips.into_iter().map(|strip| {
                                        let number = strip.number;
                                        view! {
                                            <li on:click=move |_| open_strip(number)>
                                                {i18n::strip_result(locale, strip.number, &strip.title)}
                                            </li>
                                        }
                                    }).collect_view()}
                                    {matching_rants.into_iter().map(|rant| {
                                        let number = rant.number;
                                        view! {
                                            <li on:click=move |_| open_rant(number)>
                                                {i18n::rant_result(locale, &rant.title)}
                                            </li>
                                        }
                                    }).collect_view()}
                                </ul>
                            </section>
                        })}
                        <section class="card dashboard-continue">
                            <p>{continue_subtitle}</p>
                            <button class="btn btn-primary" on:click=open_reader>
                                {continue_label}
                            </button>
                        </section>
                        {data.latest_rant.map(|rant| {
                            let number = rant.number;
                            view! {
                                <section
                                    class="card dashboard-latest-rant"
                                    on:click=move |_| open_rant(number)
                                >
                                    <h2>{t(locale, Key::LatestRant)}</h2>
                                    <p>{rant.title.clone()}</p>
                                </section>
                            }
                        })}
                        <section class="card dashboard-stats">
                            <p>{i18n::strip_count(locale, data.strips.len())}</p>
                            <p>{i18n::rant_count(locale, data.rants.len())}</p>
                            <p>{i18n::favorite_count(locale, data.favorite_count)}</p>
                        </section>
                    }
                    .into_any()
                }
            }}
        </main>
    }
}
