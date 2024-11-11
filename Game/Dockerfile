FROM --platform=linux/amd64 ubuntu:20.04

COPY ./Server_Build/ .

EXPOSE 7777

ENV SERVER_IP="0.0.0.0"
ENV SERVER_PORT="7777"

CMD ./StandaloneLinux64/game_server -batchmode -nographics -server -ip "$SERVER_IP" -port "$SERVER_PORT"

