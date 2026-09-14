mod daemon_client;
mod dashboard;
mod push;
mod reader;
mod ripple;
mod settings;
mod storage;

use leptos::prelude::*;

use dashboard::Dashboard;
use reader::Reader;
use settings::Settings;

/// First run (no base URL/token saved yet) has nothing to show on a
/// Dashboard, so it lands on Settings instead; every later run starts on
/// the Dashboard, matching the desktop GUI's shell.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum Screen {
    Dashboard,
    Reader,
    Settings,
}

fn main() {
    console_error_panic_hook::set_once();
    leptos::mount::mount_to_body(App);
}

#[component]
fn App() -> impl IntoView {
    let initial = storage::load();
    let is_configured = !initial.base_url.trim().is_empty() && !initial.token.trim().is_empty();
    let (base_url, set_base_url) = signal(initial.base_url);
    let (token, set_token) = signal(initial.token);
    let (screen, set_screen) = signal(if is_configured {
        Screen::Dashboard
    } else {
        Screen::Settings
    });

    let go_dashboard = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        set_screen.set(Screen::Dashboard);
    };
    let go_reader = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        set_screen.set(Screen::Reader);
    };
    let go_settings = move |ev: web_sys::MouseEvent| {
        ripple::spawn(&ev);
        set_screen.set(Screen::Settings);
    };

    view! {
        <nav class="tabs">
            <button
                class=move || if screen.get() == Screen::Dashboard { "btn tab active" } else { "btn tab" }
                on:click=go_dashboard
            >
                "Home"
            </button>
            <button
                class=move || if screen.get() == Screen::Reader { "btn tab active" } else { "btn tab" }
                on:click=go_reader
            >
                "Reader"
            </button>
            <button
                class=move || if screen.get() == Screen::Settings { "btn tab active" } else { "btn tab" }
                on:click=go_settings
            >
                "Settings"
            </button>
        </nav>
        {move || match screen.get() {
            Screen::Dashboard => view! { <Dashboard base_url token set_screen /> }.into_any(),
            Screen::Reader => view! { <Reader base_url token /> }.into_any(),
            Screen::Settings => view! {
                <Settings base_url set_base_url token set_token />
            }
            .into_any(),
        }}
    }
}
