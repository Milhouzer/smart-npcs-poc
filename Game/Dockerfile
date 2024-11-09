# Use a lightweight base for ARM64 (alpine, for instance)
FROM --platform=linux/amd64 ubuntu:20.04 as builder

# Copy server files
COPY --from=builder ./Server_Build/ .

EXPOSE 7777

# Run server on startup
CMD ./game_server -batchmode -nographics -server -ip "192.168.1.100" -port "7777"
