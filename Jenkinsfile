pipeline {
  agent any
  stages {
    stage('Build') {
      parallel {
        stage('API') {
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
        stage('API') {
          environment {
            DOCKER_BUILD_DIR = "${RELEASE_DIR}/API/Master"
            MAIN_PROGRAM = 'SAE.CommonComponent.Master.dll'
            DOCKER_NAME = 'mypjb/sae-commoncomponent-master'
            DOCKER_TAG = '1.0.0'
          }
          steps {
            sh '''cd $DOCKER_BUILD_DIR
docker build --rm --build-arg MAIN_PROGRAM=$MAIN_PROGRAM -t $DOCKER_NAME:$DOCKER_TAG .'''
          }
        }

        stage('Client') {
		  environment {
            DOCKER_BUILD_DIR = "${RELEASE_DIR}/Client"
            DOCKER_NAME = 'mypjb/sae-commoncomponent-client'
            DOCKER_TAG = '1.0.0'
          }
          steps {
            sh '''cd $DOCKER_BUILD_DIR
docker build --rm -t $DOCKER_NAME:$DOCKER_TAG .'''
          }
        }
		
      }
    }
	
	stage('Deploy') {
      parallel {
        stage('API') {
          environment {
            DOCKER_NAME = 'mypjb/sae-commoncomponent-master'
            DOCKER_TAG = '1.0.0'
			DOCKER_CONTAINER_NAME="sae-commoncomponent-master"
          }
          steps {
            sh '''if [ $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME  | wc -l) != 0 ]; then docker rm -f $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME); fi
docker run -d --name $DOCKER_CONTAINER_NAME --net=$DOCKER_CLUSTER_NETWORK -e ASPNETCORE_ENVIRONMENT=Development $DOCKER_NAME:$DOCKER_TAG '''
          }
        }

        stage('Client') {
		  environment {
            DOCKER_NAME = 'mypjb/sae-commoncomponent-client'
            DOCKER_TAG = '1.0.0'
			DOCKER_CONTAINER_NAME="sae-commoncomponent-client"
          }
          steps {
            sh '''if [ $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME  | wc -l) != 0 ]; then docker rm -f $(docker ps -q -a -f name=$DOCKER_CONTAINER_NAME); fi
docker run -d --name $DOCKER_CONTAINER_NAME --net=$DOCKER_CLUSTER_NETWORK $DOCKER_NAME:$DOCKER_TAG '''
          }
        }
		
      }
    }
	
	stage('Reload Nginx') {
      steps {
            sh 'docker restart nginx'
          }
    }

  }
}