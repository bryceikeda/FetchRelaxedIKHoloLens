#!/bin/bash

# Check if the shell is Zsh
if [ -n "$ZSH_VERSION" ]; then
    # Running in Zsh
    source /opt/ros/noetic/setup.zsh
    if [ -f /opt/overlay_ws/devel/setup.zsh ]; then
        source /opt/overlay_ws/devel/setup.zsh
        echo "Sourced overlay_ws"
    fi
    if [ -f /opt/underlay_ws/devel/setup.zsh ]; then
        source /opt/underlay_ws/devel/setup.zsh
        echo "Sourced underlay_ws"
    fi
else
    # Running in Bash or another shell
    source /opt/ros/noetic/setup.bash
    if [ -f /opt/overlay_ws/devel/setup.bash ]; then
        source /opt/overlay_ws/devel/setup.bash
    fi
    if [ -f /opt/underlay_ws/devel/setup.bash ]; then
        source /opt/underlay_ws/devel/setup.bash
    fi
fi

echo "Sourced Catkin workspace!"

# Define the desired directory
TARGET_DIRECTORY="/opt/overlay_ws/"

# Check if the directory exists
if [ -d "$TARGET_DIRECTORY" ]; then
    # Change directory to the target directory
    cd "$TARGET_DIRECTORY" || exit
    echo "Changed directory to $TARGET_DIRECTORY"
else
    echo "Directory $TARGET_DIRECTORY does not exist."
fi

# Execute the command passed into this entrypoint
exec "$@"
