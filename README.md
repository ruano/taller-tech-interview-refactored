# taller-tech-interview

This is a refactored project for: https://github.com/ruano/taller-tech-interview

Click [here](./assets/challenge.png) to see the challenge description.

- View and create a Kafka topic:

docker exec -it e99e1b434196 /opt/kafka/bin/kafka-topics.sh --list --bootstrap-server localhost:9092
docker exec -it e99e1b434196 /opt/kafka/bin/kafka-topics.sh --create --topic events --bootstrap-server localhost:9092