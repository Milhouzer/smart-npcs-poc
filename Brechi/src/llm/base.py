from langchain.output_parsers import PydanticOutputParser
from langchain_core.prompts import PromptTemplate
from langchain_core.pydantic_v1 import BaseModel, Field, validator
from pydantic import BaseModel, Field, model_validator


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


def get_llm_chain():
    parser = get_parser_output()
    llm = get_llm()
    prompt_template = get_prompt_template()

    chain = prompt_template | llm | parser

    return chain


if __name__ == "__main__":
    chain = get_llm_chain()

    ex_payload = {
        "robot_context": {"player_id": 1},
        "environment_context": {"whether": "sunny", "temperature": "22 degrés"},
    }

    print("Generated Output:")
    chain.invoke({"input_text": ex_payload})
