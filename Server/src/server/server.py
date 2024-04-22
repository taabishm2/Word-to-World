import uuid
import json
import openai

from langchain_openai import ChatOpenAI
from flask import Flask, jsonify, request
from langsmith.wrappers import wrap_openai
from langchain_openai import OpenAIEmbeddings
from langchain_openai import OpenAIEmbeddings

from src.llm.chat import Chat, get_prompt, gpt_3_5_turbo
from src.llm.rag import create_asset_db

# Used to save chats for continued conversations
# ID of the chat is returned in response
CHAT_HISTORY = dict()

LLM_MODEL = ChatOpenAI(model="gpt-4", temperature=0)
EMBEDDING_MODEL = OpenAIEmbeddings()
VECTOR_DB_PATH = "./db"

app = Flask(__name__)
client = wrap_openai(openai.Client())

# Override sqlite with existing
# __import__('pysqlite3')
# import sys
# sys.modules['sqlite3'] = sys.modules.pop('pysqlite3')


def start_chat():
    chat_id = str(uuid.uuid4())

    val = dict()
    chat_obj = Chat(get_prompt("design", "sys_msg", val))

    CHAT_HISTORY[chat_id] = chat_obj

    return chat_id, chat_obj


# Test route to check if server runs
@app.route("/", methods=["GET"])
def home():
    return jsonify({"message": "Server is running"}), 200


# Create a new scene from scratch
@app.route("/create-scene", methods=["POST"])
def generate_initial_scene():
    # Create the asset database
    create_asset_db(overwrite=False)

    chat_id, chat_obj = start_chat()

    vals = {"scene": request.get_json()["prompt"]}

    # 1. Get a query to fetch assets
    asset_query = chat_obj.chat(
        gpt_3_5_turbo,
        get_prompt("design", "get_assets_query", vals),
    )

    # TODO: Set sampling size correctly
    # 2. Fetch the assets using the query
    chat_obj.set_context(asset_query, 10)
    chat_obj.clear_history()

    # 3. Generate the scene
    response = json.loads(
        chat_obj.chat(
            gpt_3_5_turbo,
            get_prompt("design", "usr_msg", vals),
        )
    )

    response["chat_id"] = chat_id
    return jsonify(response), 200


if __name__ == "__main__":
    app.run(host="0.0.0.0", port="5555", debug=True)

