from pydantic import BaseModel, Field, model_validator
from langchain.output_parsers import PydanticOutputParser
from src.llm.base import get_llm_chain


class PlayerQuery(BaseModel):
    id_bot: int
    id_player: int
    player_query: str
    additionnal_argument: dict  # TODO : to define


# Data model using Pydantic
class BotAnswer(BaseModel):
    id_bot: int
    id_player: int
    message: str = Field(description="Response message of the bot")
    actions: list


def get_parser_output():
    # Set up a parser + inject instructions into the prompt template.
    return PydanticOutputParser(pydantic_object=BotAnswer)


def get_bot_answer(player_query: PlayerQuery) -> BotAnswer:

    chain = get_llm_chain()

    chain.invoke()

    pass
