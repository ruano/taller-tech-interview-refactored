# taller-tech-interview

This is a refactored project for: https://github.com/ruano/taller-tech-interview

Click [here](./assets/challenge.png) to see the challenge description.

## Instructions

### Build and start Kafka cluster using Docker:

```docker-compose up -d kafka```

### View and create a Kafka topic:

```docker exec -it CONTAINER_ID /opt/kafka/bin/kafka-topics.sh --list --bootstrap-server localhost:9092```

```docker exec -it CONTAINER_ID /opt/kafka/bin/kafka-topics.sh --create --topic events --bootstrap-server localhost:9092```

### Publish and receive message using Kafka sh:

- Publishing a message:

```docker exec -it CONTAINER_ID /opt/kafka/bin/kafka-console-producer.sh --topic events --bootstrap-server localhost:9092```

- Open another tab terminal to see received messages:

```docker exec -it CONTAINER_ID /opt/kafka/bin/kafka-console-consumer.sh --topic events --from-beginning --bootstrap-server localhost:9092```

### Testing

You can test endpoints using CURL or Swagger interface:

```
curl -X 'POST' \
  'https://localhost:7162/events' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "userId": "string",
  "type": "string",
  "timeStamp": "2025-10-28T20:51:33.996Z",
  "data": "string"
}'
```

```
curl -X 'GET' \
  'https://localhost:7162/events' \
  -H 'accept: */*'
```