# Smallapi
A quick excercise to demo. The Small API is built using Azure Functions, .Net Core 3.1, C# and Cosmos DB. 


## Features
The sample work provides the following endpoints:

1. For Post a request : https://smallapi.azurewebsites.net/api/request

    This endpoint accepts a JSON document consisting of a one key, "body", assigns a id, and stores the document in CosmoDB
2. Post Callback: https://smallapi.azurewebsites.net/api/callback

    This Callback endpoint can be used to POST update the status of document. It accepts request with document id and status in JSON format.
3. Put Callback: https://smallapi.azurewebsites.net/api/callback

    This Callback endpoint can be used to PUT update the status of document. It accepts request with document id, status and detail in JSON format.
4. Get Status: https://smallapi.azurewebsites.net/api/status/{id}

    Given the unique ID, we should be able to GET the status of the document.
    
Please refer the exercise requirements or contact me for any questions.

## Getting Started

### 1. Clone this samples

Clone this repo with your Visual Studo. 

```
git clone https://github.com/nealpandey/smallapi.git
```

### 2. Prerequisites to run locally.

Microsoft Visual Studio Community 2019
Version 16.6.4

.Net Core 3.1

Azure Development SDK. You can install with Visual Studio 2019.


### 3. Note

This is a first shot and not a finish work. The endpoints have been tested using POSTMAN. As always with some review and iterations, this can be made better.
