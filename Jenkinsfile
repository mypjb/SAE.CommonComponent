pipeline {
  agent any
  stages {
    stage('Select'){
        input {
                message "Please select build process"
                ok "Select"
                parameters {
                    choice(choices: ['ALL', 'API', 'Client'], description: '', name: 'BUILD_TYPE_TEMP')
                }
            }
        
        steps{
            script {
                BUILD_TYPE = BUILD_TYPE_TEMP
              }
        }
    }
    stage('Build') {
      parallel {
            stage('API') {
             when {
                not {
                   equals expected: 'Client', actual: BUILD_TYPE
                }
              }
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
        stage('Client') {
              when {
                not {
                   equals expected: 'API', actual: BUILD_TYPE
                }
              }
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