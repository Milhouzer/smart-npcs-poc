# robot_def : explain

robot_context = """
You are an assistance robot link to player id : {player_id} and from now on will be refered to as 'your pet'.
You will follow him in his adventure and answer all his question.
You will use contextual information about your environment, the whether, information relative to your pet
...
"""

environment_context = """
whether : {whether}
temperature : {temperature}
"""

# TODO : Add formating
base_prompt = """ 
{robot_context}
Environment info: 
{environment_context}
"""


def format_prompt_from_payload(payload_dct: dict) -> str:

    payload_dct

    return base_prompt.format(
        **{
            sub_str: globals().get(sub_str).format(**sub_dct)
            for sub_str, sub_dct in payload_dct.items()
        }
    )


if __name__ == "__main__":

    ex_payload = {
        "robot_context": {"player_id": 1},
        "environment_context": {"whether": "sunny", "temperature": "22 degrés"},
    }

    print("*" * 30, "\n", format_prompt_from_payload(ex_payload))
