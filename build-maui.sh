#!/usr/bin/env bash
set -euo pipefail
shopt -s nullglob

PROJECT_PATH="${PROJECT_PATH:-Demos/EvilGenius.MvxTabbedNavigation.Demo/EvilGenius.MvxTabbedNavigation.Demo.csproj}"
SOURCES_DIR="${SOURCES_DIR:-/sources}"
FRAMEWORK="${FRAMEWORK:-net6.0-android}"
CONFIGURATION="${CONFIGURATION:-Release}"
OUTPUT_DIR="${OUTPUT_DIR:-/output}"
# Wipe obj/ and bin/ before building. The sources tree is bind-mounted and shared
# between the host IDE and several SDK images, so stale intermediates from a
# different SDK can produce an APK that segfaults during runtime init.
CLEAN="${CLEAN:-1}"
# Optional ABI filter, e.g. "android-x64" (Intel emulator) or "android-arm64"
# (Apple Silicon emulator). Empty = build every ABI the SDK defaults to.
RUNTIME_IDS="${RUNTIME_IDS:-}"

# Resolve project path (absolute or relative to SOURCES_DIR)
if [[ "$PROJECT_PATH" = /* ]]; then
  proj="$PROJECT_PATH"
else
  proj="$SOURCES_DIR/$PROJECT_PATH"
fi

if [ ! -f "$proj" ]; then
  echo "Project not found: $proj" >&2
  exit 1
fi

cd "$(dirname "$proj")"

echo "Building runnable Android APK"
echo "Project: $proj"
echo "Framework: $FRAMEWORK"
echo "Configuration: $CONFIGURATION"

proj_name="$(basename "$proj")"

# Drop stale intermediates
if [ "$CLEAN" = "1" ]; then
  echo ""
  echo "Cleaning intermediates under $SOURCES_DIR ..."
  find "$SOURCES_DIR" -type d \( -name obj -o -name bin \) -prune -print -exec rm -rf {} +
fi

# Clean output
mkdir -p "$OUTPUT_DIR"
rm -rf "${OUTPUT_DIR:?}"/*

ALIAS=$(grep '^ALIAS=' /keystore/.env | cut -d'=' -f2)
PASS=$(grep '^PASS=' /keystore/.env | cut -d'=' -f2)

publish_args=(
  -f:"$FRAMEWORK"
  -c:RELEASE
  -p:AndroidKeyStore=true
  -p:AndroidSigningKeyStore=/keystore/stub.keystore
  -p:AndroidSigningKeyAlias="$ALIAS"
  -p:AndroidSigningKeyPass="$PASS" 
  -p:AndroidSigningStorePass="$PASS"
  #Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
  --verbosity n
)
  
if [ -n "$RUNTIME_IDS" ]; then
  publish_args+=( -p:RuntimeIdentifiers="$RUNTIME_IDS" )
fi

# RunAOTCompilation is only needed for .NET 6
if [[ "$FRAMEWORK" == "net6.0-android" ]]; then
  publish_args+=( -p:RunAOTCompilation=false )
fi

echo "Calling dotnet clean..."
if ! dotnet clean "$proj_name"; then
  echo "dotnet clean failed" >&2
  exit 1
fi

# Build and publish
echo ""
echo "Publishing APK..."
if ! dotnet publish "$proj_name" "${publish_args[@]}" -o "$OUTPUT_DIR"/; then
  echo "dotnet publish failed" >&2
  exit 1
fi

echo "Build complete"

