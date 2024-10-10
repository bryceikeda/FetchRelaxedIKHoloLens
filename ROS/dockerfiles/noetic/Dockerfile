# Stage 1: Base dependencies
FROM ros:noetic-ros-core AS base
ARG DEBIAN_FRONTEND=noninteractive
ARG OVERLAY_WS=/opt/overlay_ws
ARG UNDERLAY_WS=/opt/underlay_ws
ARG CMAKE_VERSION=3.28.1

RUN apt-get update && apt-get install -y \
    git \
    build-essential && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Install CMake
RUN git clone https://gitlab.kitware.com/cmake/cmake.git && \
    cd cmake && git checkout tags/v${CMAKE_VERSION} && \
    ./bootstrap --parallel=8 && make -j8 && make install && \
    cd .. && rm -rf cmake

# Stage 2: CMake Builder
FROM base AS apt

# Combine all apt-get installations into a single layer
RUN apt-get update && apt-get install -y \
    libssl-dev \
    python3-vcstool \
    python3-rosdep \
    python3-rosinstall \
    python3-rosinstall-generator \
    python3-wstool \
    python3-catkin-tools \
    python3-pip \
    curl \
    wget \
    bash-completion \
    vim \
    zsh \
    terminator \
    software-properties-common && \
    rosdep init && \
    rosdep update && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Stage 3: Caching dependencies
FROM apt AS cacher
ARG UNDERLAY_WS
ENV UNDERLAY_WS $UNDERLAY_WS 

# Clone and setup workspace
WORKDIR $UNDERLAY_WS/src
COPY dockerfiles/noetic/assets/underlay.repos ../
RUN vcs import ./ < ../underlay.repos --recursive

# Copy manifests for caching
WORKDIR /opt
RUN find . -name "src" -type d \
      -mindepth 1 -maxdepth 2 -printf '%P\n' \
      | xargs -I % mkdir -p /tmp/opt/% && \
    find . -name "package.xml" \
      | xargs cp --parents -t /tmp/opt

# Stage 4: Builder
FROM apt AS builder

# Install Rust
ENV RUSTUP_HOME=/usr/local/rustup \
    CARGO_HOME=/usr/local/cargo \
    PATH="/usr/local/cargo/bin:$PATH"

RUN curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | \
    sh -s -- -y --no-modify-path --profile minimal --default-toolchain stable && \
    chmod -R a+w $RUSTUP_HOME $CARGO_HOME

# Setup and build underlay source
ARG UNDERLAY_WS
ENV UNDERLAY_WS $UNDERLAY_WS 
WORKDIR $UNDERLAY_WS

# Install underlay dependencies
COPY --from=cacher /tmp/$UNDERLAY_WS ./
RUN . /opt/ros/$ROS_DISTRO/setup.sh && \
    apt-get update && rosdep install -q -y \
      --from-paths src \
      --ignore-src \
    && rm -rf /var/lib/apt/lists/*

# Build underlay source
COPY --from=cacher $UNDERLAY_WS ./
RUN . /opt/ros/$ROS_DISTRO/setup.sh && \
    catkin config --extend /opt/ros/$ROS_DISTRO && \
    catkin build

# Build relaxed_ik_core
WORKDIR $UNDERLAY_WS/src/relaxed_ik_ros1/relaxed_ik_core
RUN cargo build
RUN mv configs/example_settings/fetch.yaml configs/settings.yaml
WORKDIR $UNDERLAY_WS/src/ROS-TCP-Endpoint/src/ros_tcp_endpoint
RUN sed -i '1s|^#!.*|#!/usr/bin/env python3|' default_server_endpoint.py

# Stage 5: Development
FROM builder AS devel

# Add zsh
RUN wget https://github.com/robbyrussell/oh-my-zsh/raw/master/tools/install.sh -O - | zsh || true && \
    chsh -s /usr/bin/zsh root && \
    git clone --depth 1 https://github.com/bryceikeda/sky.git /root/.oh-my-zsh/custom/sky && \
    cp /root/.oh-my-zsh/custom/sky/.profile /root/ && \
    ln -sf /root/.oh-my-zsh/custom/sky/sky.zsh-theme /root/.oh-my-zsh/custom/ && \
    ln -sf /root/.oh-my-zsh/custom/sky/sky-hostname.zsh-theme /root/.oh-my-zsh/custom/ && \
    sed -i -e 's/ZSH_THEME="robbyrussell"/[[ -n $SSH_CONNECTION ]] \&\& ZSH_THEME="sky-hostname" || ZSH_THEME="sky"/' /root/.zshrc && \
    git clone --depth 1 https://github.com/zsh-users/zsh-autosuggestions /root/.oh-my-zsh/custom/plugins/zsh-autosuggestions && \
    sed -i '/plugins=(git)/c\plugins=(git pyenv colored-man-pages command-not-found extract z zsh-autosuggestions)' /root/.zshrc && \
    echo 'DISABLE_AUTO_UPDATE="true"\nENABLE_CORRECTION="false"\nHYPHEN_INSENSITIVE="true"\nsource /opt/ros/noetic/setup.zsh\nexport ROSLAUNCH_SSH_UNKNOWN=1\nsource /opt/overlay_ws/devel/setup.zsh\nsource /opt/underlay_ws/devel/setup.zsh\nsource $CARGO_HOME/env' >> /root/.zshrc

# Setup terminator
COPY dockerfiles/noetic/assets/terminator_config /root/.config/terminator/config

# Setup entrypoint
COPY dockerfiles/noetic/assets/entrypoint_setup.sh /
ENTRYPOINT ["/entrypoint_setup.sh"]
CMD ["terminator"]
