#!/bin/sh
# Run this locally, with network access, before building a Launchpad source
# upload (debuild -S -sa). NEVER run as part of debian/rules — Launchpad's
# build farm has no general internet access, so everything this script
# fetches must already be sitting in the working directory by the time
# dpkg-source tars it up.
#
# Unlike linux-hello/kio-protondrive, this workspace is pure Cargo (no
# CMake/Corrosion, no build.rs downloading model files), but the pwa
# package's Leptos/WASM bundle still needs two offline-unfriendly things
# nothing else here does: the trunk/wasm-bindgen-cli tools trunk build
# needs, and — the harder one — a full official Rust toolchain, because
# Ubuntu's own apt rustc cannot build std for wasm32 at all (see the
# relevant sections below for both, and why).
#
# IMPORTANT: vendor with the SAME cargo version the target Ubuntu series
# ships (check with `rmadison -u ubuntu cargo`), not whatever's locally
# "stable" — a newer cargo vendoring the tree can silently omit
# Cargo.toml.orig companion files an OLDER cargo needs at build time to
# verify a vendored crate's checksum against Cargo.lock (hit for real on
# linux-hello, see its own copy of this script). Set RUST_TOOLCHAIN to the
# version to vendor with; defaults to "stable", which is very likely wrong
# for an older LTS target — pass the real one explicitly:
#   RUST_TOOLCHAIN=1.93.1 ./debian/scripts/prepare-offline-build.sh
#
# See docs/LAUNCHPAD.md for how this fits into the release process.

set -eu

RUST_TOOLCHAIN="${RUST_TOOLCHAIN:-stable}"

cd "$(dirname "$0")/../.."

echo "==> Removing target/ (must not exist when dpkg-source tars up the tree)"
rm -rf target

echo "==> Vendoring Cargo dependencies into vendor/ (toolchain: $RUST_TOOLCHAIN)"
if [ "$RUST_TOOLCHAIN" = "stable" ]; then
  echo "    WARNING: no RUST_TOOLCHAIN given, using 'stable' — check the target" >&2
  echo "    series' cargo version first (rmadison -u ubuntu cargo) and pass" >&2
  echo "    RUST_TOOLCHAIN=<version> explicitly if it differs." >&2
fi
rustup toolchain install "$RUST_TOOLCHAIN" > /dev/null 2>&1 || true
rm -rf vendor
# Not a bare `rm -rf .cargo`: .cargo/audit.toml is a tracked, committed
# file (cargo-audit's config — see its own doc comment), not part of the
# generated/gitignored vendoring output; wiping the whole directory here
# deleted it for real on a previous run.
rm -f .cargo/config.toml
mkdir -p .cargo
cargo "+$RUST_TOOLCHAIN" vendor vendor > /tmp/cargo-vendor-config.toml.tmp
cat /tmp/cargo-vendor-config.toml.tmp > .cargo/config.toml
rm -f /tmp/cargo-vendor-config.toml.tmp
echo "    $(du -sh vendor | cut -f1) in vendor/, $(find vendor -name '*.orig' | wc -l) .orig files"

echo "==> Disabling cargo's per-file checksum verification for vendored crates"
# dpkg-source's native-tarball builder has a hardcoded exclude list (VCS
# control files, backup/swap files — .git, .gitignore, .svn, CVS, *.orig,
# DEADJOE, ...) that cannot be turned off via debian/source/options. Some
# vendored crate, somewhere, will always have a test fixture or metadata
# file that happens to match one of those generic names — dpkg-source
# silently drops it from the tarball, and cargo's offline build then fails
# verifying that crate's per-file checksums against .cargo-checksum.json.
# Documented Debian Rust-packaging fix: blank out each vendored crate's
# "files" checksum map so cargo only trusts the vendor directory as-is (the
# "package" checksum, verified against Cargo.lock, is untouched). Same fix
# as linux-hello's and kio-protondrive's own copy of this script.
find vendor -maxdepth 2 -name ".cargo-checksum.json" |
  while IFS= read -r f; do
    jq '.files = {}' "$f" > "$f.tmp" && mv "$f.tmp" "$f"
  done

