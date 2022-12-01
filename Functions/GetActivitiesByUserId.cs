using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace FunctionApp3.Functions
{
    public static class GetActivitiesByUserId
    {
        [FunctionName("GetActivitiesByUserId")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation("C# HTTP trigger function processed a request. GetActivitiesByUserId");

                // Get the connection string from app settings and use it to create a connection.
                var str = Environment.GetEnvironmentVariable("sqldb_connection");
                string userId = req.Query["userid"];
                //var user = new AppUser();
                string jsonString = "";

                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = $"SELECT * FROM Activity\n" +
                                        $"WHERE UserId = '{userId}'\n" +
                                        "FOR JSON AUTO";

                    cmd.Connection = conn;

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            jsonString = reader.GetString(0);
                        }
                    }
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string responseMessage = string.IsNullOrEmpty(jsonString)
                    ? "This HTTP triggered function executed successfully. ID finns inte"
                    : jsonString;

                return new OkObjectResult(responseMessage);

            }
            catch (Exception e)
            {

                log.LogInformation("Error", e);
                return new OkObjectResult(e);
            }
        }
    }
}
