#!/bin/bash

cd $(dirname $0)

base_dir=$(pwd)

release_dir=$(echo $1 | sed 's/%2F/\//g')

app_dir=$release_dir/app

mkdir -p $app_dir

echo -e "build workspace ${base_dir},start pull workspace package"

yarn config set registry https://registry.npm.taobao.org

yarn

project_array=(ConfigServer Identity Master OAuth Routing Authorize User)

for project in ${project_array[@]};do

echo "start build $project"

project_release_dir=$app_dir/$project

project_dir=$base_dir/$project

echo -e "	project_dir:${project_dir}"

cd $project_dir

yarn build 

project_release_dir=$app_dir/$(echo $project | tr '[A-Z]' '[a-z]')

echo -e "	project_release_dir:$project_release_dir"

mv -f dist $project_release_dir

echo "build $project end"

done

cp -f $base_dir/default.conf $release_dir
cp -f $base_dir/Dockerfile $release_dir
