from langchain.output_parsers import PydanticOutputParser
from langchain_core.prompts import PromptTemplate
from langchain_core.pydantic_v1 import BaseModel, Field, validator
from src.llm.response import get_parser_output, get_llm, get_prompt_template


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
