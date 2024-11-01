from langchain import PromptTemplate
from .base import get_parser_output

# robot_def : explain

robot_context_template = """
You are an assistance robot link to player id : {player_id} and from now on will be refered to as 'your pet'.
You will follow him in his adventure and answer all his question.
You will use contextual information about your environment, the weather, information relative to your pet
...
"""

environment_context_template = """
weather : {weather}
temperature : {temperature}
"""

# TODO : Add formating
base_prompt_template = """ 
{robot_context_template}
Environment info: 
{environment_context_template}
"""


def format_section(template: str, context: dict) -> str:
    """Helper function to format each section with the given context."""
    return template.format(**context)


def format_prompt_from_payload(payload: dict) -> str:
    """Formats the full prompt based on the payload."""
    # Build each section separately
    robot_context = format_section(
        robot_context_template, payload.get("robot_context", {})
    )
    environment_context = format_section(
        environment_context_template, payload.get("environment_context", {})
    )

    # Combine sections into the base prompt
    return base_prompt_template.format(
        robot_context=robot_context, environment_context=environment_context
    )


def get_prompt_template():
    parser = get_parser_output()

    # Define a prompt template
    prompt_template = PromptTemplate(
        input_variables=["robot_context", "environment_context"],
        template=base_prompt_template,
        partial_variables={"format_instructions": parser.get_format_instructions()},
    )

    return prompt_template


if __name__ == "__main__":

    ex_payload = {
        "robot_context": {"player_id": 1},
        "environment_context": {"weather": "sunny", "temperature": "22 degrés"},
    }
    # prompt_template = get_prompt_template()

    # prompt_template.invoke(ex_payload)

    # print("*" * 30, "\n", format_prompt_from_payload(ex_payload))
