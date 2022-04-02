using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Starter.Api;
using Starter.Api.Requests;
using Starter.Api.Responses;
using System.Net;

namespace CosmosDemo
{
    public class Program
    {
        /// The Azure Cosmos DB endpoint for running this GetStarted sample.
        private string EndpointUrl = "https://localhost:8081";

        /// The primary key for the Azure DocumentDB account.
        private string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "Battlesnake";
        private string containerId = "Moves";

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.GetStartedDemoAsync();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.AddItemsToContainerAsync();
            await this.QueryItemsAsync();

            //await DeleteDatabaseAndCleanupAsync();
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        /// Create the container if it does not exist. 
        /// Specifiy "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Request/Game/Id");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        private async Task AddItemsToContainerAsync()
        {
            var json = File.ReadAllText(@"C:\Users\defines.fineout.LAN\source\repos\defines\Fun\BattleSnake\Docs\Examples\challenge_avoid_snakes.json");
            GameStatusRequest request = JsonConvert.DeserializeObject<GameStatusRequest>(json);

            var moveLog = new MoveLog
            {
                Request = request,
                Response = new MoveResponse
                {
                    Move = "up",
                    Shout = "Why am I doing this to myself?!"
                }
            };

            try
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen".
                ItemResponse<MoveLog> moveLogResponse = await this.container.CreateItemAsync<MoveLog>(moveLog, new PartitionKey(moveLog.Request.Game.Id));
                // Note that after creating the item, we can access the body of the item with the Resource property of the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", moveLogResponse.Resource.Id, moveLogResponse.RequestCharge);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Item in database with id: {0} already exists\n", moveLog.Id);
            }
        }

        private async Task QueryItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c WHERE c.id = '93a05d6d-d93a-4642-9a64-c81baa4a9284_1'";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<MoveLog> queryResultSetIterator = this.container.GetItemQueryIterator<MoveLog>(queryDefinition);

            List<MoveLog> logs = new();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<MoveLog> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (MoveLog log in currentResultSet)
                {
                    logs.Add(log);
                    Console.WriteLine($"\tRead {log.Id}\n");
                }
            }
        }

        private async Task DeleteDatabaseAndCleanupAsync()
        {
            DatabaseResponse databaseResourceResponse = await this.database.DeleteAsync();
            // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

            Console.WriteLine("Deleted Database: {0}\n", this.databaseId);

            //Dispose of CosmosClient
            this.cosmosClient.Dispose();
        }
    }

    //public class Family
    //{
    //    [JsonProperty(PropertyName = "id")]
    //    public string Id { get; set; }
    //    public string LastName { get; set; }
    //    public Parent[] Parents { get; set; }
    //    public Child[] Children { get; set; }
    //    public Address Address { get; set; }
    //    public bool IsRegistered { get; set; }
    //    // The ToString() method is used to format the output, it's used for demo purpose only. It's not required by Azure Cosmos DB
    //    public override string ToString()
    //    {
    //        return JsonConvert.SerializeObject(this);
    //    }
    //}

    //public class Parent
    //{
    //    public string FamilyName { get; set; }
    //    public string FirstName { get; set; }
    //}

    //public class Child
    //{
    //    public string FamilyName { get; set; }
    //    public string FirstName { get; set; }
    //    public string Gender { get; set; }
    //    public int Grade { get; set; }
    //    public Pet[] Pets { get; set; }
    //}

    //public class Pet
    //{
    //    public string GivenName { get; set; }
    //}

    //public class Address
    //{
    //    public string State { get; set; }
    //    public string County { get; set; }
    //    public string City { get; set; }
    //}
}