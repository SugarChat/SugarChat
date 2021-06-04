# How to use

## values.yaml中的參數説明
```
signalr:
  type: ClusterIP
  domains: 
    - "localhost"
  image:
    repository: "registry.cn-hangzhou.aliyuncs.com/funshow/sugarchat-signalr-server" # 鏡像地址
    tag: "1.1.0" # 鏡像版本
    pullPolicy: "IfNotPresent" #拉取模式  supported values: "Always", "IfNotPresent", "Never"
  redisConnectionString: "127.0.0.1:6379" # redis緩存連接字符串
  signalRRedisConnectionString: "127.0.0.1:6379" # signalR的Redis底板連接字符串
  SUGARCHAT_SIGNAL_HUB_URL: "http://127.0.0.1" # signalR鏈接域名 用于創建connectionUrl
  ServerClientKey: "key" # 業務端鏈接SignalRServer驗證的Key
  Security: true 是否启用業務端鏈接驗證
```

## 啓動命令
使用helm install命令啓動
```
helm install xxxservername signalr
```
卸載命令
```
helm uninstall xxxservername signalr
```