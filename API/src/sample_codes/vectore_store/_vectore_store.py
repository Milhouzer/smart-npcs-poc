from typing import Literal

from ..chunk.chunker import Chunk
from ..common_config import CommonConfig
from ..writer.writer import OSReadWriter
from ..writer.writer_factory import init_writer


class VectorStore:
    def __init__(self, index_name: str, common_config: CommonConfig, **kwargs):
        self.index_name: str = index_name
        self.common_config: CommonConfig = common_config
        self.writer: OSReadWriter = init_writer(common_config)
        self.kwargs = kwargs

    def create_index(self, reference_chunk: Chunk) -> None:
        raise NotImplementedError(
            "This method should be implemented in the child class"
        )

    def upload_chunks(self, chunks: list[Chunk]) -> None:
        # TODO: arbitrate if we should parallelize upload of chunks
        raise NotImplementedError(
            "This method should be implemented in the child class"
        )


def init_vector_store(
    provider: Literal["qdrant", "azure"],
    index_name: str,
    common_config: CommonConfig,
    **kwargs,
) -> VectorStore:
    if provider == "qdrant":
        from .qdrant import QdrantStore

        return QdrantStore(index_name, common_config, **kwargs)
    else:
        raise ValueError(f"Unknown vector store provider {common_config}")
