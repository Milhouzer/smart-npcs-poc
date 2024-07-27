from transformers import pipeline
from langchain.llms import HuggingFacePipeline


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
