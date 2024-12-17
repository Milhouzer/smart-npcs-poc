import string
from typing import Literal

from nltk.corpus import stopwords
from nltk.stem.snowball import SnowballStemmer
from nltk.tokenize import word_tokenize
from sklearn.feature_extraction.text import TfidfVectorizer

from rag_ingestion.common_config import CommonConfig
from rag_ingestion.embed.embedder import Embedder

DEFAULT_LANGUAGE = "english"


class TfidfEmbedder(Embedder):
    def __init__(
        self,
        embedding_provider: Literal["tfidf"],
        common_config: CommonConfig,
        **kwargs
    ) -> None:
        super().__init__(embedding_provider, common_config, **kwargs)
        self.language: str = kwargs.get("language", DEFAULT_LANGUAGE)
        self.tfidf = self._get_tfidf()

    def _embed(self, text: str) -> list[float]:
        return self.tfidf.fit_transform([text]).asfptype()

    def _get_tfidf(self) -> TfidfVectorizer:
        return TfidfVectorizer(tokenizer=self._get_tokenizer, norm=None)

    def _get_tokenizer(self, corpus: str) -> list[str]:
        language = self.language
        text = word_tokenize(corpus, language=language)
        stemmer = SnowballStemmer(language)
        stop_words = stopwords.words(language) + list(set(string.punctuation))
        return [stemmer.stem(token) for token in text if token not in stop_words]
