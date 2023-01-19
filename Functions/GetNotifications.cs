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
using FunctionApp3.Model;

namespace FunctionApp3.Functions
{
    public static class GetNotifications
    {
        [FunctionName("GetNotifications")]
        public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                ILogger log)
        {

            try
            {
                log.LogInformation("C# HTTP trigger function, GetNotifications, processed a request.");

                // Get the connection string from app settings and use it to create a connection.
                var str = Environment.GetEnvironmentVariable("sqldb_connection");
                string userId = req.Query["userid"];

                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                log.LogDebug("userId", userId);

                string jsonString = "";
                int returnValue = 100;
                string newNotId = Guid.NewGuid().ToString("N");

                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = $"SELECT * FROM Notification WHERE ReceiverUserId = '{userId}' FOR JSON AUTO";
                    cmd.Connection = conn;

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            jsonString = reader.GetString(0);
                        }
                    }
                    conn.Close();

                    log.LogDebug("res", jsonString);

                }

                return new OkObjectResult(jsonString);

            }
            catch (Exception e)
            {

                log.LogInformation("Error", e);
                return new OkObjectResult(e);
            }
        }
    }
}
