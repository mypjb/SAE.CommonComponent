pipeline {
  agent any
  stages {
    stage('Build') {
      parallel {
        stage('API') {
          agent {
            docker {
              image 'mypjb/dotnet-core-sdk:3.1'
              args '-v nuget:/root/.nuget'
            }

          }
          steps {
            sh 'echo dotnet test -v n'
            sh 'bash ./build.sh $RELEASE_DIR'
          }
        }

        stage('Client') {
          agent {
            docker {
              image 'andrewmackrodt/nodejs:12'
              args '-v yarn:/usr/local/share/.cache/yarn'
            }

          }
          steps {
            sh 'bash ./clients/build.sh $RELEASE_DIR/client'
          }
        }

      }
    }

  }
}