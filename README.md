Usage: build .apk with debug symbols using the provided Docker builder

1) Build the image (from repo root):

```bash
   docker build -f DockerfileMaui6 -t maui6 .
   docker build -f DockerfileMaui8 -t maui8 .
   docker build -f DockerfileMaui10 -t maui10 .
```

2) Run the builder to build a specific git ref (output is written to a host folder):

```bash   
docker run --rm \
     -e CONFIGURATION="Debug" \
     -v "$(pwd)/maui-output:/output" \
   maui6
```
Notes:
- The image installs OpenJDK 17 and Android command-line tools and the maui-android workload.
- The script publishes a Debug APK with managed PDBs (portable) and disables linking/stripping so debug metadata remains.
- Depending on your project and SDK versions you may need to adjust FRAMEWORK and Android platform/build-tools versions in the Dockerfile.
- This builder is intended for CI and local reproducible builds. For device debugging from your host, install matching sources and ensure symbols (.pdb) from the output are accessible to your IDE.

If you want, next step can be: (A) tune the Dockerfile for a specific .NET/MAUI version, (B) add Android SDK licenses acceptance via env to automate non-interactive acceptance, or (C) produce an AAB instead of APK.
