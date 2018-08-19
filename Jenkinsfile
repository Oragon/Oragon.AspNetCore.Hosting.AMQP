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
                
                echo sh(script: 'env|sort', returnStdout: true)

                sh 'dotnet build ./Oragon.AspNetCore.Hosting.AMQP.sln'

            }

        }

        stage('Test') {

            steps {

                sh 'dotnet test ./Oragon.AspNetCore.Hosting.AMQPTests/Oragon.AspNetCore.Hosting.AMQPTests.csproj --configuration Debug --output ../output-tests'
            }

        }

        stage('Pack') {

            when { buildingTag() }

            steps {

                script{

                    def projetcs = [
                        './Oragon.AspNetCore.Hosting.AMQP/Oragon.AspNetCore.Hosting.AMQP.csproj'
                    ]

                    if (env.BRANCH_NAME.endsWith("-alpha")) {

                        for (int i = 0; i < projetcs.size(); ++i) {
                            sh "dotnet pack ${projetcs[i]} --configuration Debug /p:PackageVersion=${BRANCH_NAME} --include-source --include-symbols --output ../output-packages"
                        }

                    } else if (env.BRANCH_NAME.endsWith("-beta")) {

                        for (int i = 0; i < projetcs.size(); ++i) {
                            sh "dotnet pack ${projetcs[i]} --configuration Release /p:PackageVersion=${BRANCH_NAME} --output ../output-packages"                        
                        }

                    } else {

                        for (int i = 0; i < projetcs.size(); ++i) {
                            sh "dotnet pack ${projetcs[i]} --configuration Release /p:PackageVersion=${BRANCH_NAME} --output ../output-packages"                        
                        }

                    }

                }

            }

        }

        stage('Publish') {

            when { buildingTag() }

            steps {
                
                script {
                    
                    def publishOnNuGet = ( env.BRANCH_NAME.endsWith("-alpha") == false );
                        
                        withCredentials([usernamePassword(credentialsId: 'myget-oragon', passwordVariable: 'MYGET_KEY', usernameVariable: 'DUMMY' )]) {

                        sh 'for pkg in ./output-packages/*.nupkg ; do dotnet nuget push "$pkg" -k "$MYGET_KEY" -s https://www.myget.org/F/oragon/api/v3/index.json ; done'

                        }

                    if (publishOnNuGet) {

                        withCredentials([usernamePassword(credentialsId: 'nuget-luizcarlosfaria', passwordVariable: 'NUGET_KEY', usernameVariable: 'DUMMY')]) {

                            sh 'for pkg in ./output-packages/*.nupkg ; do dotnet nuget push "$pkg" -k "$NUGET_KEY" -s https://api.nuget.org/v3/index.json ; done'

                        }

                    }                    
				}
            }
        }
    }
}