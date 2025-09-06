**Azure-OpenAI-RAG**
An ASP.NET Core 9 Web API application that demonstrates Retrieval-Augmented Generation (RAG) using Azure Cognitive Search and Azure OpenAI.
The app retrieves context chunks from Azure Cognitive Search and uses them to ground GPT responses from Azure OpenAI.



To read, source - https://www.youtube.com/watch?v=NjbUzAwtizQ - <img width="72" height="52" alt="image" src="https://github.com/user-attachments/assets/81cd65f1-3872-4e1e-b57b-39a42491006a" />


<img width="711" height="368" alt="image" src="https://github.com/user-attachments/assets/3a20997a-d9d1-4f86-823d-0b394ac7afb6" />

**Use case :**
The project has info of cost of different kind of interior work in a house like wood work, stone work, plumber, tile etc etc.
The Azure search has indexed all these data as embeddings, so when asked as query it first goes to embedding and get context info and pass this data along when the 
LLM is called to get grounded data.

--

**📌 Features**

Built with .NET 9 Web API
Uses Azure Cognitive Search to retrieve relevant context from Azure search AI
Calls Azure OpenAI (GPT-4o) for grounded answers
Supports HttpClientFactory for secure API calls
Demonstrates how to handle chunked data from search results
Configurable defaults for queries and deployment setup

<img width="965" height="227" alt="image" src="https://github.com/user-attachments/assets/cf0b9e00-3300-46f4-b504-3c9e27019654" />

--

**⚙️ Configuration**

Before running, set up your Azure resources:
Azure OpenAI
Get your Endpoint and API Key from the portal.
Create a deployment (e.g., gpt-4o).
Azure Cognitive Search
Create an index and upload your documents.
Configure your SearchEndpoint, SearchKey, and IndexName

--

**Get such output**
<img width="1120" height="468" alt="Screenshot 2025-09-06 230314" src="https://github.com/user-attachments/assets/7a2be1c1-821b-41ac-bf8f-0beae6d4d26e" />
