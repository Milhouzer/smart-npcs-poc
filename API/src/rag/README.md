
docker pull qdrant/qdrant
docker run -p 6333:6333 \
    -v $(pwd)/path/to/data:/qdrant/storage \
    <!-- -v $(pwd)/path/to/snapshots:/qdrant/snapshots \
    -v $(pwd)/path/to/custom_config.yaml:/qdrant/config/production.yaml \ qdrant/qdrant -->

https://help.openai.com/en/articles/6654000-best-practices-for-prompt-engineering-with-the-openai-api