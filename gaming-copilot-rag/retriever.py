import os
from typing import List, Dict, Any
import json

from dotenv import load_dotenv
from langchain_community.embeddings import SentenceTransformerEmbeddings
from langchain_community.vectorstores import Chroma
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate

# Load environment variables
load_dotenv()

# Configuration
CHROMA_DB_DIR = "./chroma_db"
EMBEDDING_MODEL_NAME = "all-MiniLM-L6-v2"
GOOGLE_API_KEY = os.getenv("GOOGLE_API_KEY")
LLM_MODEL_NAME = "gemini-pro"

class Retriever:
    """
    Handles semantic search and query expansion using ChromaDB and an LLM.
    """
    def __init__(self):
        """
        Initializes the embeddings, loads the ChromaDB, and sets up the LLM.
        """
        print(f"Initializing embeddings with model: {EMBEDDING_MODEL_NAME}")
        self.embeddings = SentenceTransformerEmbeddings(model_name=EMBEDDING_MODEL_NAME)

        print(f"Loading ChromaDB from: {CHROMA_DB_DIR}")
        # Ensure ChromaDB exists before trying to load
        if not os.path.exists(CHROMA_DB_DIR):
            raise FileNotFoundError(f"ChromaDB not found at {CHROMA_DB_DIR}. Please run ingestion first.")

        self.vectordb = Chroma(persist_directory=CHROMA_DB_DIR, embedding_function=self.embeddings)
        print("ChromaDB loaded successfully.")

        if not GOOGLE_API_KEY:
            print("Warning: GOOGLE_API_KEY not found in environment variables. Query expansion will not work.")
            self.llm = None
        else:
            print(f"Initializing LLM with model: {LLM_MODEL_NAME}")
            self.llm = ChatGoogleGenerativeAI(model=LLM_MODEL_NAME, temperature=0, google_api_key=GOOGLE_API_KEY)

        # Prompt for query expansion
        self.query_expansion_prompt = ChatPromptTemplate.from_messages([
            ("system", "You are a helpful assistant specialized in rephrasing technical support queries. Return only a JSON array of 3 specific and distinct ways to rewrite the query."),
            ("user", "Rewrite this gaming device support query in 2 more specific ways that would help retrieve relevant documentation: {query}. Return only a JSON array of 3 strings."),
        ])

    def semantic_search(self, query: str, top_k: int = 5) -> List[Dict[str, Any]]:
        """
        Performs a semantic search on the ChromaDB.
        """
        print(f"Performing semantic search for query: '{query}' with top_k={top_k}")
        # Retrieve documents and their relevance scores
        results = self.vectordb.similarity_search_with_score(query, k=top_k)

        formatted_results = []
        for doc, score in results:
            formatted_results.append({
                "content": doc.page_content,
                "source": doc.metadata.get("source", "unknown"),
                "score": score
            })
        return formatted_results

    async def expand_query(self, query: str) -> List[str]:
        """
        Rewrites the query using an LLM to get more specific search queries.
        """
        if not self.llm:
            print("LLM not initialized. Cannot perform query expansion.")
            return [query] # Return original query if LLM is not available

        print(f"Expanding query: '{query}' using LLM")
        chain = self.query_expansion_prompt | self.llm
        response = await chain.ainvoke({"query": query})

        # Parse the JSON array from the LLM's string response
        try:
            expanded_queries = json.loads(response.content)
            if not isinstance(expanded_queries, list) or len(expanded_queries) != 3:
                raise ValueError("LLM did not return a JSON array of 3 strings.")
            print(f"Expanded queries: {expanded_queries}")
            return expanded_queries
        except (json.JSONDecodeError, ValueError) as e:
            print(f"Error parsing LLM response for query expansion: {e}")
            print(f"LLM response content: {response.content}")
            return [query] # Fallback to original query

    async def expanded_search(self, query: str, top_k: int = 5) -> List[Dict[str, Any]]:
        """
        Performs query expansion and then searches with all expanded queries,
        deduplicates, and reranks results.
        """
        original_query_results = await self.semantic_search(query, top_k=top_k)
        expanded_queries = await self.expand_query(query)

        all_results: List[Dict[str, Any]] = original_query_results
        seen_contents = {res['content'] for res in original_query_results}

        for exp_query in expanded_queries:
            if exp_query == query: # Avoid re-searching with the exact original query if it's included by LLM
                continue
            exp_results = await self.semantic_search(exp_query, top_k=top_k)
            for res in exp_results:
                if res['content'] not in seen_contents:
                    all_results.append(res)
                    seen_contents.add(res['content'])

        # Re-rank results by score (higher score is better)
        all_results.sort(key=lambda x: x['score'], reverse=True)

        return all_results[:top_k]

if __name__ == "__main__":
    # Example usage (requires GOOGLE_API_KEY in .env and ingested ChromaDB)
    # First, run 'python gaming-copilot-rag/ingestion.py' to create the DB
    retriever = Retriever()

    # Test semantic search
    print("\n--- Testing Semantic Search ---")
    search_query = "double click problem g502"
    results = retriever.semantic_search(search_query, top_k=2)
    for i, res in enumerate(results):
        print(f"Result {i+1}: (Score: {res['score']:.2f}, Source: {res['source']})\n{res['content']}\n")

    # Test expanded search (requires OPENAI_API_KEY)
    if retriever.llm:
        print("\n--- Testing Expanded Search ---")
        expanded_search_query = "my logitech headset mic is muffled"
        import asyncio
        expanded_results = asyncio.run(retriever.expanded_search(expanded_search_query, top_k=3))
        for i, res in enumerate(expanded_results):
            print(f"Result {i+1}: (Score: {res['score']:.2f}, Source: {res['source']})\n{res['content']}\n")
