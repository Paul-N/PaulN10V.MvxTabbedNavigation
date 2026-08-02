FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install OS packages
ENV DEBIAN_FRONTEND=noninteractive
RUN apt-get update && apt-get install -y --no-install-recommends \
    unzip wget curl git openjdk-17-jdk ca-certificates && \
    rm -rf /var/lib/apt/lists/*

ENV ANDROID_SDK_ROOT=/opt/android-sdk
ENV JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64
ENV PATH=$PATH:$ANDROID_SDK_ROOT/cmdline-tools/latest/bin:$ANDROID_SDK_ROOT/platform-tools

# Install Android command-line tools
RUN mkdir -p $ANDROID_SDK_ROOT && \
    cd /tmp && \
    curl -sS https://dl.google.com/android/repository/repository2-1.xml | \
      grep -oE 'commandlinetools-linux-[0-9_]+_latest.zip' | sort -V | tail -n1 | \
      xargs -I{} wget -q https://dl.google.com/android/repository/{} -O cmdline.zip && \
    unzip cmdline.zip -d $ANDROID_SDK_ROOT/cmdline-tools && \
    mv $ANDROID_SDK_ROOT/cmdline-tools/cmdline-tools $ANDROID_SDK_ROOT/cmdline-tools/latest && \
    rm cmdline.zip

# Install essential Android packages (platform-tools, build-tools, platforms)
RUN yes | $ANDROID_SDK_ROOT/cmdline-tools/latest/bin/sdkmanager --sdk_root=$ANDROID_SDK_ROOT "platform-tools" "platforms;android-33" "build-tools;33.0.2" || true

# Install MAUI Android workload (use SDK's default dotnet)
RUN dotnet workload install maui-android --skip-manifest-update || true

# Copy build script and make it entrypoint
WORKDIR /workspace
COPY build-maui.sh /usr/local/bin/build-maui
RUN chmod +x /usr/local/bin/build-maui

ENTRYPOINT ["/usr/local/bin/build-maui"]
