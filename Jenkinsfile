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
            sh 'echo dotnet test -v n'
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

  }
}