#!/usr/bin/env bash
set -euo pipefail
shopt -s nullglob

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DOTNET_VERSION="${1:-6}"

# Validate version
if [[ ! "$DOTNET_VERSION" =~ ^(6|8|10)$ ]]; then
  echo "Usage: $0 [6|8|10]" >&2
  exit 1
fi

IMAGE="maui${DOTNET_VERSION}"
FRAMEWORK="net${DOTNET_VERSION}.0-android"

docker run --rm \
  --platform linux/amd64 \
  -v "${SCRIPT_DIR}/sources:/sources" \
  -v "${SCRIPT_DIR}/maui-output:/output" \
  -v "${SCRIPT_DIR}/build-maui.sh:/work/build-maui.sh" \
  -v "${SCRIPT_DIR}/keystore:/keystore" \
  -w /work \
  -e FRAMEWORK="${FRAMEWORK}" \
  -e CONFIGURATION="RELEASE" \
  -it "${IMAGE}" \
  bash -c "/work/build-maui.sh"
