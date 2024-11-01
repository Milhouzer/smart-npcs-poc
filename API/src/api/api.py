import os
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
from fastapi.responses import JSONResponse

# from .llm.base import BotAnswer, get_bot_answer, PlayerQuery

# Define the app instance
app = FastAPI()


@app.get("/test")
async def test_api_connexion() -> JSONResponse:
    return JSONResponse(status_code=200, content=" - ".join(os.getcwd(), os.listdir()))


# # CRUD Endpoints
# @app.post("/talk-to-bot")
# async def talk_to_bot(player_query: PlayerQuery) -> JSONResponse:

#     bot_answer = "bot answer"  # get_bot_answer(player_query)

#     return JSONResponse(content=bot_answer)
