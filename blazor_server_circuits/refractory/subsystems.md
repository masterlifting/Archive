| Name                                                                                   | DaprAppId | Prefix | DbName     | SchemaId |
|----------------------------------------------------------------------------------------|-----------|--------|------------|----------|
| [contacthubexpress](https://github.com/lungmuss/contacthubexpress)                     | -         | -      | -          | 25       |
| [container](https://github.com/lungmuss/container)                                     | container | CNT    | container  | 17       |
| [dashboard](https://github.com/lungmuss/dashboard)                                     | -         | -      | -          | 23       |
| [devops](https://github.com/lungmuss/devops)                                           | -         | -      | -          | -        |
| [feuerfest.deploy](https://github.com/lungmuss/feuerfest.deploy)                       | -         | -      | -          | -        |
| [feuerfest.devcontainer](https://github.com/lungmuss/feuerfest.devcontainer)           | -         | -      | -          | -        |
| [laboratory](https://github.com/lungmuss/laboratory)                                   | lab       | LAB    | laboratory | 24       |
| [lungmuss.agent.host](https://github.com/lungmuss/lungmuss.agent.host)                 | -         | -      | -          | -        |
| [lungmuss.backend.blob](https://github.com/lungmuss/lungmuss.backend.blob)             | blob      | BLB    | blob       | 14       |
| [lungmuss.backend.cns](https://github.com/lungmuss/lungmuss.backend.cns)               | cns       | CNS    | cns        | 13       |
| [lungmuss.backend.latex](https://github.com/lungmuss/lungmuss.backend.latex)           | -         | -      | -          | -        |
| [lungmuss.backend.pdf](https://github.com/lungmuss/lungmuss.backend.pdf)               | -         | -      | -          | -        |
| [lungmuss.backend.purchase](https://github.com/lungmuss/lungmuss.backend.purchase)     | -         | -      | -          | -        |
| [lungmuss.devops.test](https://github.com/lungmuss/lungmuss.devops.test)               | -         | -      | -          | -        |
| [lungmuss.frontend.nop](https://github.com/lungmuss/lungmuss.frontend.nop)             | -         | -      | -          | -        |
| [lungmuss.latex.templates](https://github.com/lungmuss/lungmuss.latex.templates)       | -         | -      | -          | -        |
| [lungmuss.postgres](https://github.com/lungmuss/lungmuss.postgres)                     | -         | -      | -          | -        |
| [lungmuss.refractory.library](https://github.com/lungmuss/lungmuss.refractory.library) | -         | -      | -          | -        |
| [mms](https://github.com/lungmuss/mms)                                                 | mms       | MMS    | mms        | 16       |
| [nop](https://github.com/lungmuss/nop)                                                 | -         | -      | -          | -        |
| [qrcode](https://github.com/lungmuss/qrcode)                                           | -         | -      | -          | 22       |
| [refractory-legacy](https://github.com/lungmuss/refractory-legacy)                     | -         | -      | -          | -        |
| [refractory](https://github.com/lungmuss/refractory)                                   | -         | -      | -          | -        |
| [refractorytestsuite](https://github.com/lungmuss/refractorytestsuite)                 | -         | -      | -          | -        |
| [rnd](https://github.com/lungmuss/rnd)                                                 | rnd       | RND    | rnd        | 15       |
| [sagedatabridge](https://github.com/lungmuss/sagedatabridge)                           | sdb       | SDB    | sdb        | 20       |
| [scanner](https://github.com/lungmuss/scanner)                                         | scanner   | SCN    | scanner    | 19       |
| [scannerdisplay](https://github.com/lungmuss/scannerdisplay)                           | scd       | SCD    | scd        | 21       |
| [warehouse](https://github.com/lungmuss/warehouse)                                     | warehouse | WRH    | warehouse  | 18       |

## Column description

| Column    | Description                                           |
|-----------|-------------------------------------------------------|
| App id    | The Dapr app id of the application.                   |
| Prefix    | The prefix of error code in this sub-system.          |
| Schema id | The id of the database schema used for advisory lock. |
| Port      | The port of the microservice.                         |
| Subsystem | The name of the subsystem.                            |

### App ID

Dapr App ID is a unique identifier given to each Dapr application at runtime. It is a vital component in the Dapr system because it determines the overall behavior of the application and its interaction with other applications and Dapr runtime.

#### Purpose of Dapr App ID

1. **Service Discovery:** The Dapr App ID helps in service discovery within the Dapr runtime environment. It allows other Dapr applications and services to discover and communicate with one another, using the App ID as an identifier.

2. **State Store:** When you use Dapr state management, the state store uses the Dapr App ID to partition and manage the state for different applications.

3. **Pub/Sub Messaging:** In Dapr pub/sub system, the App ID becomes the name of the subscriber when subscribing to topics.

#### How to Configure Dapr App ID

While starting a Dapr application, you pass the App ID as a command-line argument, for example: `dapr run --app-id myapp -- ...`

In the example above, 'myapp' is the App ID. The App ID must be unique across the Dapr applications running in the same environment.

Remember, Dapr App ID is case-sensitive, and it should match any references to the app in other components accurately. For instance, if you're utilizing the pub/sub component, the casing for the app ID should be the same in the component's metadata and the actual usage.

#### Best Practices for Dapr App ID

Here are some best practices while defining Dapr App IDs:

- Keep it unique across your Dapr environment to avoid any routing and state management clash.
- Use a naming convention that represents the functionality of your application.
- Maintain the same case as identifiers are case-specific.

In summary, the Dapr App ID is a crucial part of the Dapr environment that is used in service discovery, state store partitioning, and pub/sub messaging, among other things.

#### Prefix

When an exception is thrown or a failure code is returned, we use a standard layout for the failure code: `<prefix>-00000`.

The prefix is from the table in column prefix. The number is a 5 digit number, the error code. The error code is unique within the subsystem.

#### Schema ID

The schema id is a unique identifier used to acquire an advisory lock in the database cluster.
Schema updates a part of the docker image start up.
When clusters are deployed in a replication set, hence there can be more than one instance of the same microservice running at the same time.
To prevent schema updates from running multiple times, or even worse, concurrently, we use the advisory lock.
