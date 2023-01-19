﻿using System;
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
        [FunctionName("InsertActivity")]
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

                string jsonString = "";
                int returnValue = 100;
                string newNotId = Guid.NewGuid().ToString("N");

                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "DECLARE\t@return_value int\n" +
                                        "\n" +
                                        "EXEC\t@return_value = [dbo].[NewNotification]\n" +
                                        $"\t\t@NotId = '{newNotId}',\n" +
                                        $"\t\t@NotType = '{data.NotType}',\n" +
                                        $"\t\t@NotText = '{data.NotText}',\n" +
                                        $"\t\t@ReceiverUserId = '{data.ReceiverUserId}',\n" +
                                        $"\t\t@UserId = {data.UserId},\n" +
                                        "\n" +
                                        "SELECT\t'Return Value' = @return_value";

                    cmd.Connection = conn;

                    reader = cmd.ExecuteReader();

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