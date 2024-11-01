import logging

from qdrant_client import QdrantClient, models
from qdrant_client.models import Distance, PointStruct, SparseVector

from ..chunk.chunker import Chunk
from .vector_store import VectorStore


class QdrantStore(VectorStore):
    def create_index(self, reference_chunk: Chunk) -> None:
        client = get_qdrant_client(**self.kwargs)

        if client.collection_exists(self.index_name):
            logging.info(f"Index {self.index_name} already exists, skipping creation")
            return None
        dense_vectors_config = _get_dense_config_from_chunk(reference_chunk)
        sparse_vectors_config = _get_sparse_config_from_chunk(reference_chunk)
        client.create_collection(
            self.index_name,
            vectors_config=dense_vectors_config,
            sparse_vectors_config=sparse_vectors_config,
        )

    def upload_chunks(self, chunks: list[Chunk]) -> None:
        client = get_qdrant_client(**self.kwargs)
        client.upsert(
            self.index_name,
            points=[
                PointStruct(
                    id=chunk.id,
                    vector={
                        **_chunk_to_dense_vector(chunk),
                        **_chunk_to_sparse_vector(chunk),
                    },
                    payload={**chunk.get_metadata(), **{"text": chunk.text}},
                )
                for chunk in chunks
            ],  # type: ignore
        )
        client.close()


def _chunk_to_dense_vector(chunk: Chunk) -> dict[str, list[float]]:
    return chunk.dense_embeddings


def _chunk_to_sparse_vector(chunk: Chunk) -> dict[str, models.SparseVector]:
    return {
        provider: SparseVector(
            indices=sparse_embedding.indices.tolist(),
            values=sparse_embedding.data.tolist(),
        )
        for provider, sparse_embedding in chunk.sparse_embeddings.items()
    }


def _get_sparse_config_from_chunk(chunk: Chunk) -> dict[str, models.SparseVectorParams]:
    return {
        provider: models.SparseVectorParams(modifier=models.Modifier.IDF)
        for provider in chunk.sparse_embeddings.keys()
    }


def _get_dense_config_from_chunk(chunk: Chunk) -> dict[str, models.VectorParams]:
    return {
        provider: models.VectorParams(
            size=len(chunk.dense_embeddings[provider]), distance=Distance.COSINE
        )
        for provider in chunk.dense_embeddings.keys()
    }


def get_qdrant_client(**kwargs) -> QdrantClient:
    return QdrantClient(**kwargs)
