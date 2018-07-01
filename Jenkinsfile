pipeline {
    agent {
        docker { 
            alwaysPull false
            image 'microsoft/dotnet:2.1-sdk'
            reuseNode false
            args '-u root:root'
        }
    }
    stages {
      
        stage('Build') {
            steps {
                git branch: 'develop', credentialsId: 'GITHUB_USERNAME', url: 'https://github.com/Oragon/Oragon.AspNetCore.Hosting.AMQP.git'
                
                sh 'dotnet build ./Oragon.AspNetCore.Hosting.AMQP.sln'
            }
        }
        stage('Test') {
            steps {
                echo 'Sem Testes'
            }
        }
        stage('Pack') {
            steps {
                sh 'dotnet pack ./Oragon.AspNetCore.Hosting.AMQP/Oragon.AspNetCore.Hosting.AMQP.csproj --configuration Debug --include-source --include-symbols --output ../output-packages'
            }
        }
        
    }
}