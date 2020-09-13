pipeline {
  agent any
  stages {
    stage('Select'){
        input message: 'Please select build process', parameters: [choice(choices: ['0', '1', '2'], description: '', name: 'BUILD_TYPE')]
    }
    stage('API') {
      parallel {
      if(BUILD_TYPE != 2) {
            stage('Build') {
              agent {
                docker {
                  image 'mypjb/dotnet-core-sdk:3.1'
                  args '-v nuget:/root/.nuget'
                }

              }
              steps {
                sh 'echo dotnet test -v n'
                sh '''dotnet publish -c release -o $RELEASE_DIR/Application src/SAE.CommonComponent.Application
    dotnet publish -c release -o $RELEASE_DIR/Authorize src/SAE.CommonComponent.Authorize
    dotnet publish -c release -o $RELEASE_DIR/ConfigServer src/SAE.CommonComponent.ConfigServer
    dotnet publish -c release -o $RELEASE_DIR/Identity src/SAE.CommonComponent.Identity
    dotnet publish -c release -o $RELEASE_DIR/Master src/SAE.CommonComponent.Master
    dotnet publish -c release -o $RELEASE_DIR/OAuth src/SAE.CommonComponent.OAuth
    dotnet publish -c release -o $RELEASE_DIR/Routing src/SAE.CommonComponent.Routing
    dotnet publish -c release -o $RELEASE_DIR/User src/SAE.CommonComponent.User
    '''
              }
            }
        }
        if(BUILD_TYPE != 1) {
        stage('Client') {
              agent {
                docker {
                  image 'andrewmackrodt/nodejs:12'
                  args '-v yarn:/usr/local/share/.cache/yarn'
                }

              }
              steps {
                sh '''cd clients
    yarn
    cd ConfigServer
    yarn build
    cd ../Identity
    yarn
    yarn build
    cd ../Master
    yarn
    yarn build
    cd ../OAuth
    yarn
    yarn build
    cd ../Routing
    yarn
    yarn build'''
              }
            }
        }

      }
    }

  }
}