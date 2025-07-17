using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace FunctionApp1;

public class Turbine
{
    public const double RevenuePerkW = 0.12;
    public const double TechnicianCost = 250;
    public const double TurbineCost = 100;

    [Function("TurbineRepair")]
    [OpenApiOperation(operationId: "Run")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody("application/json", typeof(RequestBodyModel),
        Description = "JSON request body containing { hours, capacity}")]
    [OpenApiResponseWithBody(
        statusCode: HttpStatusCode.OK,
        contentType: "application/json",
        bodyType: typeof(string),
        Description = "The OK response message containing a JSON result.")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request)
    {
        // Get request body data.
        using var stream = new StreamReader(request.Body);
        string requestBody = await stream.ReadToEndAsync();
        dynamic? data = JsonSerializer.Deserialize<dynamic>(requestBody);
        int? capacity = data?.capacity;
        int? hours = data?.hours;

        // Return bad request if capacity or hours are not passed in
        if (capacity == null || hours == null)
        {
            return new BadRequestObjectResult("Please pass capacity and hours in the request body");
        }
        // Formulas to calculate revenue and cost
        double? revenueOpportunity = capacity * RevenuePerkW * 24;
        double? costToFix = hours * TechnicianCost + TurbineCost;
        string repairTurbine = revenueOpportunity > costToFix ? "Yes" : "No";

        return new OkObjectResult(new
        {
            message = repairTurbine,
            revenueOpportunity = "$" + revenueOpportunity,
            costToFix = "$" + costToFix
        });
    }

    public class RequestBodyModel
    {
        public int Hours { get; set; }
        public int Capacity { get; set; }
    }
}