echo "==> Pre-fetching trunk + wasm-bindgen-cli into a throwaway cargo registry cache"
# pwa/ isn't a plain cargo binary: it's a Leptos/WASM bundle trunk
# assembles from a wasm32-unknown-unknown build + wasm-bindgen's JS glue.
# Neither trunk nor wasm-bindgen-cli are apt packages, so debian/rules
# needs to build them from source too — offline, like everything else
# here.
#
# `cargo vendor` (used above for the main workspace) was tried first for
# these two and abandoned: resolving trunk and wasm-bindgen-cli into a
# single fresh Cargo.lock forces Cargo to unify their otherwise-unrelated
# transitive `wasm-bindgen` versions, and a "latest compatible" resolve
# done today can also land on a genuinely broken pairing (hit for real:
# lightningcss 1.0.0-alpha.65 against a too-new cssparser). trunk's own
# published Cargo.lock is already a known-good pairing — the simplest way
# to reuse it verbatim is to let a real, network-backed `cargo install
# --locked` populate an ordinary cargo registry cache, then replay the
# exact same command with `--offline` at build time (see override_dh_auto_build
# in debian/rules). This also produces a noticeably smaller source tree
# than vendoring (registry cache dedupes; `cargo vendor` doesn't).
# Populated OUTSIDE the repo, then copied in — not built in place. Cargo's
# config discovery turns out to also walk up from CARGO_HOME's own path
# (confirmed empirically, undocumented), so a CARGO_HOME living anywhere
# under this repo picks up the .cargo/config.toml just written above (the
# *main* vendor/ source-replacement) even when invoked from a totally
# unrelated CWD — same mechanism, wrong vendor tree, "could not find trunk
# in registry crates-io".
TMP_CARGO_HOME="$(mktemp -d)"
trap 'rm -rf "$TMP_CARGO_HOME"' EXIT

# Exact match required: trunk invokes whatever `wasm-bindgen` binary it
# finds, and that binary's version must equal the `wasm-bindgen` *library*
# version the pwa crate was compiled against (0.2.127 as of writing), or
# the generated JS glue is rejected at runtime.
WASM_BINDGEN_VERSION=$(grep -A1 '^name = "wasm-bindgen"$' Cargo.lock | grep '^version' | head -1 | sed -E 's/.*"(.+)".*/\1/')
echo "    pinning wasm-bindgen-cli to $WASM_BINDGEN_VERSION (this run's resolved wasm-bindgen)"

# --root cleaned before EACH install: cargo silently skips reinstalling
# (and skips populating the cache for) a package it thinks is already
# installed at that root, going only off leftover .crates.toml tracking
# state — bit us for real, produced a cache missing most of trunk's deps.
rm -rf /tmp/megatokyo-build-tools-throwaway
CARGO_HOME="$TMP_CARGO_HOME" cargo install --locked --root /tmp/megatokyo-build-tools-throwaway trunk
rm -rf /tmp/megatokyo-build-tools-throwaway
CARGO_HOME="$TMP_CARGO_HOME" cargo install --locked --root /tmp/megatokyo-build-tools-throwaway wasm-bindgen-cli --version "$WASM_BINDGEN_VERSION"
rm -rf /tmp/megatokyo-build-tools-throwaway

rm -rf debian/build-tools/cargo-home
mkdir -p debian/build-tools
cp -r "$TMP_CARGO_HOME" debian/build-tools/cargo-home
rm -rf "$TMP_CARGO_HOME"
trap - EXIT
echo "    $(du -sh debian/build-tools/cargo-home | cut -f1) in debian/build-tools/cargo-home"

