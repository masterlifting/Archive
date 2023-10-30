
```bash
dapr run --app-id whoami --dapr-http-port 3500 --app-port 41045 --app-protocol http
```

```bash
curl -X GET http://localhost:3500/v1.0/invoke/whoami/method/
```


```bash
dapr run --app-id whoami --dapr-http-port 3500 --app-port 41045 --app-protocol http --resources-path .\dapr\v3\components\ --config .\dapr\v3\config.yaml --enable-api-logging  --log-level debug
```
