pipeline {
  agent {
    docker {
      image 'mypjb/dotnet-core-sdk:3.1'
      args '-v /home/mypjb/.nuget/packages:/root/.nuget/packages'
    }

  }
  stages {
    stage('Build') {
      steps {
        sh '''dotnet nuget add source ${DOTNET_NUGET_SOURCE} -n azure.org

dotnet build -c Release'''
      }
    }

  }
}