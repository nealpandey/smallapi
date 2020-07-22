using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SmallApiFunctions
{
    public static class PostRequest
    {
        [FunctionName("PostRequest")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "request")] HttpRequest req,
            [CosmosDB(databaseName: "SampleDB", collectionName: "Requests", ConnectionStringSetting = "npdbcs")] out object document,
            ILogger log)
        {
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string sbody = data?.body;

                if (string.IsNullOrEmpty(sbody))
                {
                    document = null;
                    log.LogInformation($"Bad request at {DateTime.Now}");
                    return new BadRequestObjectResult("Invalid body.");
                }
                else
                {
                    document = new { id = Guid.NewGuid(), body = sbody };
                    log.LogInformation($"Document received at {DateTime.Now}");
                    return new OkObjectResult(document.ToString());
                }
            }
            catch (Exception e)
            {
                document = null;
                log.LogInformation(e.Source);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }

}
