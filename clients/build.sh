#!/bin/bash
base_dir=$(cd $(dirname $0) && pwd)

release_dir=$1

app_dir=$release_dir/app

main_dir=$release_dir/Master

plugin_dir=$main_dir/plugins

mkdir -p $plugin_dir

echo -e "build workspace ${base_dir}"

project_array=(ConfigServer Identity Master OAuth Routing)

for project in ${project_array[@]};do

echo "start build $project"

project_release_dir=$release_dir/$project

project_dir=$base_dir/$project

echo -e "	project_dir:${project_dir}"

cd $project_dir
yarn

yarn build

if [ $project != 'Master' ]
then
	project_release_dir=$plugin_dir/$project
fi

mv -f dist $project_release_dir

echo "build $project end"

done

mv $main_dir $app_dir