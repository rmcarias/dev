#!/bin/bash
rm -rf ./dist
rm index.zip
mkdir dist
cp package.json ./dist/
cp ./src/*.js ./dist/
cp -R  ./node_modules ./dist/
cd ./dist
zip -r -X  index.zip *
#cd ..
aws lambda update-function-code --function-name MobileAuth --zip-file fileb://index.zip --profile <yourprofile>