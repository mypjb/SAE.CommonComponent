pipeline {
  agent {
    docker {
      image 'mypjb/dotnet-core-sdk:3.1'
      args '-v /home/mypjb/.nuget:/root/.nuget'
    }

  }
  stages {
    stage('Build') {
      steps {
        sh 'dotnet build -c Release'
      }
    }

  }
}