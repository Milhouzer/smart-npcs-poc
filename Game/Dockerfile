FROM ubuntu:20.04

COPY ./Server_Build/ .

EXPOSE 7777

CMD ./game_server -batchmode -nographics -server -ip "192.168.1.100" -port "7777"
