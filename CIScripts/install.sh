#!/bin/sh

BASE_URL=https://netstorage.unity3d.com/unity
HASH=4550892b6062
VERSION=2018.2.18f1

download() {
    file=$1
    url="$BASE_URL/$HASH/$package"

    echo "Downloading from $url: "
    curl -o `basename "$package"` "$url"
}

install() {
    package=$1
    download "$package"

    echo "Installing "`basename "$package"`
    sudo installer -dumplog -package `basename "$package"` -target /
}

install "MacEditorInstaller/Unity-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Windows-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Mac-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Linux-Support-for-Editor-$VERSION.pkg"
