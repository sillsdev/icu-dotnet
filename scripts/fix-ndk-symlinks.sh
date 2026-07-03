#!/usr/bin/env bash
# Repair NDK symlinks broken by zip extractors that don't preserve links (e.g. Python zipfile).
set -euo pipefail

NDK_BIN="${1:?Usage: fix-ndk-symlinks.sh <ndk-toolchain-bin-dir>}"

cd "$NDK_BIN"
for f in *; do
    [[ -f "$f" ]] || continue
    size=$(stat -c%s "$f" 2>/dev/null || echo 999)
    [[ "$size" -lt 64 ]] || continue
    target=$(tr -d '\r\n' < "$f")
    [[ -n "$target" ]] || continue
    if [[ -e "$NDK_BIN/$target" ]]; then
        rm -f "$f"
        ln -s "$target" "$f"
    fi
done

"$NDK_BIN/x86_64-linux-android21-clang" --version | head -1
