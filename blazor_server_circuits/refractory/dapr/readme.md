# Dapr - Distributed Application Runtime

[Dapr](https://dapr.io/) (Distributed Application Runtime) is a vital tool in modern software development due to its outstanding features and capabilities.
**Portability** is one of the key strengths of Dapr as it can be used in any language and on any platform, making it an incredible partner in the development of microservices.

Dapr offers a wide array of **building blocks** including service-to-service invocation, state management, publish/subscribe messaging, and event-driven resource bindings.
These building blocks allow developers to focus on writing business logic instead of infrastructure code.

Moreover, Dapr provides **powerful security features** including mTLS for secure communication between services, secret management, and more, ensuring that our applications are always secure.

One of the defining features of Dapr is its ability to simplify **distributed systems capabilities**. 
It’s not merely a framework but a complete runtime which sits alongside your application to provide powerful distributed systems capabilities without the need for you, the developer, to become a distributed systems expert.

Finally, the way Dapr is designed makes it **agnostic to the underlying infrastructure**.
Whether you’re running your application in the cloud, on-premises, or on an edge device, Dapr works the same way.

Clearly, using Dapr greatly simplifies the process of building, deploying, and securing microservices.
Its flexibility, wide array of developer-centric features, and strong security make it a great choice in the world of software development.

## Dapr in the Lungmuß Refractory Project

In the Lungmuß Refractory Project, we use Dapr as the main runtime for our microservices.
Dapr is used to simplify the development of our microservices and to provide powerful distributed systems capabilities.
It is also used to secure our microservices and to make them portable across different platforms.

## Dapr in Docker Compose

In order to minimize friction, we provide large docker compose files per project.
These files contain all the necessary services to run the project locally with all services available.
A developer should be up and running in no-time.

The service name, container name, database and, if needed, port numbers adhere all to the standards defined in the [subsystems](../subsystems.md) document.

For a service to run successfully, it needs to be configured to use Dapr.
This configuration is done in the `docker-compose.yml` file.
It consists of two parts: the service itself and the Dapr sidecar.

In the excerpt below we use the `blob` service as an example.

```yaml
  blob:
    image: ghcr.io/lungmuss/lungmuss.backend.blob:latest
    container_name: blob
    depends_on:
      postgres:
        condition: service_healthy
    environment:
      - PGHOST=postgres
      - PGPORT=5432
      - PGDATABASE=blob
      - PGUSER=postgres
      - PGPASSWORD=test*12*
      - OIDC_VALID_ISSUER=https://${keykloak}/realms/feuerfest
      - OIDC_VALID_AUDIENCE=account
      - OIDC_AUTHORITY=https://${keykloak}/realms/feuerfest
    restart: on-failure
```

```yaml
  blob-dapr:
    image: "daprio/daprd:latest"
    command: [ "./daprd",
               "--app-id", "blob",
               "--app-port", "80",
               "--components-path","/components",
               "--placement-host-address", "dapr-placement:50005",
               "--config", "/config/config.yaml"
    ]
    volumes:
      - "./.dapr/components/:/components"
      - "./.dapr/config/:/config"
    depends_on:
      - blob
    restart: on-failure
    network_mode: "service:blob"
```

The first part is the service itself.
It is configured as a normal service in docker compose.
The only difference is that it is configured to use the `postgres` service as a dependency.
This is because the `blob` service needs to connect to the `postgres` service to function properly.

The second part is the Dapr sidecar.
This is a separate container that runs alongside the service container.
It is configured to use the `daprd` image from the Dapr team.
The `command` section configures the Dapr sidecar to use the `blob` service.
The `--app-id` parameter is the name of the service.
The `--app-port` parameter is the port on which the service is running.
The `--components-path` parameter is the path to the Dapr components.
The `--placement-host-address` parameter is the address of the Dapr placement service.
The `--config` parameter is the path to the Dapr configuration file.

The `volumes` section configures the volumes that are mounted in the Dapr sidecar.

The `depends_on` section configures the Dapr sidecar to start after the service container.

The `restart` section configures the Dapr sidecar to restart when it fails.

The `network_mode: "service:blob"` line in the Docker Compose definition for the Dapr sidecar settings is particularly crucial.
This setting enables the Dapr sidecar to share the network stack of the specified service (`blob` in this case).
This is commonly known as sharing the network namespace which eliminates the overhead of network communication between the main service and its sidecar as they don't need to traverse the Docker network layers, effectively side-stepping any network latency that would otherwise occur.

Dapr sidecars are served in separate containers and they need to communicate with the primary application containers (in this case, `blob` application).
This is done, for instance, through local loopback where source and destination both reside in the same network namespace, i.e., they're on `localhost`.
This frequent communication necessitates efficient network routing and latency reduction; both of which are addressed by using service's network mode (`network_mode: "service:blob"`).
In terms of implementation, when Docker Compose starts up the `blob-dapr` service, instead of creating a new network namespace, Docker reuses the network namespace from the blob service.
This effectively causes the `blob` and `blob-dapr` services to behave as if they are on the same logical machine.

One key thing to note is the order of service startup.
The blob service must be started prior to the blob-dapr service.
This order has been ensured with the `depends_on` clause (`depends_on: - blob`), making certain that the blob service's network is established and ready to be shared with the blob-dapr service prior to its instantiation.

Using this network mode, the inter-service communication within the application becomes a local affair, thus minimizing potential network bottlenecks and optimizing communication between the service and its Dapr sidecar.