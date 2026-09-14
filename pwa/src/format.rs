//! Human-readable renderings of the RFC 3339 timestamps the daemon returns
//! (e.g. `"2026-09-14T13:27:29Z"`, or `"2024-11-16T00:00:00Z"` for a
//! date-only value) — shown as-is those read as machine output, not
//! something a person parses at a glance. `Date`'s own locale-aware
//! formatting (the user's `navigator.language`, so it also respects their
//! timezone) turns them into normal-looking dates/times.

fn parse(rfc3339: &str) -> Option<js_sys::Date> {
    let date = js_sys::Date::new(&wasm_bindgen::JsValue::from_str(rfc3339));
    (!date.get_time().is_nan()).then_some(date)
}

fn locale() -> String {
    web_sys::window()
        .and_then(|w| w.navigator().language())
        .unwrap_or_else(|| "en-US".to_string())
}

/// Full date and time, e.g. for "last checked at ...".
pub fn human_datetime(rfc3339: &str) -> String {
    let Some(date) = parse(rfc3339) else {
        return rfc3339.to_string();
    };
    date.to_locale_string(&locale(), &wasm_bindgen::JsValue::UNDEFINED)
        .as_string()
        .unwrap_or_else(|| rfc3339.to_string())
}

/// Date only, e.g. for a rant's publish date.
pub fn human_date(rfc3339: &str) -> String {
    let Some(date) = parse(rfc3339) else {
        return rfc3339.to_string();
    };
    date.to_locale_date_string(&locale(), &wasm_bindgen::JsValue::UNDEFINED)
        .as_string()
        .unwrap_or_else(|| rfc3339.to_string())
}
