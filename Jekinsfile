pipeline{
    agent none
    stages{
        stage('Non-Parallel Stage'){
            agent {
                label "Master_Node"
            }
            steps{
                echo "THis stage will be executed first"
            }
        }
        stage("Run Tests"){
            parallel{
                stage("Test on window"){
                    agent{
                        label "Window_Node"
                    }
                    steps{
                        echo "Task1 on AGENT"
                    }
                }
                stage("Test on Master Node"){
                    agent{
                        label "Master_Node"
                    }
                    steps{
                        echo "Task1 on MASTER"
                    }
                }
            }
        }
    }
}
