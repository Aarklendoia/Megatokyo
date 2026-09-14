//! Blog-post list + detail, with an EN/system-language translation toggle
//! — the PWA counterpart of the desktop GUI's `qml/RantsScreen.qml`.

use std::collections::HashMap;

use leptos::prelude::*;
use leptos::task::spawn_local;
use megatokyo_core::domain::Rant;

use crate::daemon_client;
use crate::format::human_date;
use crate::ripple;

/// The primary language subtag from `navigator.language` (e.g. `"fr"` from
/// `"fr-FR"`), or `None` if it's already English or unavailable — matching
/// `RantsScreen.qml`'s "EN / system language" toggle, not a full locale
/// picker.
fn system_lang() -> Option<String> {
    let lang = web_sys::window()?.navigator().language()?;
    let primary = lang.split('-').next().unwrap_or(&lang).to_ascii_lowercase();
    (primary != "en").then_some(primary)
}

#[component]
pub fn Rants(
    base_url: ReadSignal<String>,
    token: ReadSignal<String>,
    /// Set by the Dashboard before switching here, to open a specific rant
    /// directly instead of showing the list. Cleared back to `None` once
    /// consumed.
    requested_rant: ReadSignal<Option<i32>>,
    set_requested_rant: WriteSignal<Option<i32>>,
) -> impl IntoView {
    let (rants, set_rants) = signal(None::<Result<Vec<Rant>, String>>);
    let (search, set_search) = signal(String::new());
    let (selected, set_selected) = signal(requested_rant.get_untracked());
    let (translated, set_translated) = signal(false);
    let (detail, set_detail) = signal(None::<Result<Rant, String>>);
    let (cache, set_cache) = signal(HashMap::<(i32, bool), Rant>::new());
    let target_lang = system_lang();
    let has_target_lang = target_lang.is_some();
    let target_lang_for_fetch = target_lang.clone();
    set_requested_rant.set(None);

    Effect::new(move |_| {
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        spawn_local(async move {
            let result = daemon_client::fetch_rants(&base_url, &token).await;
            set_rants.set(Some(result));
        });
    });

    // Re-fetches whenever the selected rant or the EN/translated toggle
    // changes, serving from `cache` when that exact (number, translated)
    // pair was already fetched this session.
    Effect::new(move |_| {
        let Some(number) = selected.get() else {
            set_detail.set(None);
            return;
        };
        let want_translated = translated.get();
        if let Some(cached) = cache
            .get_untracked()
            .get(&(number, want_translated))
            .cloned()
        {
            set_detail.set(Some(Ok(cached)));
            return;
        }
        let base_url = base_url.get_untracked();
        let token = token.get_untracked();
        let lang = want_translated
            .then(|| target_lang_for_fetch.clone())
            .flatten();
        set_detail.set(None);
        spawn_local(async move {
            let result =
                daemon_client::fetch_rant(&base_url, &token, number, lang.as_deref()).await;
            if let Ok(rant) = &result {
                set_cache.update(|c| {
                    c.insert((number, want_translated), rant.clone());
                });
            }
            set_detail.set(Some(result));
        });
    });

    let open = move |number: i32| {
        set_translated.set(false);
        set_selected.set(Some(number));
    };
    let back = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        set_selected.set(None);
    };
    let toggle_lang = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        set_translated.update(|t| *t = !*t);
    };

    view! {
        <main>
            <h1>"Rants"</h1>
            {move || match selected.get() {
                None => match rants.get() {
                    None => view! { <p class="status">"Loading..."</p> }.into_any(),
                    Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                    Some(Ok(list)) => {
                        let query = search.get().to_ascii_lowercase();
                        view! {
                            <input
                                type="text"
                                class="rants-search"
                                placeholder="Search rants..."
                                prop:value=move || search.get()
                                on:input=move |ev| set_search.set(event_target_value(&ev))
                            />
                            <ul class="rants-list">
                                {list.into_iter().filter(|rant| {
                                    query.is_empty()
                                        || rant.title.to_ascii_lowercase().contains(&query)
                                        || rant.number.to_string().contains(&query)
                                }).map(|rant| {
                                    let number = rant.number;
                                    view! {
                                        <li on:click=move |_| open(number)>
                                            <span class="rants-item-title">{rant.title.clone()}</span>
                                            <span class="rants-item-date">{human_date(&rant.publish_date)}</span>
                                        </li>
                                    }
                                }).collect_view()}
                            </ul>
                        }
                        .into_any()
                    }
                },
                Some(_) => view! {
                    <div class="card rants-detail">
                        <button class="btn" on:click=back>"Back"</button>
                        {has_target_lang.then(|| {
                            let target_lang = target_lang.clone();
                            let label = move || if translated.get() {
                                target_lang.clone().unwrap_or_default()
                            } else {
                                "EN".to_string()
                            };
                            view! {
                                <button class="btn" on:click=toggle_lang>{label}</button>
                            }
                        })}
                        {move || match detail.get() {
                            None => view! { <p class="status">"Loading..."</p> }.into_any(),
                            Some(Err(err)) => view! { <p class="status error">{err}</p> }.into_any(),
                            Some(Ok(rant)) => view! {
                                <h2>{rant.title.clone()}</h2>
                                <p class="rants-meta">
                                    {format!("{} — {}", rant.author, human_date(&rant.publish_date))}
                                </p>
                                <div class="rants-content" inner_html=rant.content.clone()></div>
                            }
                            .into_any(),
                        }}
                    </div>
                }
                .into_any(),
            }}
        </main>
    }
}
