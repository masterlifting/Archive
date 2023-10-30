## Chemikalien-Gesellschaft Hans Lungmuß mbH & Co. KG

Project ID: `FF-165` (Always use this ID, in addition to the GitHub issue ID, in the commit messages)

Please use [Conventional Commits](https://www.conventionalcommits.org) for commit messages.

# Feuerfest

In this repository you will find all the information needed to work on the Feuerfest application.

## Podman / Docker

## Dapr

Initializing Dapr with the podman runtime

```bash
dapr init --container-runtime podman
```

Remove the Dapr runtime

```bash
dapr uninstall --all
```

## Build Postgres container

Building the database container on the local machine.
You cannot push to the github registry from the local machine.
If you create new databases, you can add them to the init.sql script and rebuild the images locally.
This images is then used in the subsequent steps.

```bash
podman build -t ghcr.io/lungmuss/lungmuss.postgres  .
```

Running all needed containers on the local machine in a "docker compose" fashion.

[Moving from docker-compose to Podman pods](https://www.redhat.com/sysadmin/compose-podman-pods).

[Podman: Managing pods and containers in a local container runtime](https://developers.redhat.com/blog/2019/01/15/podman-managing-containers-pods)

Create a pod named `feuerfest` with the following command:

```bash
podman pod create --name feuerfest -p 5432:5432 -p 9092:9092
```

List the pod you created:

```bash
podman pod list
```

Now add the database to the pod (note the `--pod=feuerfest` parameter).

```bash
podman run -d --restart=always --name temporal-db --pod=feuerfest  -e POSTGRES_PASSWORD=test*12* ghcr.io/lungmuss/lungmuss.postgres
```

Dump the logs to the console:

```bash
podman logs temporal-db
```

run zookeeper

```bash
podman run -d --restart=always --name zookeeper --pod=feuerfest  -e ZOOKEEPER_CLIENT_PORT=2181 -e ZOOKEEPER_TICK_TIME=2000 confluentinc/cp-zookeeper:7.3.2
```

run kafka

```bash
podman run -d --restart=always --name broker --pod=feuerfest  -e KAFKA_BROKER_ID=1 -e KAFKA_ZOOKEEPER_CONNECT='zookeeper:2181' -e KAFKA_LISTENER_SECURITY_PROTOCOL_MAP='PLAINTEXT:PLAINTEXT,PLAINTEXT_INTERNAL:PLAINTEXT' -e KAFKA_ADVERTISED_LISTENERS='PLAINTEXT://localhost:9092,PLAINTEXT_INTERNAL://broker:29092' -e KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR=1 -e KAFKA_TRANSACTION_STATE_LOG_MIN_ISR=1 -e KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR=1 confluentinc/cp-kafka:7.3.2
```

stop the feuerfest pod.

```bash
podman pod stop feuerfest
```

delete the feuerfest pod.

```bash
podman pod rm feuerfest
```

You can also run this script to cycle through the containers and renew all instances. After cycling through, the
databases will be empty.

```bash
./cycle.ps1
```

### Runtime

```bash
docker compose up ./docker-compose.yml
```

# Docker Compose Service Documentation

## Service Name: Postgres

### Description:

PostgreSQL, commonly known as Postgres, is a powerful, open-source object-relational database system with over 30 years
of active development. It's known for its reliability, feature robustness, and performance.

### Docker Image:

**Image Name**: `orion6docker/lungmuss.postgres:latest`

**Source**: [Docker Hub](https://hub.docker.com/r/orion6docker/lungmuss.postgres)

### Container Name:

**Name**: `postgres`

By setting the container name, you can easily identify and interact with the Postgres container using Docker CLI
commands.

### Ports:

- **5432**: This is the default port for PostgreSQL client connections.
    - **Host**: `5432`
    - **Container**: `5432`

### Restart Policy:

- **on-failure**: This means the container will restart only if it exits with a non-zero status (i.e., on failure).

### Environment Variables:

- **POSTGRES_USER**: Specifies the superuser name for the PostgreSQL instance. (Value: `postgres`)

- **POSTGRES_PASSWORD**: Specifies the password for the superuser. This is critical for security, so make sure to keep
  it secure and possibly use more advanced methods like secrets in production. (Value: `test*12*`)

### Healthcheck:

This configuration checks the health of the PostgreSQL container:

- **test**: Executes the command `pg_isready -U postgres` to check if the Postgres service is ready to accept
  connections.
- **interval**: The time duration between two consecutive health checks. (Value: `5s`)
- **timeout**: The time after which the health check times out. (Value: `5s`)
- **retries**: Number of retries before considering the service as unhealthy. (Value: `5`)

### Links and References:

- **Official Website**: [PostgreSQL](https://www.postgresql.org/)
- **GitHub Repository**: PostgreSQL development happens in various repositories. Here's
  the [main one](https://github.com/postgres/postgres).
- **Docker Image Details**: [Docker Hub](https://hub.docker.com/r/orion6docker/lungmuss.postgres)
- **User Guide**: [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- **Community**: [PostgreSQL Mailing Lists](https://www.postgresql.org/community/lists/)

### Configuration & Usage:

To run the Postgres service using the provided configuration, navigate to the directory containing the Docker Compose
file and execute:

Make sure Docker and Docker Compose are installed on your system.

For advanced configurations, environment variables, and more, refer to
the [official documentation](https://www.postgresql.org/docs/).

### Troubleshooting:

If you face any issues with the Postgres service, refer to
the [official troubleshooting guide](https://www.postgresql.org/docs/current/runtime.html) or seek help in the community
forums and mailing lists.

### Notes:

- Ensure the specified port (`5432`) is not occupied by other services on your host.
- Always make sure your PostgreSQL credentials are secured, especially in production environments.
- Regularly check for updates to the Docker image for bug fixes and performance improvements.

## Service Name: Zipkin

### Description:

Zipkin is a distributed tracing system that helps gather timing data needed to troubleshoot latency problems in
microservice architectures. It manages both the collection and lookup of this data.

### Docker Image:

**Image Name**: `openzipkin/zipkin`

**Source**: [Docker Hub](https://hub.docker.com/r/openzipkin/zipkin)

### Ports:

- **9411**: This is the default port on which Zipkin listens. It's exposed to the host system for external access.

    - **Host**: `9411`
    - **Container**: `9411`

  You can access the Zipkin web UI by navigating to `http://localhost:9411` on your host machine.

### Links and References:

- **Official Website**: [Zipkin.io](https://zipkin.io/)
- **GitHub Repository**: [Zipkin GitHub](https://github.com/openzipkin/zipkin)
- **Docker Image Details**: [Docker Hub](https://hub.docker.com/r/openzipkin/zipkin)
- **User Guide**: [Zipkin User Guide](https://zipkin.io/pages/documentation.html)
- **Community**: [Zipkin Gitter](https://gitter.im/openzipkin/zipkin)

### Configuration & Usage:

To run the Zipkin service using the provided configuration, navigate to the directory containing the Docker Compose file
and execute:

## Service Name: Prometheus

### Description:

Prometheus is an open-source monitoring and alerting toolkit originally built at SoundCloud. It is now a standalone open
source project and maintained independently of any company.

### Docker Image:

**Image Name**: `prom/prometheus:latest`

**Source**: [Docker Hub](https://hub.docker.com/r/prom/prometheus)

### Ports:

- **9090**: This is the default port on which Prometheus listens. It's mapped to port `8080` on the host system for
  external access.

    - **Host**: `8080`
    - **Container**: `9090`

  You can access the Prometheus web UI by navigating to `http://localhost:8080` on your host machine.

### Volumes:

- **Host**: `./.prometheus/`
  **Container**: `/etc/prometheus/`

  This volume is used to mount the Prometheus configuration and rule files from the host to the container.

### Command:

Prometheus is started with the following command arguments:

- `--config.file=/etc/prometheus/prometheus.yml`: Specifies the location of the Prometheus configuration file.
- `--storage.tsdb.path=/prometheus`: Defines the directory for time-series data storage.
- `--web.console.libraries=/usr/share/prometheus/console_libraries`: Location of the console libraries.
- `--web.console.templates=/usr/share/prometheus/consoles`: Location of the console templates.

### Restart Policy:

**always**: This policy ensures that the Prometheus container always restarts if it stops. If it's stopped manually,
it'll be restarted only when the container is manually restarted or the docker daemon restarts.

### Links and References:

- **Official Website**: [Prometheus.io](https://prometheus.io/)
- **GitHub Repository**: [Prometheus GitHub](https://github.com/prometheus/prometheus)
- **Docker Image Details**: [Docker Hub](https://hub.docker.com/r/prom/prometheus)
- **User Guide**: [Prometheus User Guide](https://prometheus.io/docs/introduction/overview/)
- **Community**: [Prometheus Community](https://prometheus.io/community/)

## Service Name: Keycloak

### Description:

Keycloak is an open-source Identity and Access Management solution aimed at modern applications and services. It
provides features such as Single-Sign On (SSO), Identity Brokering, and Social Login, User Federation, and more.

### Docker Image:

**Image Name**: `bitnami/keycloak:latest`

**Source**: [Docker Hub](https://hub.docker.com/r/bitnami/keycloak)

### Ports:

- **8080**: This is the default port on which Keycloak listens. It's mapped to port `8180` on the host system for
  external access.

    - **Host**: `8180`
    - **Container**: `8080`

  You can access the Keycloak web UI by navigating to `http://localhost:8180` on your host machine.

### Environment Variables:

- **KEYCLOAK_ADMIN_USER**: Username for the Keycloak admin account. (Value: `admin`)
- **KEYCLOAK_ADMIN_PASSWORD**: Password for the Keycloak admin account. (Value: `test*12*`)
- **KEYCLOAK_IMPORT**: Path to import realm configuration. (Value: `/tmp/realm-export.json`)
- **KEYCLOAK_DATABASE_HOST**: Hostname of the database server. (Value: `postgres`)
- **KEYCLOAK_DATABASE_PORT**: Port of the database server. (Value: `5432`)
- **KEYCLOAK_DATABASE_NAME**: Name of the database. (Value: `keycloak`)
- **KEYCLOAK_DATABASE_USER**: Username for the database connection. (Value: `postgres`)
- **KEYCLOAK_DATABASE_PASSWORD**: Password for the database connection. (Value: `test*12*`)
- **KEYCLOAK_DATABASE_SCHEMA**: Schema for the database. (Value: `public`)

### Dependencies:

- **postgres**: Keycloak depends on the `postgres` service to be healthy before it starts.

### Links and References:

- **Official Website**: [Keycloak.org](https://www.keycloak.org/)
- **GitHub Repository**: [Keycloak GitHub](https://github.com/keycloak/keycloak)
- **Docker Image Details**: [Docker Hub](https://hub.docker.com/r/bitnami/keycloak)
- **User Guide**: [Keycloak User Guide](https://www.keycloak.org/docs/latest/server_admin/index.html)
- **Community**: [Keycloak Community](https://www.keycloak.org/community.html)

### Configuration & Usage:

To run the Keycloak service using the provided configuration, navigate to the directory containing the Docker Compose
file and execute:

Make sure Docker and Docker Compose are installed on your system.

For advanced configurations, environment variables, and more, refer to
the [official documentation](https://www.keycloak.org/docs/latest/).

### Troubleshooting:

If you face any issues with the Keycloak service, refer to
the [official troubleshooting guide](https://www.keycloak.org/docs/latest/server_admin/index.html#troubleshooting) or
raise an issue in the [GitHub repository](https://github.com/keycloak/keycloak/issues).

### Notes:

- Ensure that port `8180` is not being used by any other service on the host machine to avoid port conflicts.
- The environment variables mentioned (especially passwords) should be secured properly in a production environment.
  Using Docker secrets or similar mechanisms is recommended.
- Always refer to the official documentation or Docker Hub page for the latest image updates and configuration options.

## Service Name: Zookeeper

### Description:

Apache ZooKeeper is a centralized service for maintaining configuration information, naming, providing distributed
synchronization, and providing group services. It is especially useful in distributed systems and cloud-native
applications for coordination and management tasks.

### Docker Image:

**Image Name**: `confluentinc/cp-zookeeper:latest`

**Source**: [Docker Hub](https://hub.docker.com/r/confluentinc/cp-zookeeper)

### Container Name:

**Name**: `zookeeper`

By setting the container name, you can easily identify and interact with the Zookeeper container using Docker CLI
commands.

### Environment Variables:

- **ZOOKEEPER_CLIENT_PORT**: This is the port on which Zookeeper will listen for client connections. (Value: `2181`)
- **ZOOKEEPER_TICK_TIME**: This is the basic time unit in Zookeeper, measured in milliseconds, which is used to regulate
  heartbeats, timeouts, etc. (Value: `2000`)

### Links and References:

- **Official Website**: [ZooKeeper Apache](https://zookeeper.apache.org/)
- **GitHub Repository**: [ZooKeeper GitHub](https://github.com/apache/zookeeper)
- **Docker Image Details**: [Docker Hub](https://hub.docker.com/r/confluentinc/cp-zookeeper)
- **User Guide**: [ZooKeeper Documentation](https://zookeeper.apache.org/doc/current/)
- **Community**: [ZooKeeper Mailing Lists](https://zookeeper.apache.org/lists.html)

### Configuration & Usage:

To run the Zookeeper service using the provided configuration, navigate to the directory containing the Docker Compose
file and execute:

Make sure Docker and Docker Compose are installed on your system.

For advanced configurations, environment variables, and more, refer to
the [official documentation](https://zookeeper.apache.org/doc/current/).

### Troubleshooting:

If you face any issues with the Zookeeper service, refer to
the [official troubleshooting guide](https://zookeeper.apache.org/doc/current/zookeeperTroubleshooting.html) or raise an
issue in the [GitHub repository](https://github.com/apache/zookeeper/issues).

### Notes:

- Ensure that the client port (`2181` in this case) is not being used by other services, especially if you are running
  multiple Zookeeper instances or other services on the same host.
- The tick time is an essential configuration parameter, and tweaking it can have implications on the performance and
  behavior of your Zookeeper ensemble.
- Always refer to the official documentation or Docker Hub page for the latest image updates and configuration options.

## Service Name: Kafka

### Description:

Apache Kafka is an open-source stream-processing software platform that functions as a message broker. Developed by
LinkedIn and donated to the Apache Software Foundation, it is particularly used for building real-time data pipelines
and streaming applications.

### Docker Image:

**Image Name**: `confluentinc/cp-kafka:latest`

**Source**: [Docker Hub](https://hub.docker.com/r/confluentinc/cp-kafka)

### Container Name:

**Name**: `kafka`

By setting the container name, you can easily identify and interact with the Kafka container using Docker CLI commands.

### Ports:

- **9092**: This is the primary port for Kafka client connections.
    - **Host**: `9092`
    - **Container**: `9092`

- **29092**: This is an internal communication port, often used for inter-broker communication or specific
  configurations.
    - **Host**: `29092`
    - **Container**: `29092`

### Dependencies:

- **zookeeper**: Kafka relies on Zookeeper for various cluster management functionalities.

### Environment Variables:

- **KAFKA_BROKER_ID**: Unique identifier for the Kafka broker in a cluster. Each broker in a Kafka cluster needs a
  unique ID. (Value: `1`)

- **KAFKA_ZOOKEEPER_CONNECT**: Connection string to Zookeeper. Kafka uses Zookeeper to store metadata about the Kafka
  cluster, as well as topic and consumer group information. (Value: `'zookeeper:2181'`)

- **KAFKA_LISTENER_SECURITY_PROTOCOL_MAP**: Maps listener names to security protocols. This segregates the internal and
  external traffic. (Value: `PLAINTEXT:PLAINTEXT,PLAINTEXT_INTERNAL:PLAINTEXT`)

- **KAFKA_ADVERTISED_LISTENERS**: List of listeners to publish for client connections. These are the addresses the
  client will use to connect. The `PLAINTEXT` prefix denotes unencrypted traffic. (
  Value: `PLAINTEXT://localhost:9092,PLAINTEXT_INTERNAL://kafka:29092`)

- **KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR**: Number of replicas for the offsets topic. As this example seems to be a
  single-node setup, it is set to 1. For a production environment, a higher value is recommended for fault tolerance. (
  Value: `1`)

- **KAFKA_TRANSACTION_STATE_LOG_MIN_ISR**: The minimum number of replicas that a Kafka write must be acknowledged by
  before it's considered successful. (Value: `1`)

- **KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR**: Number of replicas for the transaction topic. Again, as this seems
  to be a single-node setup, it is set to 1. (Value: `1`)

### Links and References:

- **Official Website**: [Kafka Apache](https://kafka.apache.org/)
- **GitHub Repository**: [Kafka GitHub](https://github.com/apache/kafka)
- **Docker Image Details**: [Docker Hub](https://hub.docker.com/r/confluentinc/cp-kafka)
- **User Guide**: [Kafka Documentation](https://kafka.apache.org/documentation/)
- **Community**: [Kafka Mailing Lists](https://kafka.apache.org/contact)

### Configuration & Usage:

To run the Kafka service using the provided configuration, navigate to the directory containing the Docker Compose file
and execute:

Make sure Docker and Docker Compose are installed on your system.

For advanced configurations, environment variables, and more, refer to
the [official documentation](https://kafka.apache.org/documentation/).

### Troubleshooting:

If you face any issues with the Kafka service, refer to
the [official troubleshooting guide](https://kafka.apache.org/documentation/#troubleshooting) or raise an issue in
the [GitHub repository](https://github.com/apache/kafka/issues).

### Notes:

- Ensure the specified ports (`9092` and `29092`) are not occupied by other services on your host.
- Kafka heavily depends on Zookeeper, so always ensure that your Zookeeper instance is healthy and running.
- For production environments, it's crucial to have multiple broker instances and appropriate replication factors for
  topics to ensure fault tolerance.

Test the docker compose file:
docker-compose --env-file C:\customer\lungmuss\refractory\docker\env-file.txt -f C:
\customer\lungmuss\refractory\docker\docker-compose.yml -f .\docker-compose.yaml config

Certainly! Below is a comprehensive Markdown documentation for your README file:

---

# Lungmuss Database Persistence with Docker

This README provides guidance on persisting the Lungmuss database with Docker by utilizing a filesystem junction in Windows. This allows the database to be stored in a directory that's more appropriate for long-term storage or other requirements, while Docker references the database through a conventional path.

## Prerequisites

1. Docker and Docker Compose installed on your machine.
2. The provided `docker-compose.yml` and `up.ps1` script.
3. PowerShell with elevated permissions for creating junctions.

## Creating a Filesystem Junction

A filesystem junction is a type of symbolic link made at the directory level. By leveraging junctions, you can have Docker reference your database at a default path (`c:\customer\lungmuss-database`) while the actual data resides in a different location of your choosing.

To create a filesystem junction:

1. Decide on your target directory where you wish to persist the database. This could be any location on your machine that suits your storage requirements. For example: `F:\lungmuss-dev-database`.

2. Run the following PowerShell command with elevated permissions:

```powershell
New-Item -ItemType Junction -Path c:\customer\lungmuss-database -Target YOUR_CHOSEN_TARGET_PATH
```

Replace `YOUR_CHOSEN_TARGET_PATH` with your chosen directory from step 1.

**Example**:

If you choose `F:\lungmuss-dev-database` as your target directory:

```powershell
New-Item -ItemType Junction -Path c:\customer\lungmuss-database -Target F:\lungmuss-dev-database
```

This will create a junction at `c:\customer\lungmuss-database` pointing to `F:\lungmuss-dev-database`.

## Starting Docker with Database Persistence

To start Docker containers with database persistence enabled:

1. Navigate to the directory `C:\customer\lungmuss\refractory`

2. Run the `up.ps1` script with the `-persistDatabase` flag set to `$true`:

```powershell
.\up.ps1 -persistDatabase $true
```

This will start the Docker containers and map the database storage to the junction path, allowing the data to be persisted in the target directory you've chosen.

## Important Notes

- Ensure that the target directory for your junction (e.g., `F:\lungmuss-dev-database`) exists before creating the junction.
- Avoid modifying or deleting the junction path (`c:\customer\lungmuss-database`) directly when Docker containers are running, as this can lead to unexpected behaviors or data loss.
- Always ensure you have backups of critical data.

---

This documentation provides a clear step-by-step guide on using filesystem junctions with Docker for database persistence.