#!/bin/bash

base_dir=$(cd $(dirname $0) && pwd)

# running Unit test
#dotnet test -v n

release_dir=$(echo $1 | sed 's/%2F/\//g')

main_dir=$release_dir/Master

plugin_dir=$main_dir/plugins

mkdir -p $plugin_dir

project_array=(Application Authorize Identity OAuth Routing User InitializeData ConfigServer Master)

index=0

for project in ${project_array[@]};do

echo "start build $project"

output_dir=${release_dir}/${project}

plugin_setting_file=${output_dir}/package.json

project_file=src/SAE.CommonComponent.${project}/SAE.CommonComponent.${project}.csproj

version=$(grep -Po "<Version>([\d\.a-zA-Z])+</Version>" $project_file | grep -Po "[\d]+[^<>]+")

echo -e "	output_dir:${output_dir}	\n	project_file:${project_file}	\n	version:${version}"

dotnet publish -c release -o ${output_dir} "src/SAE.CommonComponent.${project}"

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

mv -f $output_dir $plugin_dir

fi

echo "build $project end"

cp -f Dockerfile $main_dir

done