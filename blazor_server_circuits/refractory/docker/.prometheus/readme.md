# Prometheus Configuration Guide

The structure of this configuration is yaml based. At the top level, it is defined by the `global:` block. Inside this default settings for various parameters are defined. The `scrape_interval` set at the global level defines the default frequency at which Prometheus scrapes the metrics data from different targets. Here, it is set to 15 seconds (`15s`).

For each individual target, we define a `scrape_config`. Each `scrape_config` includes the `job_name`, `scrape_interval`, and `static_configs`:

- The `job_name` indicates the name of a collection of instances that have the same purpose, for example, 'dapr'.

- The `scrape_interval` for specific jobs can be set to override the global `scrape_interval`. In the given example, Prometheus is set to scrape the metrics for the 'dapr' and 'warehouse' jobs every 5 seconds.

- `static_configs` allows providing a static list of targets for Prometheus to scrape. The `targets` option is an array consisting of strings in the form 'host:port', such as 'localhost:9090' in the 'dapr' job and 'host.docker.internal:5118' in the 'warehouse' job.