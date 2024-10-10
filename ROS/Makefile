# If the first argument is ...
ifneq (,$(findstring tools_,$(firstword $(MAKECMDGOALS))))
	# use the rest as arguments
	RUN_ARGS := $(wordlist 2,$(words $(MAKECMDGOALS)),$(MAKECMDGOALS))
	# ...and turn them into do-nothing targets
	#$(eval $(RUN_ARGS):;@:)
endif

.PHONY: help

help: ## This help.
	@awk 'BEGIN {FS = ":.*?## "} /^[0-9a-zA-Z_-]+:.*?## / {printf "\033[36m%-42s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

.DEFAULT_GOAL := help

## BUILD
build_ros_noetic: ## [Build] Build ROS Noetic base container. "make build_ros_noetic"
	docker build -t ros1/ros-noetic:ARCADE -f dockerfiles/noetic/Dockerfile .
	@printf "\n\033[92mDocker Image: ros1/ros-noetic:base\033[0m\n"

# RUN
run_ros_noetic: ## [RUN]   Run the image noetic. Use it as "make run_ros_noetic"
	docker run -it --gpus all --privileged \
	--name ROS \
        -p 10000:10000 \
        -v /tmp/.X11-unix:/tmp/.X11-unix -e DISPLAY=$$DISPLAY \
        -v ~/.Xauthority:/root/.Xauthority -e XAUTHORITY=/root/.Xauthority \
        -v `pwd`/workspace/:/opt/overlay_ws/ \
        -v ~/.ssh:/root/.ssh \
	ros1/ros-noetic:ROS

# START
start_ros_noetic: ## [START] Start stopped container. "make start_ros_noetic [Container ID]"
	docker start $(RUN_ARGS)
