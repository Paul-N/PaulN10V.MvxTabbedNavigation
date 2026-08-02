#!/usr/bin/env bash
set -euo pipefail

PROJECT_PATH="${PROJECT_PATH:-Demos/EvilGenius.MvxTabbedNavigation.Demo/EvilGenius.MvxTabbedNavigation.Demo.csproj}"
SOURCES_DIR="${SOURCES_DIR:-/sources}"
FRAMEWORK="${FRAMEWORK:-net9.0-android}"
CONFIGURATION="${CONFIGURATION:-Debug}"
OUTPUT_DIR="${OUTPUT_DIR:-/output}"

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

echo "Using project: $proj"
echo "Framework: $FRAMEWORK, configuration: $CONFIGURATION"

proj_name="$(basename "$proj")"

# Clean output
mkdir -p "$OUTPUT_DIR"
rm -rf "$OUTPUT_DIR"/*

# Phase 1: Publish (matches VS F5 workflow - creates complete, deployable APK)
echo "Publishing with dotnet publish (VS F5 style)..."
if ! dotnet publish "$proj_name" \
  -f:"$FRAMEWORK" \
  -c:"$CONFIGURATION" \
  -p:AndroidLinkMode=None \
  -p:AndroidSdkDirectory="$ANDROID_SDK_ROOT" \
  -p:AndroidStrip=false \
  -p:DebugSymbols=true \
  -p:DebugType=portable \
  -p:AndroidSigningKeyStore=true \
  -p:AndroidSigningKeyAlias=androiddebugkey \
  -p:AndroidSigningKeyPass=android \
  -p:AndroidSigningStorePass=android \
  -p:TrimMode=link \
  -p:PublishTrimmed=false \
  -p:AndroidPackageFormat=apk \
  -p:AndroidUseApkSigner=true \
  -p:EmbedAssembliesIntoApk=true \
  -p:AndroidDexTool=d8 \
  -o "$OUTPUT_DIR/publish" \
  --verbosity n; then
  echo "dotnet publish failed" >&2
  exit 1
fi

# Phase 2: Extract artifacts from publish output
# Create subdirectories in output
mkdir -p "$OUTPUT_DIR/apk"
mkdir -p "$OUTPUT_DIR/assemblies"
mkdir -p "$OUTPUT_DIR/symbols"
mkdir -p "$OUTPUT_DIR/deploy-script"

# 1. Copy signed APK from publish output
if [ -f "$OUTPUT_DIR/publish"/*-Signed.apk ]; then
  cp "$OUTPUT_DIR/publish"/*-Signed.apk "$OUTPUT_DIR/apk/" 2>/dev/null || true
  echo "✓ Signed APK copied from publish output"
fi

# 2. Extract assemblies from APK (for Fast Deployment backup)
# Note: With EmbedAssembliesIntoApk=true, assemblies are IN the APK
# This is just for reference/debugging
if [ -d "$OUTPUT_DIR/publish/assemblies" ]; then
  find "$OUTPUT_DIR/publish/assemblies" -name "*.dll" -exec cp {} "$OUTPUT_DIR/assemblies/" \;
  echo "✓ Assemblies extracted from publish output"
fi

# 3. Extract debug symbols (.pdb files)
CONF_UPPER=$(echo "$CONFIGURATION" | tr '[:lower:]' '[:upper:]')
BIN_DIR="$(dirname "$proj")/bin/$CONF_UPPER/${FRAMEWORK}/android-arm64"
if [ -d "$BIN_DIR" ]; then
  find "$BIN_DIR" -maxdepth 1 -name "*.pdb" -exec cp {} "$OUTPUT_DIR/symbols/" \; 2>/dev/null || true
fi
echo "✓ Debug symbols extracted"

# 4. Extract package name from manifest or csproj
APP_PACKAGE=$(grep -oP 'package android:name="\K[^"]+' "$SOURCES_DIR/Demos/EvilGenius.MvxTabbedNavigation.Demo/AndroidManifest.xml" 2>/dev/null || \
              grep -oP '<ApplicationId>\K[^<]+' "$SOURCES_DIR/Demos/EvilGenius.MvxTabbedNavigation.Demo/EvilGenius.MvxTabbedNavigation.Demo.csproj" 2>/dev/null || \
              echo "com.evilgenius.tabbednavigationdemo")

# 5. Generate macOS deployment script
cat > "$OUTPUT_DIR/deploy-script/deploy.sh" << 'EOFSCRIPT'
#!/bin/bash
# macOS deployment script for VS F5-style debugging workflow

set -euo pipefail

OUTPUT_DIR="${1:-.}"
PACKAGE="$2"

if [ -z "$PACKAGE" ]; then
  echo "Usage: ./deploy.sh <output_dir> <package_name>"
  echo "Example: ./deploy.sh /path/to/output com.evilgenius.tabbednavigationdemo"
  exit 1
fi

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "VS F5 Debugging Deployment (Phase 2 - macOS)"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Check adb is available
if ! command -v adb &> /dev/null; then
  echo "❌ adb not found. Install Android SDK or add to PATH"
  exit 1
fi

# Check device/emulator is connected
if [ -z "$(adb devices | grep -E 'emulator|device' | grep -v devices)" ]; then
  echo "❌ No Android device/emulator connected"
  exit 1
fi

echo "✓ Device connected"

# 1. Uninstall previous version (optional, helps ensure clean state)
# adb uninstall "$PACKAGE" 2>/dev/null || true

# 2. Install APK
echo ""
echo "Installing APK..."
adb install-multiple "$OUTPUT_DIR/apk"/*.apk
echo "✓ APK installed"

# 3. Create Fast Deployment directory
echo ""
echo "Setting up Fast Deployment directories..."
adb shell mkdir -p "/data/user/0/$PACKAGE/files/.__override__/assemblies"
echo "✓ Override directory created"

# 4. Push individual assemblies (this is what makes F5 fast - only changed DLLs sync)
echo ""
echo "Pushing assemblies for Fast Deployment..."
for dll in "$OUTPUT_DIR/assemblies"/*.dll; do
  if [ -f "$dll" ]; then
    adb push "$dll" "/data/user/0/$PACKAGE/files/.__override__/assemblies/" > /dev/null
  fi
done
echo "✓ Assemblies deployed"

# 5. Summary
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "Deployment complete!"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "Next steps:"
echo "  1. Open your project in Visual Studio / Rider"
echo "  2. Select Debug configuration"
echo "  3. Select Android Emulator as target"
echo "  4. Press F5 or Run → Attach to Process"
echo ""
echo "Debug symbols available at:"
echo "  $OUTPUT_DIR/symbols/"
echo ""
EOFSCRIPT

chmod +x "$OUTPUT_DIR/deploy-script/deploy.sh"
echo "✓ Deploy script created"

# 6. Final summary
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✓ Build Phase Complete (Docker)"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "Output structure:"
echo "  apk/         - APK file (install on device)"
echo "  assemblies/  - Individual DLLs (Fast Deployment)"
echo "  symbols/     - PDB files (source-level debugging)"
echo "  deploy-script/ - Automated deployment"
echo ""
echo "On macOS, run:"
echo "  \$ ./deploy-script/deploy.sh $OUTPUT_DIR $APP_PACKAGE"
echo ""
echo "Files:"
ls -lh "$OUTPUT_DIR"/apk/ "$OUTPUT_DIR"/assemblies/ "$OUTPUT_DIR"/symbols/ 2>/dev/null | grep -v total || true