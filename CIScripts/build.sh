#! /bin/sh

PROJECT_PATH=$(pwd)/Battlestar\ Arena/
UNITY_BUILD_DIR=$(pwd)/Build

ERROR_CODE=0
echo "Items in project path ($PROJECT_PATH):"
ls "$PROJECT_PATH"

echo "Building project for Windows..."
mkdir $UNITY_BUILD_DIR
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile \
  -projectPath "$PROJECT_PATH" \
  -buildWindows64Player  "$(pwd)/Build/windows/Battlestar.exe" \
  -quit

echo "Building project for macOS..."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile \
  -projectPath "$PROJECT_PATH" \
  -buildOSXUniversalPlayer "$(pwd)/Build/osx/Battlestar.app"  \
  -quit

echo "Building project for Linux..."
/Applications/Unity/Unity.app/Contents/MacOS/Unity 
  -batchmode \
  -nographics \
  -silent-crashes \
  -projectPath "$PROJECT_PATH" \
  -buildLinuxUniversalPlayer "$(pwd)/Build/linux/Battlestar.exe" 
  -quit

if [ $? = 0 ] ; then
  echo "Building Windows exe completed successfully."
  ERROR_CODE=0
else
  echo "Building Windows exe failed. Exited with $?."
  ERROR_CODE=1
fi

echo "Finishing with code $ERROR_CODE"
exit $ERROR_CODE

echo 'Attempting to zip builds'
zip -r $(pwd)/Build/linux.zip $(pwd)/Build/linux/
zip -r $(pwd)/Build/mac.zip $(pwd)/Build/osx/
zip -r $(pwd)/Build/windows.zip $(pwd)/Build/windows/
