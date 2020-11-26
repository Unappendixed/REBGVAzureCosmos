using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace REBGV.CosmosDB
{
	public static class PushToCosmos
	{
		[FunctionName("PushToCosmos")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");
			string jsonData = await new StreamReader(req.Body).ReadToEndAsync();
			Listing listing = JsonConvert.DeserializeObject<Listing>(jsonData);
			Database database = await FetchDatabaseAsync();
			Container container = await FetchContainerAsync(database);
			int resCode = await AddItemsToContainerAsync(listing, container);

			string responseMessage = (resCode == 200) ?
			"Operation completed successfully.\n" :
			string.Format("Operation failed with error code {0}\n", resCode);

			return new OkObjectResult(responseMessage);
		}

		private static readonly string EndpointUri = System.Environment.GetEnvironmentVariable("EndpointUri");
		private static readonly string PrimaryKey = System.Environment.GetEnvironmentVariable("PrimaryKey");

		public static CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

		public static Listing DeserializeListing(string jsonData)
		{
			Listing listing = JsonConvert.DeserializeObject<Listing>(jsonData);
			return listing;
		}

		public static async Task<Database> FetchDatabaseAsync()
		{
			Database db = await cosmosClient.CreateDatabaseIfNotExistsAsync("listingInput");
			return db;
		}
		
		public static async Task<Container> FetchContainerAsync(Database database)
		{
			Container container = await database.CreateContainerIfNotExistsAsync("listings", "/addressStreetName");
			return container;
		}

		private static async Task<int> AddItemsToContainerAsync(Listing data, Container cont)
		{
			try
			{
				ItemResponse<Listing> dataResponse = await cont.CreateItemAsync<Listing>(data, new PartitionKey(data.addressStreetName));
				Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", dataResponse.Resource.id, dataResponse.RequestCharge);
				return 200;
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
			{
				Console.WriteLine("Item in database with id: {0} already exists\n", data.id);
				return (int) ex.StatusCode;
			}
		}
	}
}
