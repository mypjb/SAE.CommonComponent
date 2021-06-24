#!/bin/sh

cd $(dirname $0);

echo start childs app
# start childs
cd ../Identity && tyarn start &
cd ../OAuth && tyarn start &
cd ../Routing && tyarn start &
cd ../ConfigServer && tyarn start &
cd ../Authorize && tyarn start &

echo start master app
# start master 
cd ../Master && tyarn start &
