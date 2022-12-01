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
    public static class InsertChallenge
    {
        [FunctionName("InsertChallenge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation("C# HTTP trigger function, InsertChallenge, processed a request.");

                // Get the connection string from app settings and use it to create a connection.
                var str = Environment.GetEnvironmentVariable("sqldb_connection");

                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                log.LogDebug(requestBody);
                
                string jsonString = "";
                int returnValue = 100;
                CreateChallenge newChallCreated;
                string newChallId = Guid.NewGuid().ToString("N");

                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    /*
DECLARE	@return_value int

EXEC	@return_value = [dbo].[InsertChallenge]
		@Name = 'TestSP',
		@StartDate = '2022-01-01',
		@EndDate = '2022-01-02',
		@ChallengeId = 774322

SELECT	'Return Value' = @return_value

GO


                     */
                    cmd.CommandText = "DECLARE\t@return_value int\n" +
                                        "\n" +
                                        "EXEC\t@return_value = [dbo].[InsertChallenge]\n" +
                                        $"\t\t@Name = '{data.Name}',\n" +
                                        $"\t\t@StartDate = '{data.StartDate}',\n" +
                                        $"\t\t@EndDate = '{data.EndDate}',\n" +
                                        $"\t\t@ChallengeId = '{newChallId}',\n" +
                                        $"\t\t@UserId = '{data.UserId}'\n" +
                                        "\n" +
                                        "SELECT\t'Return Value' = @return_value";

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

                    if(returnValue != 0)
                    {
                        log.LogInformation("Error code: " + returnValue);
                    }
                    else
                    {
                        log.LogInformation("Successfull: " + returnValue);
                    }
                    log.LogInformation("before dateonly.parse");
                    DateOnly sd = DateOnly.ParseExact(data.StartDate.ToString(), "yyyy-mm-dd");
                    DateOnly ed = DateOnly.ParseExact(data.EndDate.ToString(), "yyyy-mm-dd");
                    log.LogInformation("after dateonly.parse: " + sd + " " + ed);
                    newChallCreated = new CreateChallenge(newChallId, sd, ed, data.Name.ToString(), data.UserId.ToString(), returnValue);
                    
                }

                return new OkObjectResult(newChallCreated);

            }
            catch (Exception e)
            {

                log.LogInformation("Error", e);
                return new OkObjectResult(e);
            }
        }
    }
}
