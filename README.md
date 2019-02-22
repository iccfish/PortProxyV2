# PortProxyV2

> 这是 [PortProxy](https://github.com/iccfish/PortProxy) 的第二代版本。

## 这是干嘛的？

> 不能多言，言多必死，死后鞭尸。

需要服务器端和本地均部署，然后通过混淆的算法将远程服务器上的指定端口（本地或远程）映射到本地，防止协议被识别。

所以，就是个端口映射吼！

举个栗子，你在服务器上有一个 **Redis** 服务器只能内网访问，现在想要在家里也可以访问，那么可以通过这个工具将远程的 Redis 端口映射到本地，然后直接访问本地的端口就阔以了。

但是呢，在公网上的流量是看不粗这是个Redis服务器滴。

## 核心功能

- 多平台支持（Windows/Linux/Mac）
- 支持以 Windows Service 模式运行
- 支持简单的状态统计页面

> 协议混淆不使用任何已知的公开的加密算法

## 开发配置

- 基于 `.NET CORE 2.0`
- 开发工具 `Visual Studio 2019 Community` OR `JetBrains Rider`
- 开发平台 `Windows 10`

## 授权

`GPLv3`

## 部署

先部署服务器端，再部署客户端。

### 服务器端

根据服务器平台，选择对应的版本。

#### Windows

直接运行 `PortProxy.WinService.exe`，在出现的界面上选择对应选项即可。

#### CentOS

直接运行 `setup_centos.sh` 即可。

#### 其它Linux版本或MAC或手动安装

使用命令行运行。

命令：

`dotnet PortProxy.dll <参数>`

支持的参数：

- `--server=<地址>` 上游服务器IP或域名
- `--sport=<端口>` 上游服务器端口
- `--port=<端口>` 本地监听端口
- `--stat_server=<URL前缀>` 本地状态服务器前缀，格式为 `<IP或域名>:端口`，只支持HTTP协议，留空禁用
- `--console` 控制台模式运行，日志信息将会输出到控制台

剩下的就是怎么后台运行的事情了~~

### 客户端

根据客户端平台，选择对应的版本。

#### Windows

直接运行 `PortProxy.WinService.exe`，在出现的界面上选择对应选项即可。

#### CentOS

直接运行 `setup_centos.sh` 即可。

#### 其它Linux版本或MAC或手动安装

使用命令行运行。

命令：

`dotnet PortProxy.dll <参数>`

支持的参数和**服务器模式**一样，需要额外加 `--local` 参数

剩下的就是怎么后台运行的事情了~~

### 提示

> 请先启动服务器端，然后在服务器端的 `config` 目录下会自动生成一个 `seed` 文件。客户端和服务器端需要保持这个文件一致才能正常访问。

> 客户端时间和服务器端时间相差不能超过10秒，否则服务器会拒绝连接

> 状态页面地址为 `http://<URL前缀>/stat.html`，需要用户名密码，在 `config` 下的 `httpConfig.json` 中设置，默认为 `iFish` 和 `123456` ，请务必修改

> 装有防火墙的话，注意放行相关端口哦

## 开发

> 先自己玩吧。