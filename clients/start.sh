#!/bin/bash

cd $(dirname $0)

echo "start childs app"

project_array=(BasicData Identity OAuth Routing Application Authorize PluginManagement User Master)

for project in ${project_array[@]};do
	echo "start run $project"
	cd $project && pnpm start &
done
