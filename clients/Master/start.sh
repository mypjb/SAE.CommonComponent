#!/bin/sh

cd $(dirname $0);

echo start childs app
# start childs
cd ../BasicData && pnpm start &
cd ../Identity && pnpm start &
cd ../OAuth && pnpm start &
cd ../Routing && pnpm start &
cd ../Application && pnpm start &
cd ../Authorize && pnpm start &
cd ../PluginManagement && pnpm start &
cd ../User && pnpm start &

echo start master app
# start master 
cd ../Master && pnpm start &
