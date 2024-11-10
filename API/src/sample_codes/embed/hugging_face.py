import logging
from typing import Literal

import numpy as np
import numpy.typing as npt
from sentence_transformers import SentenceTransformer

from ..common_config import CommonConfig
from ..embed.embedder import Embedder


class HuggingFaceEmbedder(Embedder):
    def __init__(
        self,
        embedding_provider: Literal["huggingface"],
        common_config: CommonConfig,
        **kwargs,
    ) -> None:
        super().__init__(embedding_provider, common_config, **kwargs)
        if "model_name_or_path" not in kwargs:
            raise ValueError("model_name_or_path is required for HuggingFaceEmbedder")
        self.model_name_or_path: str = kwargs["model_name_or_path"]
        logging.info(f"Downloading HuggingFace model {self.model_name_or_path}")
        self.model = SentenceTransformer(
            model_name_or_path=self.model_name_or_path, trust_remote_code=True
        )

    def _embed(self, text: str) -> list[float]:
        embeddings: npt.NDArray[np.float64] = self.model.encode(text)  # type: ignore
        return embeddings.tolist()
