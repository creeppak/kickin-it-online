#!/bin/bash

# Variables
UNITY_PATH="C:/Program Files/Unity/Hub/Editor/2022.3.60f1/Editor/Unity.exe"
PROJECT_PATH="C:/Workshop/Dev/Kickin_It_Online_URP"
BUILD_METHOD_NAME="Build.BuildApi.BuildWebRelease"
BUILD_PATH="./Builds"
DOCKER_IMAGE_NAME="kick"
RASPBERRY_PI_USER="creeppak"
RASPBERRY_PI_HOST="raspberrypi.local"
RASPBERRY_PI_PATH="/home/creeppak/projects/kick"

set -o pipefail

## Step 1: Build Unity Project
#"$UNITY_PATH" -batchmode -nographics -quit \
#  -projectPath "$PROJECT_PATH" \
#  -executeMethod $BUILD_METHOD_NAME \
#  -logFile build.log
#
## Check if Unity CLI succeeded
#if [ $? -eq 0 ]; then
#    echo "Build completed successfully."
#else
#    echo "Build failed (Code $?). Check Unity logs for details."
#    exit 1
#fi

# Step 2: Dockerize the Build
docker buildx build --platform linux/arm64/v8 -t $DOCKER_IMAGE_NAME:latest --load .

if [ $? -eq 0 ]; then
    echo "Docker build completed successfully."
else
    echo "Docker build failed (Code $?)."
    exit 1
fi

# Step 3: Push or Transfer Image
docker save $DOCKER_IMAGE_NAME:latest | gzip > $DOCKER_IMAGE_NAME.tar.gz
scp $DOCKER_IMAGE_NAME.tar.gz $RASPBERRY_PI_USER@$RASPBERRY_PI_HOST:$RASPBERRY_PI_PATH
  
if [ $? -eq 0 ]; then
    echo "Docker image push completed successfully."
else
    echo "Docker image push failed (Code $?)."
    exit 1
fi

# Step 4: Deploy on Raspberry Pi
ssh $RASPBERRY_PI_USER@$RASPBERRY_PI_HOST << EOF
  docker load < $RASPBERRY_PI_PATH/$DOCKER_IMAGE_NAME.tar.gz
  docker stop $DOCKER_IMAGE_NAME || true
  docker rm $DOCKER_IMAGE_NAME || true
  docker run -d --name $DOCKER_IMAGE_NAME -p 8082:80 $DOCKER_IMAGE_NAME:latest
EOF

echo "Deployment complete!"