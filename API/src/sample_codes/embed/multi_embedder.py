from typing import Any, Literal

from rag_ingestion.chunk.chunker import Chunk
from rag_ingestion.common_config import CommonConfig
from rag_ingestion.embed.embedder import Embedder
from rag_ingestion.writer.writer_factory import init_writer

DEFAULT_BATCH_SIZE = 5


class MultiEmbedder:
    def __init__(
        self,
        provider_parameters: dict[
            Literal["tfidf", "openai", "huggingface"], dict[str, Any]
        ],
        common_config: CommonConfig,
    ) -> None:
        self.provider_parameters = provider_parameters
        self.writer = init_writer(common_config)
        embedder_builders: dict[str, type[Embedder]] = {}
        for provider in provider_parameters.keys():
            if provider == "openai":
                from .azure_openai import AzureEmbedder

                embedder_builders[provider] = AzureEmbedder
            elif provider == "huggingface":
                from .hugging_face import HuggingFaceEmbedder

                embedder_builders[provider] = HuggingFaceEmbedder
            elif provider == "tfidf":
                from .tfidf import TfidfEmbedder

                embedder_builders[provider] = TfidfEmbedder
            else:
                raise ValueError(f"Unknown embedding provider {provider}")
        self.embedders: list[Embedder] = [
            embedder_builders[provider](
                provider, common_config, **self.provider_parameters[provider]
            )
            for provider in provider_parameters.keys()
        ]

    def embed_chunks(self, chunks: list[Chunk]) -> list[Chunk]:
        for embedder in self.embedders:
            embedder.embed_chunks(chunks)

        return chunks

    def write_embedded_chunks(self, chunks: list[Chunk], path: str = None) -> str:
        import pandas as pd

        # Make DF from chunks
        chunks_df = pd.DataFrame([chunk.__dict__ for chunk in chunks])
        if not path:
            path = f"{self.writer.trusted_base_path}/3_embedded/embedded_chunks.pkl"
        directory = "/".join(path.split("/")[:-1])
        self.writer.make_dirs(directory)
        return self.writer.write_pickle(path, chunks_df)
