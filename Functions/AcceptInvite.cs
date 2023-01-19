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
    public static class AcceptInvite
    {
        [FunctionName("AcceptInvite")]
        public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
                ILogger log)
        {

            try
            {
                log.LogInformation("C# HTTP trigger function, AcceptInvite, processed a request.");

                // Get the connection string from app settings and use it to create a connection.
                var str = Environment.GetEnvironmentVariable("sqldb_connection");

                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                log.LogInformation(requestBody);

                string jsonString = "";
                int returnValue = 100;
                string newActId = Guid.NewGuid().ToString("N");

                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = $"INSERT INTO [dbo].[UserChallenge] ([UserId], [ChallengeId]) VALUES ('{data.UserId}', '{data.ChallengeId}')";

                    cmd.Connection = conn;

                    reader = cmd.ExecuteReader();

                    /*                    if (reader.HasRows)
                                        {
                                            while (reader.Read())
                                            {
                                                jsonString = reader.GetString(0);
                                            }
                                        }

                                        log.LogDebug(jsonString);

                                        */

                    if (reader.Read())
                    {
                        returnValue = reader.IsDBNull(reader.GetInt32(0)) ? 99 : reader.GetInt32(0);
                    }


                }

                return new OkObjectResult(returnValue);

            }
            catch (Exception e)
            {

                log.LogInformation("Error", e);
                return new OkObjectResult(e);
            }
        }
    }
}
