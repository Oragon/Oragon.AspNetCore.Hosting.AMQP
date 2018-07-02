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
                // git branch: 'master', credentialsId: 'GITHUB_USERNAME', url: 'https://github.com/Oragon/Oragon.AspNetCore.Hosting.AMQP.git'
                
                sh 'dotnet build ./Oragon.AspNetCore.Hosting.AMQP.sln'
            }
        }
        stage('Test') {
            steps {
                sh 'dotnet test ./Oragon.AspNetCore.Hosting.AMQPTests/Oragon.AspNetCore.Hosting.AMQPTests.csproj --configuration Debug --output ../output-tests'
            }
        }
        stage('Pack') {
            steps {
                sh 'dotnet pack ./Oragon.AspNetCore.Hosting.AMQP/Oragon.AspNetCore.Hosting.AMQP.csproj --configuration Debug --include-source --include-symbols --output ../output-packages'
            }
        }
        stage('Publish') {
            when { tag "*-alpha" }
            steps {
				withCredentials([usernamePassword(credentialsId: 'myget-oragon', passwordVariable: 'MYGET_KEY')]) {
					sh 'dotnet nuget push $(ls ./output-packages/*.nupkg)  -k "$MYGET_KEY" -s https://www.myget.org/F/oragon-alpha/api/v3/index.json'
				}
            }
        }
    }
}