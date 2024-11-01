# Use the official Python 3.11 image as the base image
FROM python:3.11-slim

# Install Poetry
RUN apt-get update && \
    apt-get install -y curl && \
    curl -sSL https://install.python-poetry.org | python3 - && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Add Poetry to PATH
ENV PATH="/root/.local/bin:$PATH"

# Copy Poetry files for dependency installation
COPY Brechi/pyproject.toml Brechi/poetry.lock ./

# Install dependencies using Poetry
# RUN poetry install --no-root --no-interaction --no-ansi

# Copy the application code into the container
COPY Brechi/ .

# Expose the port on which the app will run
EXPOSE 8000

# Command to run the FastAPI application using Uvicorn
CMD ["poetry", "run", "uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
