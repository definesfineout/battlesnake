using Microsoft.Azure.Cosmos;
using System.Net;
using System.Threading.Tasks;

namespace Starter.Api.DAL
{
    public static class MoveLogRepository
    {
        /// The Azure Cosmos DB endpoint for running this GetStarted sample.
        private static string EndpointUrl = "https://localhost:8081";

        /// The primary key for the Azure DocumentDB account.
        private static string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // The Cosmos client instance
        private static CosmosClient cosmosClient;

        // The database we will create
        private static Database database;

        // The container we will create.
        private static Container container;

        // The name of the database and container we will create
        private const string databaseId = "Battlesnake";
        private const string containerId = "Moves";

        static MoveLogRepository()
        {
            cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);
            CreateDatabaseAsync().Wait();
            CreateContainerAsync().Wait();
        }

        private static async Task CreateDatabaseAsync()
        {
            // Create a new database
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }

        /// Create the container if it does not exist. 
        /// Specifiy "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
        private static async Task CreateContainerAsync()
        {
            // Create a new container
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/Request/Game/Id");
        }

        public static void AddLog(MoveLog moveLog)
        {
            try
            {
                ItemResponse<MoveLog> moveLogResponse =  container.CreateItemAsync<MoveLog>(moveLog, new PartitionKey(moveLog.Request.Game.Id)).Result;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                //Console.WriteLine("Item in database with id: {0} already exists\n", moveLog.Id);
            }
        }

        //private async Task QueryItemsAsync()
        //{
        //    var sqlQueryText = "SELECT * FROM c WHERE c.id = '93a05d6d-d93a-4642-9a64-c81baa4a9284_1'";

        //    Console.WriteLine("Running query: {0}\n", sqlQueryText);

        //    QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
        //    FeedIterator<MoveLog> queryResultSetIterator = container.GetItemQueryIterator<MoveLog>(queryDefinition);

        //    List<MoveLog> logs = new();

        //    while (queryResultSetIterator.HasMoreResults)
        //    {
        //        FeedResponse<MoveLog> currentResultSet = await queryResultSetIterator.ReadNextAsync();
        //        foreach (MoveLog log in currentResultSet)
        //        {
        //            logs.Add(log);
        //            Console.WriteLine($"\tRead {log.Id}\n");
        //        }
        //    }
        //}
    }
}
