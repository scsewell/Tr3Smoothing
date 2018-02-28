#!/bin/bash

BASE_DIR=$(cd `dirname $0` && pwd)
SOLN_DIR="${BASE_DIR}/SoSmooth"

PROJECT="${SOLN_DIR}/SoSmooth.csproj"

DEPEND_DIR="${SOLN_DIR}/Dependencies"
RELEASE_DIR="${SOLN_DIR}/bin/Release"
DEBUG_DIR="${SOLN_DIR}/bin/Debug"

CONGIFURATION="Release"
OUTPUT_DIR=$RELEASE_DIR

if [ -d "$OUTPUT_DIR" ]; then rm -Rf $OUTPUT_DIR; fi
cd $OUTPUT_DIR

xbuild $PROJECT /v:m /p:Configuration=$CONGIFURATION;OutDir=$OUTPUT_DIR
LIB_DIR="${OUTPUT_DIR}/lib/"
mkdir $LIB_DIR
cp "${DEPEND_DIR}/OpenTK.dll"           $LIB_DIR
cp "${DEPEND_DIR}/OpenTK.dll.config"    $LIB_DIR
cp "${DEPEND_DIR}/GLWidget.dll"         $LIB_DIR

zip -r "${OUTPUT_DIR}/SoSmooth.zip" $OUTPUT_DIR

CONGIFURATION="Debug"
OUTPUT_DIR=$DEBUG_DIR

if [ -d "$OUTPUT_DIR" ]; then rm -Rf $OUTPUT_DIR; fi
cd $OUTPUT_DIR

xbuild $PROJECT /v:m /p:Configuration=$CONGIFURATION;OutDir=$OUTPUT_DIR
LIB_DIR="${OUTPUT_DIR}/lib/"
mkdir $LIB_DIR
cp "${DEPEND_DIR}/OpenTK.dll"           $LIB_DIR
cp "${DEPEND_DIR}/OpenTK.dll.config"    $LIB_DIR
cp "${DEPEND_DIR}/GLWidget.dll"         $LIB_DIR