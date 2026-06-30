from pydantic import BaseModel, Field
from typing import List, Dict, Any

class IngestResponse(BaseModel):
    """
    Response model for the /ingest endpoint.
    Indicates the number of documents processed and confirms ingestion.
    """
    status: str = Field(..., example="success")
    message: str = Field(..., example="Successfully ingested 3 documents into ChromaDB.")
    documents_ingested: int = Field(..., example=3)

class SearchRequest(BaseModel):
    """
    Request model for the /search endpoint.
    Defines the query string and the number of top results to retrieve.
    """
    query: str = Field(..., example="my G502 mouse is double clicking")
    top_k: int = Field(default=5, ge=1, le=10, example=3)

class SearchResult(BaseModel):
    """
    Represents a single search result with its content, source, and relevance score.
    """
    content: str = Field(..., example="Double-clicking issues can sometimes be resolved by cleaning the mouse switches...")
    source: str = Field(..., example="logitech_mouse_faq.txt")
    score: float = Field(..., example=0.85)

class SearchResponse(BaseModel):
    """
    Response model for the /search and /search/expanded endpoints.
    Contains a list of ranked search results.
    """
    query: str = Field(..., example="my G502 mouse is double clicking")
    results: List[SearchResult]

class ExpandedSearchRequest(BaseModel):
    """
    Request model for the /search/expanded endpoint.
    Similar to SearchRequest but implies query expansion will occur.
    """
    query: str = Field(..., example="how to fix G Pro X mic issues")
    top_k: int = Field(default=5, ge=1, le=10, example=3)
