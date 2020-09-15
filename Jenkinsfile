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
			sh 'echo build client'
            //sh 'bash ./clients/build.sh $RELEASE_DIR/Client'
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
          steps {
            sh 'echo pack client'
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
			DOCKER_PORT='sae.com:80'
          }
          steps {
            sh '''docker rm -f $(docker ps -f ancestor=$DOCKER_NAME -q)
docker run -d --name $DOCKER_CONTAINER_NAME -p $DOCKER_PORT:80 $DOCKER_NAME:$DOCKER_TAG '''
          }
        }

        stage('Client') {
          steps {
            sh 'echo deploy client'
          }
        }
		
      }
    }

  }
}