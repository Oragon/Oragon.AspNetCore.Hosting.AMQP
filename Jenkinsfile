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

                    if (env.BRANCH_NAME.endsWith("-alpha")) {

                        sh 'dotnet pack ./Oragon.AspNetCore.Hosting.AMQP/Oragon.AspNetCore.Hosting.AMQP.csproj --configuration Debug /p:PackageVersion="$BRANCH_NAME" --include-source --include-symbols --output ../output-packages'

                    } else if (env.BRANCH_NAME.endsWith("-beta")) {

                        sh 'dotnet pack ./Oragon.AspNetCore.Hosting.AMQP/Oragon.AspNetCore.Hosting.AMQP.csproj --configuration Release /p:PackageVersion="$BRANCH_NAME" --output ../output-packages'                        

                    } else {

                        sh 'dotnet pack ./Oragon.AspNetCore.Hosting.AMQP/Oragon.AspNetCore.Hosting.AMQP.csproj --configuration Release /p:PackageVersion="$BRANCH_NAME" --output ../output-packages'

                    }

                }

            }

        }

        stage('Publish') {

            when { buildingTag() }

            steps {
                
                script {
                    
                    if (env.BRANCH_NAME.endsWith("-alpha")) {
                        
                        withCredentials([usernamePassword(credentialsId: 'myget-oragon', passwordVariable: 'MYGET_KEY', usernameVariable: 'DUMMY' )]) {

                            sh 'dotnet nuget push $(ls ./output-packages/*.nupkg)  -k "$MYGET_KEY" -s https://www.myget.org/F/oragon-alpha/api/v3/index.json'

                        }

                    } else if (env.BRANCH_NAME.endsWith("-beta")) {

                        withCredentials([usernamePassword(credentialsId: 'myget-oragon', passwordVariable: 'MYGET_KEY', usernameVariable: 'DUMMY')]) {

                            sh 'dotnet nuget push $(ls ./output-packages/*.nupkg)  -k "$MYGET_KEY" -s https://www.myget.org/F/oragon-beta/api/v3/index.json'

                        }

                        
                    } else {

                        withCredentials([usernamePassword(credentialsId: 'nuget-luizcarlosfaria', passwordVariable: 'NUGET_KEY', usernameVariable: 'DUMMY')]) {

                            sh 'dotnet nuget push $(ls ./output-packages/*.nupkg)  -k "$NUGET_KEY"'

                        }

                    }                    
				}
            }
        }
    }
}