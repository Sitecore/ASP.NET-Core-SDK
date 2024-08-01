---
_layout: landing
---

# Overview
The ASP.NET Core SDK is built to help developers leverage Sitecore Layout Data in their applications, to build layouts and hydrate components.

## Data flow
The SDK enables ASP.NET Core Applications to connect to a Sitecore instance of XM/XP or XMC and retrieve Layout Data. The Layout Data is a JSON object that represents the structure of a page in Sitecore. The Layout Data is used to render the page in the application.

### Basic Execution Sequence
The ASP.NET CoreSDK uses GraphQL to retrieve Layout Data in JSON format. When using Sitecore XM Cloud or Sitecore Experience Edge, the SDK connects to the Sitecore Experience Edge service to retrieve the Layout Data. 
When working with Sitecore XM or Sitecore XP CD servers, the SDK connects to the Sitecore Layout Service to retrieve the Layout Data. 

Below you can see a basic sequence diagram of the execution flow, showing how the data flows between the browser, the ASP.NET Core Application, and the Experience Edge or Layout Service.

```mermaid
sequenceDiagram
    Browser->>ASP.NET Core Application: Page Request
    ASP.NET Core Application-->>Experience Edge / Layout Service: GraphQL Request
    Experience Edge / Layout Service-->>ASP.NET Core Application: Layout Data JSON
    ASP.NET Core Application->>Browser: Page HTML
```

### Full Execution Sequence
The full execution sequence is more detailed and shows how the Layout Data is used to render the page in the application. The sequence diagram below shows the full execution flow, including the rendering of the page in the application.

```mermaid
sequenceDiagram
	actor User
	participant Browser
	box ASP.NET Core Application
	participant App as Standard Middleware
	participant Middleware as Rendering Engine Middleware
	participant Rendering
	participant Client as Layout Service Client
	end
	participant Sitecore as Experience Edge or Layout Service
	
	User->>Browser: Browse to URI
	Browser-->>App: HTTP Request
	activate App
	App->>App: Resolve Controller
	App->>App: Resolve Action
	note right of App: Regular MVC Rendering happens if<br/>there is no [UseSitecoreRendering]<br/>attribute on the action
	opt has [UseSitecoreRendering] attribute
	  App->>Middleware: Execute
	  deactivate App
	  activate Middleware
	  Middleware-->>Client: Layout Service Request
	  activate Client
	  Client-->>+Sitecore: GraphQL Request
	  Sitecore-->>-Client: GraphQL Response
	  Client-->>Middleware: Layout Service Response
	  deactivate Client
	  Middleware->>Middleware: Update HTTP Context
	  Middleware-->>Rendering: HTTP+Rendering Context
	  deactivate Middleware
	  activate Rendering
	  Rendering->>Rendering: Invoke Action
	  Rendering->>Rendering: Execute Razor View
	  Rendering->>Rendering: <sc placeholder />
	  loop Resolve Component
	    alt is Model Bound View
		  Rendering->>Rendering: Model Binding
		  Rendering->>Rendering: Execute Razor View
		else is Custom View Component
		  Rendering->>Rendering: Model Binding
		  Rendering->>Rendering: Execute Razor View
		else is Partial View
		  Rendering->>Rendering: Model Binding
		  Rendering->>Rendering: Execute Razor View
		end
	  end
	  note right of Rendering: Executes recursively for each placeholder
	  Rendering-->>App: HTML
	  deactivate Rendering
	  activate App
	end
	App-->>Browser: HTTP Response
	deactivate App
	Browser->>User: Display Page
```