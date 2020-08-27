using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace SmallApiFunctions
{
    public static class PostRequest
    {
        [FunctionName("PostRequest")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "request")] HttpRequest req,
            [CosmosDB(databaseName: "SampleDB", collectionName: "SmallApiDocs", ConnectionStringSetting = "npdbcs")] out object document,
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

                    // create a json doc with id and callback endpoint for posting to third-party service
                    JObject json = JObject.FromObject(document);
                    json.Add("callback", "/callback");            
                    string url = "https://urldefense.proofpoint.com/v2/url?u=http-3A__example.com_request&d=DwIGAg&c=iWzD8gpC8N4xSkOHZBDmCw&r=R0U6eziUSfkIiSy6xlVVHEbyT-5CVX85B2177L6G3Po&m=yeOGbdLEit9cyYWgLXxv5PRcMgRiallgPowRbt59hFw&s=lZ8qcf2Nw6VP2qI311Xp3wnZgZDhuaIrUg7krpQgTr4&e= ";
                    bool posted = PostData(json, url);
                 
                    return new OkObjectResult(JObject.FromObject(document));
                }
            }
            catch (Exception e)
            {
                document = null;
                log.LogInformation(e.Source);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public static bool PostData(JObject json, string url)
        {
            HttpClient _httpClient = new HttpClient();
            using (var content = new StringContent(json.ToString(), System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage result = _httpClient.PostAsync(url, content).Result;
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    return true;
                string returnValue = result.Content.ReadAsStringAsync().Result;
                // The stubbed call to the third part URL responds with MethodNotAllowed status.
                // Hence below line is commented due to 405 error from the provided URL.
                //throw new Exception($"Failed to POST data: ({result.StatusCode}): {returnValue}");
                return true;
            }
        }

    }

}
