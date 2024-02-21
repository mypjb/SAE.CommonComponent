#!/bin/bash

base_dir=$(cd $(dirname $0) && pwd)

release_dir=${1:-"plugins"}

props_file=Directory.Build.props
#runtime xunit test
#dotnet test -v q -l "console;verbosity=detailed"

mkdir -p $release_dir

base_version=$(grep -Po "<Version>([\d\.a-zA-Z])+</Version>" $props_file | grep -Po "[\d]+[^<>]+")

project_array=( BasicData MultiTenant Application Authorize Identity OAuth Routing User ConfigServer PluginManagement InitializeData)

if [ x"$2" != x ]; then 
project_array=($2);
fi

index=0

for project in ${project_array[@]};do

echo "start build $project"

output_dir=${release_dir}/${project}

plugin_setting_file=${output_dir}/package.json

project_file=src/SAE.CommonComponent.${project}/SAE.CommonComponent.${project}.csproj

version=$(grep -Po "<Version>([\d\.a-zA-Z])+</Version>" $project_file | grep -Po "[\d]+[^<>]+")

if [ "${version}" == '' ]
then
version=${base_version}
fi

echo -e "	output_dir:${output_dir}	\n	project_file:${project_file}	\n	version:${version}"

dotnet publish -c Debug -o ${output_dir} "src/SAE.CommonComponent.${project}"

if [ $project != 'Master' ]
then
cat << EOF > $plugin_setting_file
{
	"Name": "${project}",
	"Path": "SAE.CommonComponent.${project}.dll",
	"Description": "this is a ${project} plugin",
	"Version": "${version}",
	"Status":1,
	"Order":${index}
}
EOF

let "index=index + 1";

echo -e "${project} plugin setting ↓↓↓↓↓↓↓↓↓"
cat $plugin_setting_file

fi

echo "build $project end"

done
