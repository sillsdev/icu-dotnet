#!/usr/bin/env bash
# Copyright (c) 2026 SIL Global
# Cross-compile ICU4C for Android (icu-dotnet device tests).
# Adapted from https://github.com/patrickgold/icu4c-android (Apache 2.0).
#
# Prerequisites: bash, curl, tar, make, Android NDK (ANDROID_NDK_HOME or ANDROID_HOME/ndk/*)
# On Windows: run via Git Bash or WSL (scripts/build-icu-android.ps1).
#
# Output: output/android-icu/{x86_64,arm64-v8a}/libicu*.so*

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
OUTPUT_DIR="$REPO_ROOT/output/android-icu"
ICU_VERSION="${ICU_VERSION:-72.1}"
ICU_VERSION_UNDERSCORE="${ICU_VERSION//./_}"
ARCHS="${ARCHS:-x86_64}"
API_LEVEL="${API_LEVEL:-21}"
ICU_SRC_DIR="${ICU_SRC_DIR:-$OUTPUT_DIR/src/icu}"
HOST_BUILD_DIR="$OUTPUT_DIR/build/host"
DATA_PACKAGING="${DATA_PACKAGING:-shared}"

usage() {
    cat <<EOF
Usage: $0 [options]

Options:
  --arch=LIST       Comma-separated ABIs: x86_64, arm64-v8a (default: x86_64)
  --api=LEVEL       Minimum Android API (default: 21)
  --icu-version=VER ICU version (default: 72.1)
  --clean           Remove build trees under output/android-icu/build
  --clean-arch=LIST Remove only output/android-icu/build/android/{abi} for listed arch(es)
  --help            Show this help

Environment:
  ANDROID_NDK_HOME  Path to Android NDK (or latest under ANDROID_HOME/ndk)
EOF
}

log() { echo "[build-icu-android] $*"; }
die() { echo "[build-icu-android] ERROR: $*" >&2; exit 1; }

detect_ndk_host_tag() {
    case "$(uname -s)" in
        Linux) echo "linux-$(uname -m)" ;;
        Darwin) echo "darwin-$(uname -m)" ;;
        MINGW*|MSYS*|CYGWIN*) echo "windows-x86_64" ;;
        *) die "Unsupported host OS for NDK toolchain: $(uname -s)" ;;
    esac
}

resolve_ndk_toolchain_tag() {
    local ndk="$1"
    local preferred host_os
    preferred="$(detect_ndk_host_tag)"
    if [[ -d "$ndk/toolchains/llvm/prebuilt/$preferred" ]]; then
        echo "$preferred"
        return
    fi
    host_os="$(uname -s)"
    if [[ "$host_os" == Linux && -d "$ndk/toolchains/llvm/prebuilt/windows-x86_64" ]]; then
        die "NDK at $ndk only has the Windows toolchain (windows-x86_64). WSL/Linux builds need the Linux NDK zip from https://developer.android.com/ndk/downloads — set ANDROID_NDK_HOME to that install."
    fi
    if [[ "$host_os" =~ ^(MINGW|MSYS|CYGWIN) && -d "$ndk/toolchains/llvm/prebuilt/linux-x86_64" ]]; then
        die "NDK at $ndk only has the Linux toolchain. Git Bash/Windows builds need the Windows NDK from Android Studio sdkmanager."
    fi
    die "NDK toolchain not found under $ndk/toolchains/llvm/prebuilt/ (expected $preferred)"
}

resolve_ndk() {
    if [[ -n "${ANDROID_NDK_HOME:-}" && -d "$ANDROID_NDK_HOME" ]]; then
        echo "$ANDROID_NDK_HOME"
        return
    fi
    if [[ -n "${ANDROID_HOME:-}" && -d "$ANDROID_HOME/ndk" ]]; then
        local latest
        latest="$(ls -d "$ANDROID_HOME/ndk/"* 2>/dev/null | sort -V | tail -1)"
        [[ -n "$latest" ]] && echo "$latest" && return
    fi
    if command -v ndk-build >/dev/null 2>&1; then
        dirname "$(command -v ndk-build)"
        return
    fi
    die "Android NDK not found. Set ANDROID_NDK_HOME or ANDROID_HOME."
}

arch_to_target() {
    case "$1" in
        x86_64) echo "x86_64-linux-android" ;;
        arm64-v8a|arm64) echo "aarch64-linux-android" ;;
        x86) echo "i686-linux-android" ;;
        armeabi-v7a|arm) echo "armv7a-linux-androideabi" ;;
        *) die "Unsupported arch: $1" ;;
    esac
}

