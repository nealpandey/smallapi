using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace SmallApiFunctions
{
    /*** Need clarification ****
     * As per the requirements, the Callback endpoint is for third party service to update the status of the document.
     * The requirements has a gap. It seems to be missing the Id of the document in the Post & Put requests.
     * The JSON input example with Id and Status will be helpful. Over all other requirements are defined well.
     */
    public static class Callback
    {
        [FunctionName("Callback")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = "callback")] HttpRequest req,
            [CosmosDB(databaseName: "SampleDB", collectionName: "SmallApiDocs", ConnectionStringSetting = "npdbcs")] DocumentClient client,
            ILogger log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string rid = data?.id;
            string rstatus = data?.status;
            string rdetail = data?.detail;


            if (rid == null)
            {
                return new BadRequestObjectResult("Missing Id.");
            }
            else
            {

                log.LogInformation($"Searching for document id : {rid}");
                var uri = UriFactory.CreateDocumentCollectionUri("SampleDB", "SmallApiDocs");

                /* SQL Query Option */
                //string sql = "SELECT * FROM c WHERE c.id  = '" + rid + "'";
                //var doc = client.CreateDocumentQuery<Microsoft.Azure.Documents.Document>(uri, sql).AsEnumerable().SingleOrDefault();

                var doc = client.CreateDocumentQuery<Microsoft.Azure.Documents.Document>(uri)
                            .Where(r => r.Id == rid)
                            .AsEnumerable()
                            .SingleOrDefault();

                if (rstatus != null) doc.SetPropertyValue("status", rstatus);
                if (rdetail != null) doc.SetPropertyValue("detail", rdetail);

                var updated = await client.ReplaceDocumentAsync(doc);
                log.LogInformation($"Document {rid} updated at {DateTime.Now}");
                
                return new NoContentResult();
            }
        }
    }
}
