import os
from llama_index.core import VectorStoreIndex, SimpleDirectoryReader
os.environ["OPENAI_API_KEY"] = "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"

# Load documents and build index
documents = SimpleDirectoryReader("data").load_data()
index = VectorStoreIndex.from_documents(documents)

# Query index
query_engine = index.as_query_engine()
response = query_engine.query("Give all the details of spheres")
print(response)