arch_to_abi_folder() {
    case "$1" in
        arm64) echo "arm64-v8a" ;;
        arm) echo "armeabi-v7a" ;;
        *) echo "$1" ;;
    esac
}

copy_ndk_cpp_shared() {
    local ndk="$1"
    local host_tag="$2"
    local target="$3"
    local install_dir="$4"
    local cxx_lib="$ndk/toolchains/llvm/prebuilt/$host_tag/sysroot/usr/lib/$target/libc++_shared.so"
    [[ -f "$cxx_lib" ]] || die "NDK libc++ not found: $cxx_lib"
    cp -f "$cxx_lib" "$install_dir/"
    log "Copied libc++_shared.so to $install_dir"
}

install_android_apk_libs() {
    local install_dir="$1"
    local icu_major="${ICU_VERSION%%.*}"
    for lib in icuuc icui18n icudata; do
        local src=""
        for candidate in \
            "$install_dir/lib${lib}.so.${ICU_VERSION}" \
            "$install_dir/lib${lib}.so.${icu_major}.1" \
            "$install_dir/lib${lib}.so.${icu_major}"; do
            if [[ -f "$candidate" ]]; then
                src="$candidate"
                break
            fi
        done
        [[ -n "$src" ]] || die "Missing lib${lib} for Android APK packaging in $install_dir"
        cp -f "$src" "$install_dir/lib${lib}.so"
        cp -f "$src" "$install_dir/lib${lib}.so.${icu_major}"
    done
    log "Installed Android APK libs (libicu{uc,i18n,data}.so and .so.${icu_major}) to $install_dir"
}

install_icu_data_file() {
    local icu_major="${ICU_VERSION%%.*}"
    local dat_name="icudt${icu_major}l.dat"
    local dat_src=""
    for candidate in \
        "$HOST_BUILD_DIR/data/out/tmp/$dat_name" \
        "$ICU_SRC_DIR/source/data/in/$dat_name"; do
        if [[ -f "$candidate" ]]; then
            dat_src="$candidate"
            break
        fi
    done
    [[ -n "$dat_src" ]] || die "Missing ICU data file $dat_name (build host ICU first)"
    cp -f "$dat_src" "$OUTPUT_DIR/$dat_name"
    log "Installed $dat_name to $OUTPUT_DIR"
}

download_icu_source() {
    if [[ -f "$ICU_SRC_DIR/source/configure" ]]; then
        log "ICU source already present at $ICU_SRC_DIR"
        return
    fi
    mkdir -p "$(dirname "$ICU_SRC_DIR")"
    local tarball="icu4c-${ICU_VERSION_UNDERSCORE}-src.tgz"
    local url="https://github.com/unicode-org/icu/releases/download/release-${ICU_VERSION//./-}/$tarball"
    local cache="$OUTPUT_DIR/src/$tarball"
    log "Downloading ICU $ICU_VERSION from $url"
    mkdir -p "$OUTPUT_DIR/src"
    if [[ ! -f "$cache" ]]; then
        curl -fsSL -o "$cache" "$url"
    fi
    tar -xzf "$cache" -C "$OUTPUT_DIR/src"
    local extracted="$OUTPUT_DIR/src/icu"
    if [[ "$extracted" != "$ICU_SRC_DIR" ]]; then
        rm -rf "$ICU_SRC_DIR"
        mv "$extracted" "$ICU_SRC_DIR"
    fi
}

build_host_icu() {
    if [[ -f "$HOST_BUILD_DIR/icu_build/lib/libicuuc.so" || -f "$HOST_BUILD_DIR/icu_build/lib/libicuuc.a" ]]; then
        log "Host ICU build already present"
        return
    fi
    log "Building host ICU (required for cross-build tools)..."
    mkdir -p "$HOST_BUILD_DIR"
    pushd "$HOST_BUILD_DIR" >/dev/null
    "$ICU_SRC_DIR/source/runConfigureICU" Linux \
        --enable-static=no \
        --enable-shared=yes \
        --enable-tests=no \
        --enable-samples=no \
        --enable-extras=no \
        --enable-draft=yes \
        --datadir="$HOST_BUILD_DIR/data" \
        --prefix="$HOST_BUILD_DIR/icu_build"
    make -j"$(nproc 2>/dev/null || sysctl -n hw.ncpu 2>/dev/null || echo 4)"
  popd >/dev/null
}

