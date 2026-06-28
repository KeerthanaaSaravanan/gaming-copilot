import os
from typing import List
from langchain.document_loaders import TextLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.embeddings import SentenceTransformerEmbeddings
from langchain.vectorstores import Chroma

# Configuration
DATA_DIR = "./data"
CHROMA_DB_DIR = "./chroma_db"
EMBEDDING_MODEL_NAME = "all-MiniLM-L6-v2"
CHUNK_SIZE = 300
CHUNK_OVERLAP = 50

def load_documents(data_dir: str) -> List[Any]:
    """
    Loads all text documents from the specified directory.
    """
    documents = []
    for filename in os.listdir(data_dir):
        if filename.endswith(".txt"):
            file_path = os.path.join(data_dir, filename)
            print(f"Loading document: {file_path}")
            loader = TextLoader(file_path)
            # Add source metadata to each document
            loaded_docs = loader.load()
            for doc in loaded_docs:
                doc.metadata["source"] = filename
            documents.extend(loaded_docs)
    return documents

def chunk_documents(documents: List[Any]) -> List[Any]:
    """
    Splits documents into smaller chunks using a RecursiveCharacterTextSplitter.
    Preserves source metadata for each chunk.
    """
    text_splitter = RecursiveCharacterTextSplitter(
        chunk_size=CHUNK_SIZE,
        chunk_overlap=CHUNK_OVERLAP,
        length_function=len,
        add_start_index=True,
    )
    chunks = text_splitter.split_documents(documents)
    print(f"Split into {len(chunks)} chunks.")
    return chunks

def create_embeddings_and_store(chunks: List[Any]):
    """
    Creates embeddings for document chunks using SentenceTransformerEmbeddings
    and stores them in ChromaDB.
    """
    print(f"Initializing embeddings with model: {EMBEDDING_MODEL_NAME}")
    embeddings = SentenceTransformerEmbeddings(model_name=EMBEDDING_MODEL_NAME)

    print(f"Storing {len(chunks)} chunks in ChromaDB at {CHROMA_DB_DIR}")
    # Initialize ChromaDB with the embedding function
    vectordb = Chroma.from_documents(
        documents=chunks,
        embedding=embeddings,
        persist_directory=CHROMA_DB_DIR
    )
    vectordb.persist()
    print("ChromaDB ingestion complete.")

def ingest_data():
    """
    Orchestrates the data ingestion process: loads, chunks, embeds, and stores.
    """
    print("Starting data ingestion...")
    documents = load_documents(DATA_DIR)
    if not documents:
        print("No documents found to ingest.")
        return 0
    chunks = chunk_documents(documents)
    create_embeddings_and_store(chunks)
    return len(documents)

if __name__ == "__main__":
    # Example usage: Run ingestion when the script is executed directly
    ingested_count = ingest_data()
    print(f"Total documents ingested: {ingested_count}")
