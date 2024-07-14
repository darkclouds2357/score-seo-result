# Deployment Process

## Back-end
### Database Migration

1. **Navigate to folder:** `./server/src/SeoMatchingService`.
2. **Run migration command:**

   ```bash
   dotnet ef migrations add <version_name> -c SeoRankingDbContext -o ./Migrations -- -con <connection_string>
   ```

3. **Example for localhost:**

    ```bash
    dotnet ef migrations add Initial -c SeoRankingDbContext -o ./Migrations -- -con "Host=localhost;Port=5432;Database=seo-ranking-database;Username=sa;Password=!QA2ws3ed"
    ```

### Service Setting Configuration

- Configuration located in `./server/src/SeoMatchingService/appsettings.json`.
- Define Search Engine configurations and HTML tag XPaths in the SearchEnginePatterns section.
- Currently, only Google Search Engine is defined.

### Adding a Search Engine

- Search Engines are defined in the `./server/src/SearchEngine/` folder.

- Create a .NET 8 project for the search engine client.

- The search engine crawler must implement the `ISearchEngineCrawler` interface.

- Create a public static class `ServiceCollectionExtension` in the search engine project.

- In the `ServiceCollectionExtension` class, define an extension method `public static IServiceCollection AddSearchEngine(this IServiceCollection services, IConfiguration configuration)`.

- Add a reference to the search engine project in the host service SeoMatchingService.

- In the environment, load the search engine assembly using. example `Assembly.Load("SeoMatchingService.SearchEngine.Bing")`.

>_Note_: In this example, the project names are assumed to be the same. In a real implementation, use a plugin architecture for loading search engines.

### APIs

- *Search SEO Rank*
    - POST /api/v1/seo/search
    - Body:
        ```json
        {
        "searchValue": "land registry search",
        "compareUrl": "https://www.infotrack.co.uk/"
        }
        ```
    > _Note_: Validation for the request is not implemented in this example. Implement request validation in a production setting

- *Get SEO History*
    - POST /api/v1/seo/history
    > _Note_: Validation for the request is not implemented in this example. Implement request validation in a production setting.

> _Note_: In this API setup, CORS and HTTPS have been removed for simplicity and only HTTP is allowed. In a real-world scenario, secure APIs with HTTPS, CORS policies, etc., should be implemented through an API gateway or service mesh.

## Client Application

- Created with ReactJS.
- Folder: `./client`
- Configure the server endpoint in `.env` with the environment variable `REACT_APP_API_BASE_URL`.
- Run `npm install` to install dependencies.
- Run `npm start` to start the client project.
