#!/bin/bash

# intended for MacOS and Linux

# Set the platform-specific variables
if [ "$(uname)" == "Darwin" ]; then
  # macOS
  platform="darwin"
  drives=$(df -h | awk '{print $1}')
  local_drives=$(echo "$drives" | grep -E '/dev/disk[0-9]')
elif [ "$(uname)" == "Linux" ]; then
  # Linux
  platform="linux"
  drives=$(df -h | awk '{print $1}')
  local_drives=$(echo "$drives" | grep -E '/dev/sd[a-z]')
else
  # Disallow Windows
  exit 1
fi

# Select the first local drive from the list
selected_drive=$(echo "$local_drives" | head -n 1)

# Create the temp and terraform directories if they do not exist
if [ ! -d "$selected_drive/temp" ]; then
  mkdir "$selected_drive/temp"
fi
if [ ! -d "$selected_drive/terraform" ]; then
  mkdir "$selected_drive/terraform"
fi

# Download and extract Terraform to the selected drive
curl -L "$1" -o "$selected_drive/temp/terraform.zip"
unzip "$selected_drive/temp/terraform.zip" -d "$selected_drive/terraform"

# Delete the temp directory
rm -rf "$selected_drive/temp"