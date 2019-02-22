#! /bin/bash

SERVICE_NAME_CLIENT=portproxy-client
SERVICE_NAME_SERVER=portproxy-server
SERVICEFILE_PATH=/etc/systemd/system

readArguments(){
	read -p "请输入上级服务器地址：" UPSERVER_ADDR
	read -p "请输入上级服务器端口：" UPSERVER_PORT
	read -p "请输入本地监听端口：" LOCAL_PORT
	read -p "请输入状态服务器监听端口(格式为 IP:端口，IP可以用*号，留空则禁用)：" STAT_SERVER
}

createService(){
	if [ "$INSTALL_TYPE" == 'Client' ]; then
		cmdline="$cmdline" --local
		serviceName=$SERVICE_NAME_CLIENT
	else
		serviceName=$SERVICE_NAME_SERVER
	fi
	cmdline="$cmdline --server=$UPSERVER_ADDR --sport=$UPSERVER_PORT --port=$LOCAL_PORT --stat_server=$STAT_SERVER"

	groupadd portproxy
	useradd -g portproxy portproxy
	chown -R portproxy:portproxy ./

	cat <<EOF >>$SERVICEFILE_PATH/$serviceName.service
[Unit]
Description=PortProxyServer ($INSTALL_TYPE)
After=network.target

[Service]
User=portproxy
Type=simple
PIDFile=/run/$serviceName.pid
ExecStart=/usr/bin/dotnet "$SELF/PortProxy.dll" $cmdline
Restart=on-abort
ExecReload=/bin/kill -s HUP $MAINPID
ExecStop=/bin/kill -s QUIT $MAINPID

[Install]
WantedBy=multi-user.target

EOF

	systemctl daemon-reload
	systemctl enable $serviceName
	systemctl start $serviceName

	echo "PortProxy($INSTALL_TYPE) 已安装。"
}

cat <<EOF
#############################################
#
# PortProxy 部署脚本 by 木鱼
# FOR CENTOS 7.x
#
#############################################


EOF

SELF=$(cd `dirname $0`; pwd)
cd $SELF
echo $SELF

echo 正在检测系统版本……

OS_ID=$(cat /etc/os-release | grep -Po '(?<=^ID=").*(?=")')
OS_NAME=$(cat /etc/os-release | grep -Po '(?<=^NAME=").*(?=")')
OS_VERSION=$(cat /etc/os-release | grep -Po '(?<=^VERSION=").*(?=")')
OS_PRETTYNAME=$(cat /etc/os-release | grep -Po '(?<=^PRETTY_NAME=").*(?=")')
OS_VERSIONID=$(cat /etc/os-release | grep -Po '(?<=^VERSION_ID=").*(?=")')

echo "当前系统：$OS_PRETTYNAME "

if [ "$OS_ID" != 'centos' ] || [ "$OS_VERSIONID" -lt '7' ]; then
	echo -e "\033[5;30;36m此脚本当前只支持CENTOS7及以上系统 :-(\033[0m"
	exit
fi

read -p "请选择操作：1-安装服务端 2-卸载服务端 3-安装客户端 4-卸载客户端 ：[1234] " tmp

if [ "$tmp" -eq '1' ]; then
	INSTALL_TYPE=Server
	readArguments
	createService
fi
if [ "$tmp" -eq '2' ]; then
	if [ -f "$SERVICEFILE_PATH/$SERVICE_NAME_SERVER.service" ]; then
		systemctl stop $SERVICE_NAME_SERVER
		systemctl disable $SERVICE_NAME_SERVER
		rm -rf "$SERVICEFILE_PATH/$SERVICE_NAME_SERVER.service"
		echo -e "\033[5;32m服务端服务已删除\033[0m"
	else
		echo -e "\033[5;31m服务端服务看起来并没有安装\033[0m"
	fi
fi
if [ "$tmp" -eq '3' ]; then
	INSTALL_TYPE=Client
	readArguments
	createService
fi
if [ "$tmp" -eq '4' ]; then
	if [ -f "$SERVICEFILE_PATH/$SERVICE_NAME_CLIENT.service" ]; then
		systemctl disable $SERVICE_NAME_CLIENT
		systemctl stop $SERVICE_NAME_CLIENT
		rm -rf "$SERVICEFILE_PATH/$SERVICE_NAME_CLIENT.service"
		echo -e "\033[5;32m客户端服务已删除\033[0m"
	else
		echo -e "\033[5;31m客户端服务看起来并没有安装\033[0m"
	fi
fi

