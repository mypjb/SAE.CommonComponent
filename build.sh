#!/bin/bash

release_dir=$1

project_array=(Application Authorize ConfigServer Identity OAuth Routing User Master)

for project in ${project_array[@]};do

echo "start build $project"

output_dir=${release_dir}/${project}

plugin_setting_file=${output_dir}/package.json

project_file=src/SAE.CommonComponent.${project}/SAE.CommonComponent.${project}.csproj

version=$(grep -Po "<Version>([\d\.a-zA-Z])+</Version>" $project_file | grep -Po "[\d]+[^<>]+")

echo -e "	output_dir:${output_dir}	\n	project_file:${project_file}	\n	version:${version}"

if [ $project != 'Master' ]
then
cat > $plugin_setting_file << EOF
{
	"Name": "${project}",
	"Path": "SAE.CommonComponent.${project}.dll",
	"Description": "this is a ${project} plugin",
	"Version": "${version}",
	"Status":1
}
EOF

echo -e "${project} plugin setting ↓↓↓↓↓↓↓↓↓"
cat $plugin_setting_file
fi

dotnet publish -c release -o ${output_dir} "src/SAE.CommonComponent.${project}"

echo "build $project end"

done