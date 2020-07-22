using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;

namespace SmallApiFunctions
{
    public static class GetStatus
    {
        [FunctionName("GetStatus")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "status/{id}")] HttpRequest req,
            [CosmosDB("SampleDB", "Requests",
                ConnectionStringSetting = "npdbcs",
                SqlQuery = "select * from c where c.id = {id}")]
                IEnumerable<Document> Documents,
            ILogger log)
        {
            if (Documents.Count() > 0)
            {
                Document document = Documents.First<Document>();
                return new OkObjectResult(document.Body);
            }
            else
            {
                log.LogInformation(req.GetDisplayUrl());
                return new OkObjectResult($"Document not found for request: {req.GetDisplayUrl()}");
            }
        }
    }
}