import os
import yaml
import json
import openai
import random

from langsmith import traceable
from langchain_openai import ChatOpenAI
from langsmith.wrappers import wrap_openai
from langchain_openai import OpenAIEmbeddings
from langchain_community.vectorstores import Chroma
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser

from src.llm.rag import EMBEDDING_MODEL, VECTOR_DB_PATH

# Used for LangSmith observability
client = wrap_openai(openai.Client())


class UserChatHistory:

    def __init__(self):
        self.history = []

    def add(self, usr_msg, assistant_msg):
        self.history.append(
            [
                ("user", usr_msg),
                ("assistant", assistant_msg.replace("{", "{{").replace("}", "}}")),
            ]
        )

    def get(self):
        return self.history


class Chat:

    def __init__(self, sys_msg):
        self.sys_msg = sys_msg
        self.history = UserChatHistory()
        self.context_docs = []

    def chat(
        self,
        llm,
        usr_msg,
        more_context=[],
        ignore_history=False,
        ignore_context=False,
    ):
        response = self._chat(
            llm, usr_msg, more_context, ignore_history, ignore_context
        )

        self.log_chat(
            llm,
            self.sys_msg,
            self.context_docs,
            more_context,
            usr_msg,
            response,
            self.history.get(),
        )

        self.history.add(usr_msg, response)

        # print("LLM RESPONSE:\n", response)
        return response

    def _chat(self, llm, usr_msg, more_context, ignore_history, ignore_context):
        prompt = ChatPromptTemplate.from_messages(
            self._get_msg_chain(usr_msg, more_context, ignore_history, ignore_context)
        )

        output_parser = StrOutputParser()
        chain = prompt | llm | output_parser
        response = chain.invoke({})
        return response

    @traceable
    def get_docs(self, query, sampling_size=3):
        db_path = VECTOR_DB_PATH
        if not os.path.exists(db_path):
            raise ValueError(f"Vector DB for {self.repo_path} does not exist")

        db = Chroma(
            persist_directory=db_path,
            embedding_function=OpenAIEmbeddings(model=EMBEDDING_MODEL),
        )

        retriever = db.as_retriever(
            search_type="mmr",
            search_kwargs={"k": sampling_size},
        )

        return retriever.invoke(query)

    def set_context(self, query, sampling_size=3):
        docs = self.get_docs(query, sampling_size)
        random.shuffle(docs)
        # print("DOCS:")
        # print(docs)
        self.context_docs = docs
        
    def clear_history(self):
        self.history = UserChatHistory()

    @traceable
    def log_chat(
        self, llm, sys_msg, context_docs, more_context, usr_msg, response, history
    ):
        # Do not remove any arg - used for logging
        return response

    def _get_msg_chain(self, usr_msg, more_context, ignore_history, ignore_context):
        chain = []

        # 1. Set the system message
        sys_msg_str = self.sys_msg

        # 2. Include context in system message
        if not ignore_context:
            all_context = self.context_docs + more_context
            all_context = prep_docs_for_query(all_context)
            if all_context:
                sys_msg_str += get_prompt("llm", "context", {"body": all_context})
        sys_msg_str = sys_msg_str.replace("{", "{{").replace("}", "}}")

        # print()
        # print(sys_msg_str)
        # print()
        chain.append(("system", sys_msg_str))

        # 3. Add history messages
        if not ignore_history:
            for historical_msg in self.history.get():
                chain.extend(historical_msg)

        # 4. Add user message
        chain.append(("human", usr_msg))
        usr_msg = usr_msg.replace("{", "{{").replace("}", "}}")

        return chain

def parse_content(doc_content):
    json_doc = json.loads(doc_content)
    res = f"Asset Name: '{json_doc['name']}'\n"
    res += f"Description: {json_doc['description']}\n"
    res += f"Dimensions: X: {json_doc['dimensions']['x']}, Y: {json_doc['dimensions']['y']}, Z: {json_doc['dimensions']['z']}"
    return res

def prep_docs_for_query(docs):
    cleaned_docs = []
    for doc in docs:
        content = parse_content(doc.page_content)
        cleaned_docs.append(f"\n{content}")

    return "\n".join(cleaned_docs)


def get_prompt(category, subcategory, val_dict=None):
    with open("src/llm/prompts.yml", "r") as file:
        data = yaml.safe_load(file)
        if category not in data or subcategory not in data[category]:
            raise ValueError(f"{category}.{subcategory} prompt missing")

        prompt = data.get(category, {}).get(subcategory)

        if val_dict is not None:
            return prompt.format(**val_dict)
        return prompt


# Example Usage

gpt_3_5_turbo = ChatOpenAI(model="gpt-3.5-turbo", temperature=0)
gpt_4 = ChatOpenAI(model="gpt-4", temperature=0)

# c = Chat("You are a helpful AI assistant. Answer in under 100 words")
# c.set_context("Fetch furniture items from the database.", 3)
# r = c.chat(
#     gpt_3_5_turbo,
#     "Which items are mentioned in the context? What cateogry are they?",
# )
# print(r)
