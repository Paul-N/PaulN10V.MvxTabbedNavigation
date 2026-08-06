#!/usr/bin/env bash
set -euo pipefail
shopt -s nullglob

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Defaults
DOTNET_VERSION="6"
SRC_DIR="${SCRIPT_DIR}/sources"
CSPROJ=""

# Parse arguments
while [[ $# -gt 0 ]]; do
  case $1 in
    --dotnet)
      DOTNET_VERSION="$2"
      shift 2
      ;;
    --src)
      SRC_DIR="$2"
      shift 2
      ;;
    --csproj)
      CSPROJ="$2"
      shift 2
      ;;
    *)
      echo "Unknown option: $1" >&2
      exit 1
      ;;
  esac
done

# Validate version
if [[ ! "$DOTNET_VERSION" =~ ^(6|8|10)$ ]]; then
  echo "Usage: $0 --dotnet [6|8|10] --src /path/to/sources --csproj /path/to/proj.csproj" >&2
  exit 1
fi

IMAGE="pauln10v/maui${DOTNET_VERSION}"
FRAMEWORK="net${DOTNET_VERSION}.0-android"
DOCKERFILE="DockerfileMaui${DOTNET_VERSION}"

# Build image if it doesn't exist
if ! docker image inspect "${IMAGE}" >/dev/null 2>&1; then
  echo "Image ${IMAGE} not found. Building..."
  docker build -f "${DOCKERFILE}" -t "${IMAGE}" --platform linux/amd64 "${SCRIPT_DIR}"
  echo ""
fi

if [ -z "$CSPROJ" ]; then
  echo "Error: --csproj is required" >&2
  exit 1
fi

docker run --rm \
  --platform linux/amd64 \
  -v "${SRC_DIR}:/sources" \
  -v "${SCRIPT_DIR}/output:/output" \
  -v "${SCRIPT_DIR}/build-in-docker.sh:/work/build-in-docker.sh" \
  -v "${SCRIPT_DIR}/keystore:/keystore" \
  -w /work \
  -e FRAMEWORK="${FRAMEWORK}" \
  -e PROJECT_PATH="${CSPROJ}" \
  -it "${IMAGE}" \
  bash -c "/work/build-in-docker.sh"
