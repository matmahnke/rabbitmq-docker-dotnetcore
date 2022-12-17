

#Run tests with docker
dotnet build -t rabbitmq-dotnet-app-tests -f Dockerfile.Tests .
docker run -v /var/run/docker.sock:/var/run/docker.sock -ti rabbitmq-dotnet-app-tests

#Run tests with docker compose
docker-compose -f docker-compose.tests.yml up --build