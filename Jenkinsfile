pipeline {
 
    agent any
 
    environment {
        IMAGE = "practo-backend:${BUILD_NUMBER}"
        NETWORK = "practo_net"
        MYSQL_CONT = "practo_mysql"
        API_CONT = "practo_backend"
 
        MYSQL_PWD = "root"
        MYSQL_DB = "practodb"
    }
 
    stages {
 
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
 
        stage('Build Docker Image') {
            steps {
                bat "docker build -t %IMAGE% ."
            }
        }
 
        stage('Create Network') {
            steps {
                bat "docker network create %NETWORK% 2>nul"
            }
        }
 
        stage('Start MySQL') {
            steps {
                bat """
                docker rm -f %MYSQL_CONT% 2>nul
 
                docker run -d --name %MYSQL_CONT% --network %NETWORK% ^
                    -e MYSQL_ROOT_PASSWORD=%MYSQL_PWD% ^
                    -e MYSQL_DATABASE=%MYSQL_DB% ^
                    -p 3307:3306 ^
                    -v mysql-data:/var/lib/mysql ^
                    mysql:8.0
                """
            }
        }
 
        stage('Wait for MySQL (HEALTHCHECK equivalent)') {
            steps {
                bat """
                echo Waiting for MySQL to be ready...
 
                :loop
                docker exec %MYSQL_CONT% mysqladmin ping -h localhost -uroot -p%MYSQL_PWD% >nul 2>&1
 
                IF ERRORLEVEL 1 (
                    timeout /t 5 >nul
                    goto loop
                )
 
                echo MySQL is ready!
                """
            }
        }
 
        stage('Run API') {
            steps {
                bat """
                docker rm -f %API_CONT% 2>nul
 
                docker run -d --name %API_CONT% --network %NETWORK% ^
                    -e ASPNETCORE_ENVIRONMENT=Development ^
                    -e ASPNETCORE_URLS=http://+:8080 ^
                    -e ConnectionStrings__DefaultConnection="Server=%MYSQL_CONT%;Port=3306;Database=%MYSQL_DB%;User=root;Password=%MYSQL_PWD%;" ^
                    -e Jwt__Issuer=practo_backend ^
                    -e Jwt__Audience=practo_frontend ^
                    -e Jwt__Key=super_secret_practo_clone_key_1234567890_extremely_long_key_for_security ^
                    -e Smtp__Host=smtp.gmail.com ^
                    -e Smtp__Port=587 ^
                    -e Smtp__Username=nandanar1711@gmail.com ^
                    -e Smtp__Password=wbzsvaezmwvrsoff ^
                    -e Smtp__EnableSsl=true ^
                    -e Razorpay__KeyId=rzp_test_SykLoVuV65Z8mA ^
                    -e Razorpay__KeySecret=LXOt8Mvg7wtb3cFKcrcOqFxV ^
                    -p 5016:8080 ^
                    %IMAGE%
                """
            }
        }
 
    }
}