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
	public static class NewNotification
    {
        [FunctionName("NewNotification")]
        public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
                ILogger log)
        {

            try
            {
                log.LogInformation("C# HTTP trigger function, NewNotification, processed a request.");

                // Get the connection string from app settings and use it to create a connection.
                var str = Environment.GetEnvironmentVariable("sqldb_connection");

                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                log.LogInformation(requestBody);

                string RecUserId = "";
                string jsonString = "";
                int returnValue = 100;
                string newNotId = Guid.NewGuid().ToString("N");

                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    log.LogInformation("1");
                    cmd.CommandText = $"SELECT TOP 1 [AppUser].[UserId] FROM AppUser WHERE Email = '{data.ReceiverEmail}'";
                    cmd.Connection = conn;
                    log.LogInformation("2");
                    reader = cmd.ExecuteReader();
                    log.LogInformation("3");
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            RecUserId = reader.GetString(0);
                        }
                    }
                    log.LogInformation("4", RecUserId);
                    conn.Close();
                    if (String.IsNullOrEmpty(RecUserId))
                    {
                        return new OkObjectResult("Email does not exist in DB");
                    }
                    conn.Open();
                    log.LogInformation("5");

                    cmd.CommandText = "DECLARE\t@return_value int\n" +
                                        "\n" +
                                        "EXEC\t@return_value = [dbo].[NewNotification]\n" +
                                        $"\t\t@NotId = '{newNotId}',\n" +
                                        $"\t\t@NotType = '{data.NotType}',\n" +
                                        $"\t\t@NotText = '{data.NotText}',\n" +
                                        $"\t\t@ReceiverUserId = '{RecUserId}',\n" +
                                        $"\t\t@UserId = '{data.UserId}',\n" +
                                        $"\t\t@ChallengeInvite = '{data.ChallengeInvite}'\n" +
                                        "\n" +
                                        "SELECT\t'Return Value' = @return_value";

                    cmd.Connection = conn;
                    log.LogInformation("6");
                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        log.LogInformation("7");
                        returnValue = reader.IsDBNull(reader.GetInt32(0)) ? 99 : reader.GetInt32(0);
                        log.LogInformation("8");
                    }
                    
                    conn.Close();


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