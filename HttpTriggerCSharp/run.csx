using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<NameResult> outputTable, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    name = name ?? data?.name;

    // Store message in Azure Table Storage   
    if (name != null) { 
        outputTable.Add(
            new NameResult() { 
                PartitionKey = "SFXUG", 
                RowKey = Guid.NewGuid().ToString(), 
                Name = name }
            );
    }
    return name == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
}

public class NameResult
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Name { get; set; }
}