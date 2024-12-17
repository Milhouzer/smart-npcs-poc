from langchain.output_parsers import PydanticOutputParser
from langchain_core.prompts import PromptTemplate
from pydantic import BaseModel, Field
from transformers import pipeline
from langchain.llms import HuggingFacePipeline
from src.llm.base_prompt import base_prompt_template


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


def get_pipeline():
    # Load the Hugging Face model
    model_name = "meta-llama/LLaMA-3.2-1B"  # Replace with your desired model
    hf_pipeline = pipeline("text-generation", model=model_name)
    return hf_pipeline


def get_llm():
    # Create an instance of HuggingFacePipeline
    llm = HuggingFacePipeline(
        pipeline=get_pipeline(),
        pipeline_kwargs={"max_new_tokens": 100},
    )
    return llm


def get_parser_output():
    # Set up a parser + inject instructions into the prompt template.
    return PydanticOutputParser(pydantic_object=BotAnswer)


def get_bot_answer(player_query: PlayerQuery) -> BotAnswer:

    chain = get_llm_chain()

    chain.invoke()

    pass


def get_prompt_template():
    parser = get_parser_output()

    # Define a prompt template
    prompt_template = PromptTemplate(
        input_variables=["robot_context", "environment_context"],
        template=base_prompt_template,
        partial_variables={"format_instructions": parser.get_format_instructions()},
    )

    return prompt_template


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
