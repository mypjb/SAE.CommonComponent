#!/bin/bash
base_dir=$(cd $(dirname $0) && pwd)

release_dir=$1

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

mv -f dist $project_release_dir

echo "build $project end"

done