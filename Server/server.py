import os
import json
import shutil
import openai

from langsmith import traceable
from langchain_openai import ChatOpenAI
from flask import Flask, jsonify, request
from langsmith.wrappers import wrap_openai
from langchain_community.vectorstores import Chroma
from langchain_openai import OpenAIEmbeddings
from langchain_core.messages import HumanMessage
from langchain.chains import create_history_aware_retriever, create_retrieval_chain
from langchain.chains.combine_documents import create_stuff_documents_chain
from langchain_core.prompts import ChatPromptTemplate
from langchain_openai import OpenAIEmbeddings
from langchain_community.vectorstores import Chroma


LLM_MODEL = ChatOpenAI(model="gpt-3.5-turbo", temperature=0)
EMBEDDING_MODEL = OpenAIEmbeddings()
VECTOR_DB_PATH = "./db"

app = Flask(__name__)
client = wrap_openai(openai.Client())


def create_asset_db(json_path="./assets.json"):
    if os.path.exists(VECTOR_DB_PATH):
        shutil.rmtree(VECTOR_DB_PATH)

    json_data = json.load(open(json_path, "r"))
    texts = [json.dumps(i) for i in json_data["assets"]]

    Chroma.from_texts(texts, EMBEDDING_MODEL, persist_directory=VECTOR_DB_PATH)


@traceable
def ask(question, chat_history, sampling_size=6):
    db = Chroma(
        persist_directory=VECTOR_DB_PATH,
        embedding_function=OpenAIEmbeddings(disallowed_special=()),
    )

    retriever = db.as_retriever(
        search_type="mmr",
        search_kwargs={"k": sampling_size},
    )

    prompt = ChatPromptTemplate.from_messages(
        [
            ("placeholder", "{chat_history}"),
            ("user", "{input}"),
            (
                "user",
                "Given the above conversation, generate a search query to look up information requested by the user.",
            ),
        ]
    )
    retriever_chain = create_history_aware_retriever(LLM_MODEL, retriever, prompt)

    prompt = ChatPromptTemplate.from_messages(
        [
            (
                "system",
                "Answer the user's questions based on the below context:\n\n{context}",
            ),
            ("placeholder", "{chat_history}"),
            ("user", "{input}"),
        ]
    )
    document_chain = create_stuff_documents_chain(LLM_MODEL, prompt)

    qa = create_retrieval_chain(retriever_chain, document_chain)

    result = qa.invoke({"input": question, "chat_history": chat_history})
    chat_history.extend([HumanMessage(content=question), result["answer"]])

    return result["answer"]


def generate_scene(scene):
    resp = ask(
        f"You are a 3D designer with access to the assets mentioned in the context. \
            You are tasked with creating a 3D scene in a 100x100 grid space.\n \
            Return a list of assets which you would use to depict the following scene: {scene}\n \
            Return the name of the asset (exactly as mentioned in the provided context), \
            and the location, scale, and rotation you would set for it. \
            The response MUST be a valid JSON with a list of assets under the key 'assets'. \
            For each asset, include the keys: 'name','location', 'scale', 'rotation' for each asset. \
            The last three must be objects representing the 'x','y','z' values.",
        [],
    )

    return json.loads(resp)


@app.route("/create-scene", methods=["POST"])
def generate_initial_scene():
    create_asset_db()

    data = request.get_json()
    scene = data["prompt"]

    return jsonify(generate_scene(scene)), 200


if __name__ == "__main__":
    app.run(debug=True)