echo "==> Vendoring a full official rustc+cargo+std toolchain for $RUST_TOOLCHAIN (pwa's wasm32 build)"
# Not just "the wasm32-unknown-unknown rust-std component" — a full,
# self-contained rustc+cargo+std, host AND wasm32 target both. Two things
# were tried first and BOTH turned out to be genuinely impossible against
# Ubuntu's own apt-packaged rustc, not just inconvenient:
#
#  1. Copying only rustup's official prebuilt wasm32 rust-std next to
#     Ubuntu's rustc: rejected at compile time as "compiled by an
#     incompatible version of rustc" — Rust's crate metadata fingerprint
#     bakes in the exact build (not just the version string), and Ubuntu
#     rebuilds rustc itself rather than repackaging upstream's binary, so
#     nothing prebuilt upstream ever matches it, same version number or
#     not.
#  2. Compiling rust-std from source via apt's `rust-src` package with
#     `-Z build-std` (which WOULD get the fingerprint right, same
#     compiler compiling its own std): Ubuntu's `rust-src` patches out
#     `dlmalloc` from the std build entirely (debian/patches/prune/
#     d-0020-remove-windows-dependencies.patch upstream, despite the
#     name) — std's own wasm32 allocator (library/std/src/sys/alloc/
#     wasm.rs) needs that crate and simply won't compile without it.
#     Not a bug on our end to work around; Ubuntu's rustc packaging does
#     not support building std for wasm32 at all, prebuilt or from
#     source.
#
# So: vendor a complete, self-consistent, official rustc+cargo+std (never
# mixed with Ubuntu's own rebuild) and use it just for pwa/'s wasm32
# build — daemon/gui/core keep using the apt rustc exactly as before,
# untouched by any of this.
rustup toolchain install "$RUST_TOOLCHAIN" --profile minimal > /dev/null 2>&1 || true
rustup target add wasm32-unknown-unknown --toolchain "$RUST_TOOLCHAIN"
rm -rf debian/vendor-rust-toolchain
mkdir -p debian/vendor-rust-toolchain
TOOLCHAIN_DIR="$(rustc "+$RUST_TOOLCHAIN" --print sysroot)"
cp -r "$TOOLCHAIN_DIR/." debian/vendor-rust-toolchain/
echo "    $(du -sh debian/vendor-rust-toolchain | cut -f1) in debian/vendor-rust-toolchain"

echo "==> Working around dpkg-source's built-in *.so/*.a/*.o/*.la exclusion"
# Confirmed by direct experiment (a minimal from-scratch repro, isolated
# from this project entirely): "3.0 (native)" source packages silently
# drop any file whose name ends in exactly .so, .a, .o, or .la when
# `dpkg-source -b` tars up the tree — completely unconditional, nothing to
# do with .gitignore, and (unlike the *.orig handling above) NOT
# reachable through --tar-ignore or debian/source/include-binaries either
# (both checked directly: neither affects native-format builds). It's
# `dpkg-source --help`'s documented tar-ignore default list, apparently
# applied unconditionally for this format regardless of what -I supports.
# Real, load-bearing files hit this: librustc_driver*.so, libstd*.so,
# libLLVM*.so, the sanitizer runtime *.a files in the vendored toolchain;
# ring's pregenerated *.o asm objects and the windows-* crates' *.a import
# libraries in the main vendor/. Versioned names (libfoo.so.1.2) are NOT
# affected — confirmed in the same repro — so a suffix rename dodges it
# without touching the file content at all. debian/rules' offline branch
# reverses this before anything tries to use these directories.
for d in vendor debian/build-tools/cargo-home debian/vendor-rust-toolchain; do
  find "$d" -type f \( -name '*.so' -o -name '*.a' -o -name '*.o' -o -name '*.la' \) |
    while IFS= read -r f; do mv "$f" "$f.dpkg-source-workaround"; done
done

cat <<EOF

Ready. From this same working directory (with vendor/, .cargo/config.toml,
debian/build-tools/cargo-home/ and debian/vendor-rust-toolchain/ populated),
debian/rules will build fully offline. Proceed with the dch / debuild -S -sa
/ dput cycle from docs/LAUNCHPAD.md.

Nothing here is meant to be committed to git — all of the above is
regenerated per release right before packaging (see .gitignore).
EOF
