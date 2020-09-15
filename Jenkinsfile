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
			echo 'build client'
            //sh 'bash ./clients/build.sh $RELEASE_DIR/Client'
          }
        }

      }
    }
	
	stage('Pack') {
	
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

  }
}