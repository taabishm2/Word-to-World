import uuid
import json
import openai

from pprint import pprint
from langchain_openai import ChatOpenAI
from flask import Flask, jsonify, request
from langsmith.wrappers import wrap_openai
from langchain_openai import OpenAIEmbeddings
from langchain_openai import OpenAIEmbeddings

from src.llm.chat import Chat, get_prompt, gpt_3_5_turbo, gpt_4
from src.llm.rag import create_asset_db
from src.server.assets import get_scales

# Used to save chats for continued conversations
# ID of the chat is returned in response
CHAT_HISTORY = dict()

EMBEDDING_MODEL = OpenAIEmbeddings()
VECTOR_DB_PATH = "./db"

SCALES_CACHE = dict()

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

# TODO: for feedback, we can pass the current coordinates and scales as json from unity and ask gpt to update keeping in mind the bounds 
# Create a new scene from scratch
@app.route("/create-scene", methods=["PUT"])
def generate_initial_scene():
    global SCALES_CACHE
    
    # Create the asset database
    create_asset_db(overwrite=False)

    chat_id, chat_obj = start_chat()
    
    response = request.get_json()
    hitpoint = (response["hit_point"]["x"], response["hit_point"]["y"], response["hit_point"]["z"])
    print("HITPOINT:", hitpoint)
    
    vals = {
        "scene": response["prompt"],
        "hitpoint_coord": hitpoint,
        }
    
    print("CREATING SCENE:\n\n", vals["scene"])
    
    asset_query = vals["scene"]

    # TODO: Set sampling size correctly
    # 2. Fetch the assets using the query
    chat_obj.set_context(asset_query, 15)
    chat_obj.clear_history()
    
    response = chat_obj.chat(
        gpt_3_5_turbo,
        get_prompt("design", "usr_msg", vals),
    )
    
    print()
    print(response)
    print()
    
    # 3. Generate the scene
    json_list = []
    for e in [i.split(";") for i in response.split("\n")]:
        pos = e[1].strip().replace("(", "").replace(")", "").split(",")
        rot = e[2].strip().replace("(", "").replace(")", "").split(",")
        json_list.append(
            {
                "name": e[0].strip(),
                "position": {
                    "x": int(pos[0].strip()),
                    "y": int(pos[1].strip()),
                    "z": int(pos[2].strip())
                },
                "rotation": {
                    "x": int(rot[0].strip()),
                    "y": int(rot[1].strip()),
                    "z": int(rot[2].strip())
                }
            }
        )
        
    print("Length of assets: ", len(json_list))
    response = {
        "assets": json_list,
    }
    
    if len(SCALES_CACHE) == 0:
        SCALES_CACHE = get_scales()
    # print(SCALES_CACHE)
    
    for entry in response["assets"]:
        scale = SCALES_CACHE[entry["name"]]
        entry["scale"] = {
            "x": scale,
            "y": scale,
            "z": scale
        }

    response["chat_id"] = chat_id
    
    # print("\n"*3)
    # pprint(response)
    # print()
    
    return jsonify(response), 200


if __name__ == "__main__":
    app.run(host="0.0.0.0", port="5555", debug=True)

