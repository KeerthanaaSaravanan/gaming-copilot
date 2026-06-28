import uvicorn
from fastapi import FastAPI, HTTPException, status
from typing import Dict
from fastapi.responses import JSONResponse
from contextlib import asynccontextmanager
import os

from ingestion import ingest_data, CHROMA_DB_DIR
from retriever import Retriever
from models import IngestResponse, SearchRequest, SearchResponse, ExpandedSearchRequest

# Application lifespan management
@asynccontextmanager
async def lifespan(app: FastAPI):
    """
    Context manager for managing the lifespan of the FastAPI application.
    Initializes the Retriever when the app starts.
    """
    print("Application startup: Initializing Retriever...")
    try:
        app.state.retriever = Retriever()
        print("Retriever initialized successfully.")
    except FileNotFoundError as e:
        print(f"Error during startup: {e}")
        print("Please run the /ingest endpoint or 'python -m gaming-copilot-rag.ingestion' first to create ChromaDB.")
        app.state.retriever = None # Set to None if DB is not found
    yield
    print("Application shutdown.")

app = FastAPI(title="Gaming Copilot RAG API", version="1.0.0", lifespan=lifespan)

@app.get("/health", summary="Health Check", response_model=Dict[str, str])
async def health_check():
    """
    Performs a health check to ensure the API is running.
    """
    return {"status": "healthy"}

@app.post("/ingest", response_model=IngestResponse, summary="Ingest Documents into ChromaDB")
async def ingest_documents():
    """
    Triggers the ingestion process, loading and chunking documents from the 'data/' folder,
    creating embeddings, and storing them in ChromaDB.
    This endpoint will overwrite existing ChromaDB data.
    """
    print("API: Received request to /ingest.")
    try:
        documents_ingested = ingest_data()
        # After ingestion, re-initialize the retriever to load the new data
        app.state.retriever = Retriever()
        return IngestResponse(
            status="success",
            message=f"Successfully ingested {documents_ingested} documents into ChromaDB.",
            documents_ingested=documents_ingested
        )
    except Exception as e:
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Ingestion failed: {e}")

@app.post("/search", response_model=SearchResponse, summary="Semantic Search")
async def search_documents(request: SearchRequest):
    """
    Performs a semantic search based on the provided query and retrieves the top_k most relevant chunks.
    """
    if not app.state.retriever:
        raise HTTPException(status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                            detail="Retriever not initialized. Please run /ingest first.")

    print(f"API: Received request to /search with query: '{request.query}', top_k: {request.top_k}")
    results = await app.state.retriever.semantic_search(request.query, request.top_k)
    return SearchResponse(query=request.query, results=results)

@app.post("/search/expanded", response_model=SearchResponse, summary="Expanded Semantic Search with Query Expansion")
async def expanded_search_documents(request: ExpandedSearchRequest):
    """
    Rewrites the input query using an LLM to generate more specific search queries.
    Performs searches with all generated queries, deduplicates, and re-ranks the results.
    Returns the top_k most relevant chunks.
    Requires OPENAI_API_KEY to be set in the environment for query expansion to work.
    """
    if not app.state.retriever:
        raise HTTPException(status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                            detail="Retriever not initialized. Please run /ingest first.")
    if not app.state.retriever.llm:
        raise HTTPException(status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
                            detail="LLM not configured for query expansion. Please set GOOGLE_API_KEY in your environment.")

    print(f"API: Received request to /search/expanded with query: '{request.query}', top_k: {request.top_k}")
    results = await app.state.retriever.expanded_search(request.query, request.top_k)
    return SearchResponse(query=request.query, results=results)

if __name__ == "__main__":
    # To run the FastAPI application using uvicorn
    # Use 'python -m gaming-copilot-rag.main' from the parent directory
    # or 'uvicorn gaming-copilot-rag.main:app --reload' from the parent directory
    print("Starting FastAPI application...")
    uvicorn.run("__main__:app", host="0.0.0.0", port=8000, reload=True, lifespan="on")
