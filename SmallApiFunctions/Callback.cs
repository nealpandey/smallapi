using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SmallApiFunctions
{
    /*** Need clarification ****
     * As per the requirements, the Callback endpoint is for third party service to update the status of the document.
     * The requirements has a gap. It seems to be missing the Id of the document in the post request.
     * The JSON input example with Id and Status will be helpful. Over all other requirements are defined well.
     */
    public static class Callback
    {
        [FunctionName("Callback")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", "put", Route = "callback")] HttpRequest req,
            [CosmosDB(databaseName: "SampleDB", collectionName: "Requests", ConnectionStringSetting = "npdbcs")] out object document,
            ILogger log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string rid = data?.id;
            string rstatus = data?.status;
            string rdetail = data?.detail;

            if(string.IsNullOrEmpty(rid))
            {
                document = null;
                return new BadRequestObjectResult("Missing Id.");
            }
            else
            {
                document = new { id = rid, status = rstatus, detail = rdetail };
                log.LogInformation($"Document {rid} updated at {DateTime.Now}");
                // return new OkObjectResult(document.ToString()); // uncomment to view the updated doc.
                return new NoContentResult();
            }
        }
    }
}
