# Microservice Deployment Documentation

## Required Information

### Application Name
- **Application Name**: Replace this text with the name of the application.

### Container Information
- **Container Name**: Replace this text with the name of the container.
    - **Instructions**: To create a container, [Click here](Replace_with_link_to_instructions).

### Required Environment Variables
- List of required environment variables to set before running this container:
    - `ENV_VAR_1`: Description of ENV_VAR_1.
    - `ENV_VAR_2`: Description of ENV_VAR_2.
    - `...`: Add more as needed.

### Ingress Requirements
- **Needs Ingress**: Yes/No
    - **Publicly Accessible**: Should the application be accessible from outside the cluster? (Yes/No)

### Health Checks
- **Readiness Probe URL**: `/path/to/readiness/probe`
- **Liveness Probe URL**: `/path/to/liveness/probe`

## Optional Information

### Replicas
- **Multiple Replicas**: Is it okay to run more than 1 replica/instance? (Yes/No)

### Default Deployment Tag
- **Default Tag**: Replace with default tag or version to be deployed. (Default: `latest`)

### External Authentication
- **OAuth or Similar Required**: Yes/No
    - If Yes, specify details and/or link to documentation.

### Optional Environment Variables
- **Documentation**: [Link to documentation of optional environment variables](Replace_with_link_to_documentation)

### Dapr Configuration
- **Needs Dapr Config**: Yes/No
    - If Yes, specify config details or link to documentation.

### Database Requirements
- **Needs Database**: Yes/No
    - **Type**: PostgreSQL/MySQL/etc.
    - **Configuration**: Link to configuration documentation or inline details.

### Resource Limits
- **CPU**: for example '1'
- **Memory**: for example '1024Mi'

### Pull Secrets
- **Needs Pull Secret**: Yes/No
    - If Yes, provide the name of the pull secret or link to documentation on how to create one.
