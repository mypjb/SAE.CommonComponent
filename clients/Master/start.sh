#!/bin/sh

cd $(dirname $0);

echo start childs app
# start childs
cd ../BasicData && yarn start &
cd ../Identity && yarn start &
cd ../OAuth && yarn start &
cd ../Routing && yarn start &
cd ../Application && yarn start &
cd ../Authorize && yarn start &
cd ../PluginManagement && yarn start &
cd ../User && yarn start &

echo start master app
# start master 
cd ../Master && yarn start &
