import os
import json
import shutil

from langchain_openai import OpenAIEmbeddings
from langchain_community.vectorstores import Chroma

EMBEDDING_MODEL = "text-embedding-ada-002"
VECTOR_DB_PATH = "src/db/assets_db"


def create_asset_db(overwrite=False, json_path="src/assets/assets.json"):
    if os.path.exists(VECTOR_DB_PATH) and not overwrite:
        return

    if os.path.exists(VECTOR_DB_PATH):
        shutil.rmtree(VECTOR_DB_PATH)

    json_data = json.load(open(json_path, "r"))
    texts = [json.dumps(i) for i in json_data["assets"]]

    Chroma.from_texts(
        texts, OpenAIEmbeddings(model=EMBEDDING_MODEL), persist_directory=VECTOR_DB_PATH
    )


# create_asset_db(overwrite=True)
