import asyncio
from typing import Literal

from tqdm import tqdm

from rag_ingestion.chunk.chunker import Chunk
from rag_ingestion.common_config import CommonConfig
from rag_ingestion.utils import batch_generator, run_async_gathering
from rag_ingestion.writer.writer import OSReadWriter
from rag_ingestion.writer.writer_factory import init_writer

DEFAULT_BATCH_SIZE = 5
PROVIDER_TO_EMBEDDING_TYPE = {
    "tfidf": "sparse_embeddings",
    "openai": "dense_embeddings",
    "huggingface": "dense_embeddings",
}


class Embedder:
    def __init__(
        self,
        embedding_provider: Literal["tfidf", "openai", "huggingface"],
        common_config: CommonConfig,
        **kwargs,
    ) -> None:
        self.provider: Literal["tfidf", "openai", "huggingface"] = embedding_provider
        self.common_config: CommonConfig = common_config
        self.writer: OSReadWriter = init_writer(common_config)
        self.kwargs = kwargs

    def embed_chunks(self, chunks: list[Chunk]) -> list[Chunk]:
        batch_size = DEFAULT_BATCH_SIZE
        embedded_chunks: list[Chunk] = []
        for chunk_batch in tqdm(
            batch_generator(chunks, batch_size=batch_size),
            desc=f"Embedding chunk using {self.provider}, batches of ({batch_size}).",
            total=len(chunks) // batch_size + 1,
        ):
            parallel_queries = [self.embed_chunk(chunk) for chunk in chunk_batch]
            batch_embedded_chunks = asyncio.run(run_async_gathering(parallel_queries))
            embedded_chunks.extend(batch_embedded_chunks)
        return embedded_chunks

    async def embed_chunk(self, chunk: Chunk) -> Chunk:
        chunk.__getattribute__(PROVIDER_TO_EMBEDDING_TYPE[self.provider])[
            self.provider
        ] = self._embed(chunk.text)
        return chunk

    def _embed(self, text: str) -> list[float]:
        raise NotImplementedError(
            "This method should be implemented in the child class"
        )