build_android_arch() {
    local arch="$1"
    local abi_folder
    abi_folder="$(arch_to_abi_folder "$arch")"
    local target
    target="$(arch_to_target "$arch")"
    local ndk="$2"
    local host_tag="$3"
    local build_dir="$OUTPUT_DIR/build/android/$abi_folder"
    local install_dir="$OUTPUT_DIR/$abi_folder"

    log "Cross-compiling ICU for $abi_folder ($target)..."
    mkdir -p "$build_dir" "$install_dir"
    pushd "$build_dir" >/dev/null

    local toolchain="$ndk/toolchains/llvm/prebuilt/$host_tag"
    [[ -d "$toolchain" ]] || die "NDK toolchain not found: $toolchain"

    export PATH="$toolchain/bin:$PATH"
    export CC="$toolchain/bin/${target}${API_LEVEL}-clang"
    export CXX="$toolchain/bin/${target}${API_LEVEL}-clang++"
    export AR="$toolchain/bin/llvm-ar"
    export RANLIB="$toolchain/bin/llvm-ranlib"
    export LDFLAGS="-Wl,--gc-sections -Wl,-z,max-page-size=16384"

    local configure_data_packaging="$DATA_PACKAGING"
    if [[ "$configure_data_packaging" == "shared" ]]; then
        configure_data_packaging="dll"
    fi

    "$ICU_SRC_DIR/source/configure" \
        --with-cross-build="$HOST_BUILD_DIR" \
        --host="$target" \
        --prefix="$build_dir/icu_build" \
        --enable-static=no \
        --enable-shared=yes \
        --enable-tests=no \
        --enable-samples=no \
        --enable-extras=no \
        --enable-draft=yes \
        --with-data-packaging="$configure_data_packaging"

    make -j"$(nproc 2>/dev/null || sysctl -n hw.ncpu 2>/dev/null || echo 4)"

    log "Installing libraries to $install_dir"
    cp -f "$build_dir/lib"/libicu*.so* "$install_dir/" 2>/dev/null || cp -f "$build_dir/icu_build/lib"/libicu*.so* "$install_dir/"
    if [[ "$configure_data_packaging" == "dll" && -d "$build_dir/stubdata" ]]; then
        cp -f "$build_dir/stubdata"/libicudata*.so* "$install_dir/" 2>/dev/null || true
    fi
    copy_ndk_cpp_shared "$ndk" "$host_tag" "$target" "$install_dir"
    install_android_apk_libs "$install_dir"

    popd >/dev/null
    log "Built $(ls -1 "$install_dir"/libicu*.so* 2>/dev/null | wc -l | tr -d ' ') libraries for $abi_folder"
}

CLEAN=no
CLEAN_ARCHS=""
for arg in "$@"; do
    case "$arg" in
        --help|-h) usage; exit 0 ;;
        --clean) CLEAN=yes ;;
        --clean-arch=*) CLEAN_ARCHS="${arg#*=}" ;;
        --arch=*) ARCHS="${arg#*=}" ;;
        --api=*) API_LEVEL="${arg#*=}" ;;
        --icu-version=*) ICU_VERSION="${arg#*=}"; ICU_VERSION_UNDERSCORE="${ICU_VERSION//./_}" ;;
        *) die "Unknown option: $arg" ;;
    esac
done

if [[ "$CLEAN" == "yes" ]]; then
    rm -rf "$OUTPUT_DIR/build"
    log "Cleaned $OUTPUT_DIR/build"
    exit 0
fi

if [[ -n "$CLEAN_ARCHS" ]]; then
    IFS=',' read -ra CLEAN_ARCH_LIST <<< "$CLEAN_ARCHS"
    for arch in "${CLEAN_ARCH_LIST[@]}"; do
        arch="$(echo "$arch" | xargs)"
        [[ -n "$arch" ]] || continue
        abi_folder="$(arch_to_abi_folder "$arch")"
        rm -rf "$OUTPUT_DIR/build/android/$abi_folder"
        log "Cleaned $OUTPUT_DIR/build/android/$abi_folder"
    done
fi

for cmd in curl tar make; do
    command -v "$cmd" >/dev/null || die "Required command not found: $cmd"
done

NDK="$(resolve_ndk)"
HOST_TAG="$(resolve_ndk_toolchain_tag "$NDK")"
log "NDK: $NDK"
log "Host toolchain tag: $HOST_TAG"
log "ICU version: $ICU_VERSION"
log "Architectures: $ARCHS"

download_icu_source
build_host_icu
install_icu_data_file

IFS=',' read -ra ARCH_LIST <<< "$ARCHS"
for arch in "${ARCH_LIST[@]}"; do
    arch="$(echo "$arch" | xargs)"
    [[ -n "$arch" ]] || continue
    build_android_arch "$arch" "$NDK" "$HOST_TAG"
done

log "Done. Libraries are in $OUTPUT_DIR/{abi}/"
log "Rebuild the Android test app, then run: .\\scripts\\run-android-tests.ps1"
