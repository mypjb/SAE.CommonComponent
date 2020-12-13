pipeline {
  agent any
  parameters {
        choice(choices: ['Config','API', 'Client', 'ALL'], description: '', name: 'BUILD_TARGET')
  }
  stages {
    stage('Build') {
      parallel {
        stage('Config') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'Config'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
          agent {
            docker {
              image 'mypjb/dotnet-core-sdk:3.1'
              args '-v nuget:/root/.nuget -v release:/root/release'
            }

          }
          steps {
            sh 'bash ./build.config.sh $RELEASE_DIR/Config'
          }
        }

        stage('API') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'API'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
          agent {
            docker {
              image 'mypjb/dotnet-core-sdk:3.1'
              args '-v nuget:/root/.nuget -v release:/root/release'
            }

          }
          steps {
            sh 'bash ./build.sh $RELEASE_DIR/API'
          }
        }

        stage('Client') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'Client'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
          agent {
            docker {
              image 'andrewmackrodt/nodejs:12'
              args '-v yarn:/usr/local/share/.cache/yarn -v release:/root/release'
            }

          }
          steps {
            sh 'bash ./clients/build.sh $RELEASE_DIR/Client'
          }
        }

      }
    }
	
	stage('Pack') {
      parallel {
        stage('Config') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'Config'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
          environment {
            DOCKER_BUILD_DIR = "${RELEASE_DIR}/Config/Master"
            MAIN_PROGRAM = 'SAE.CommonComponent.Master.dll'
            DOCKER_NAME = 'mypjb/sae-commoncomponent-config'
            DOCKER_TAG = "${BRANCH_NAME}"
          }
          steps {
            sh '''cd $(echo $DOCKER_BUILD_DIR | sed 's/%2F/\\//g')
docker build --rm --build-arg MAIN_PROGRAM=$MAIN_PROGRAM -t $DOCKER_NAME:$(echo $DOCKER_TAG | sed "s/\\//-/g") .'''
          }
        }

        stage('API') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'API'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
          environment {
            DOCKER_BUILD_DIR = "${RELEASE_DIR}/API/Master"
            MAIN_PROGRAM = 'SAE.CommonComponent.Master.dll'
            DOCKER_NAME = 'mypjb/sae-commoncomponent-master'
            DOCKER_TAG = "${BRANCH_NAME}"
          }
          steps {
            sh '''cd $(echo $DOCKER_BUILD_DIR | sed 's/%2F/\\//g')
docker build --rm --build-arg MAIN_PROGRAM=$MAIN_PROGRAM -t $DOCKER_NAME:$(echo $DOCKER_TAG | sed "s/\\//-/g") .'''
          }
        }

        stage('Client') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'Client'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
		  environment {
            DOCKER_BUILD_DIR = "${RELEASE_DIR}/Client"
            DOCKER_NAME = 'mypjb/sae-commoncomponent-client'
            DOCKER_TAG = "${BRANCH_NAME}"
          }
          steps {
            sh '''cd $(echo $DOCKER_BUILD_DIR | sed 's/%2F/\\//g')
docker build --rm -t $DOCKER_NAME:$(echo $DOCKER_TAG | sed "s/\\//-/g") .'''
          }
        }
		
      }
    }
	
	stage('Deploy') {
      parallel {
        stage('Config') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'Config'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
          environment {
            DOCKER_NAME = 'mypjb/sae-commoncomponent-config'
            DOCKER_TAG = "${BRANCH_NAME}"
			DOCKER_CONTAINER_NAME="sae-commoncomponent-config"
            DOCKER_PORT = "9001"
          }
          steps {
            sh '''if [ $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME  | wc -l) != 0 ]; then docker rm -f $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME); fi
docker run -d --name $DOCKER_CONTAINER_NAME --net=$DOCKER_CLUSTER_NETWORK -e ASPNETCORE_ConfigServer__Url=http://config.sae.com/app/config?id=0dbbcfdf123f44baad50ac830106c87b&env=Production -e ASPNETCORE_ConfigServer__PollInterval=00:00:05  -p $DOCKER_PORT:80 $DOCKER_NAME:$(echo $DOCKER_TAG | sed "s/\\//-/g") '''
          }
        }

        stage('API') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'API'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
          environment {
            DOCKER_NAME = 'mypjb/sae-commoncomponent-master'
            DOCKER_TAG = "${BRANCH_NAME}"
			DOCKER_CONTAINER_NAME="sae-commoncomponent-master"
            DOCKER_PORT = "9002"
          }
          steps {
            sh '''if [ $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME  | wc -l) != 0 ]; then docker rm -f $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME); fi
docker run -d --name $DOCKER_CONTAINER_NAME --net=$DOCKER_CLUSTER_NETWORK -p $DOCKER_PORT:80 $DOCKER_NAME:$(echo $DOCKER_TAG | sed "s/\\//-/g") '''
          }
        }

        stage('Client') {
		  when { 
            anyOf {
                environment name: 'BUILD_TARGET', value: 'Client'
                environment name: 'BUILD_TARGET', value: 'ALL'
            }
          }
		  environment {
            DOCKER_NAME = 'mypjb/sae-commoncomponent-client'
            DOCKER_TAG = "${BRANCH_NAME}"
			DOCKER_CONTAINER_NAME="sae-commoncomponent-client"
            DOCKER_PORT = "9003"
          }
          steps {
            sh '''if [ $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME  | wc -l) != 0 ]; then docker rm -f $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME); fi
docker run -d --name $DOCKER_CONTAINER_NAME --net=$DOCKER_CLUSTER_NETWORK -p $DOCKER_PORT:80 $DOCKER_NAME:$(echo $DOCKER_TAG | sed "s/\\//-/g") '''
          }
        }
		
      }
    }
  }
}