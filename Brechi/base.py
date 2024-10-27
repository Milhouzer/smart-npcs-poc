from langchain.output_parsers import PydanticOutputParser
from langchain_core.prompts import PromptTemplate
from langchain_core.pydantic_v1 import BaseModel, Field, validator
from langchain_openai import ChatOpenAI


# Define your desired data structure.
class ExtractedReferences(BaseModel):
    ESRS_paragraph: list = Field(
        description="List of paragraph numbers mentionned in the input."
    )
    ESRS_section: list = Field(
        description="List of ESRS sections mentionned in the input."
    )
    Directive: list = Field(
        description="List of Directives or Directive articles mentionned in the input."
    )


# Set up a parser + inject instructions into the prompt template.
parser = PydanticOutputParser(pydantic_object=ExtractedReferences)

prompt = PromptTemplate(
    template=prompt_template,
    input_variables=["query"],
    partial_variables={"format_instructions": parser.get_format_instructions()},
)

chain = prompt | llm | parser

chain.invoke({"query": test_content})
