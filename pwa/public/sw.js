// Minimal service worker: just enough for installability (a page needs an
// active service worker for the browser to consider it installable). Full
// offline strip/rant caching is a follow-up, not part of the scaffold.
const CACHE_NAME = "megatokyo-shell-v1";
const SHELL_URLS = ["/", "/manifest.json", "/icon.svg"];

self.addEventListener("install", (event) => {
  event.waitUntil(
    caches
      .open(CACHE_NAME)
      .then((cache) => cache.addAll(SHELL_URLS))
      // Without this, a new sw.js sits "waiting" until every tab showing
      // the old one closes — so a deploy's fix (like #84) wouldn't reach
      // an already-open tab without the user manually closing it first.
      .then(() => self.skipWaiting())
  );
});

self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches
      .keys()
      .then((keys) =>
        Promise.all(keys.filter((key) => key !== CACHE_NAME).map((key) => caches.delete(key)))
      )
      // Takes control of already-open tabs immediately instead of only
      // the next navigation, pairing with skipWaiting() above.
      .then(() => self.clients.claim())
  );
});

// Navigations (the HTML shell) are network-first: index.html references
// content-hashed JS/WASM filenames, so a cached copy from an earlier
// deploy would keep pointing at assets a later deploy no longer serves —
// a permanent NetworkError/SRI-mismatch trap (#84) once installed. Only
// fall back to the cache when the network is unreachable (offline).
// Everything else (the hashed assets themselves, effectively immutable)
// stays cache-first for speed and offline use.
self.addEventListener("fetch", (event) => {
  if (event.request.mode === "navigate") {
    event.respondWith(
      fetch(event.request)
        .then((response) => {
          const copy = response.clone();
          caches.open(CACHE_NAME).then((cache) => cache.put(event.request, copy));
          return response;
        })
        .catch(() => caches.match(event.request))
    );
    return;
  }
  event.respondWith(
    caches.match(event.request).then((cached) => cached || fetch(event.request))
  );
});

// Payload shape is `{"title": ..., "url": ...}` — see daemon/src/push.rs's
// `send_one`. `event.waitUntil` keeps the service worker alive until the
// notification is actually shown; without it the browser can kill the
// worker mid-`showNotification` on some platforms.
self.addEventListener("push", (event) => {
  const data = event.data ? event.data.json() : {};
  const title = data.title || "Megatokyo";
  event.waitUntil(
    self.registration.showNotification(title, {
      body: data.url || "",
      icon: "/icon.svg",
      data: { url: data.url || "/" },
    })
  );
});

// Focuses an already-open client on the notification's URL rather than
// always opening a new tab/window.
self.addEventListener("notificationclick", (event) => {
  event.notification.close();
  const url = event.notification.data && event.notification.data.url;
  if (!url) return;
  event.waitUntil(
    self.clients
      .matchAll({ type: "window", includeUncontrolled: true })
      .then((clients) => {
        for (const client of clients) {
          if (client.url === url && "focus" in client) {
            return client.focus();
          }
        }
        if (self.clients.openWindow) {
          return self.clients.openWindow(url);
        }
      })
  );
});
