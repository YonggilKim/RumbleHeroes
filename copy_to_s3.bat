@echo off
aws s3 cp ServerData s3://rookiss-rumble-addressables/Android/ --recursive

pause