using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PersonIdentification.Pages
{
    public class AutoPredictionMLAzure
    {
        public class StringTable
        {
            public string[] ColumnNames { get; set; }
            public string[,] Values { get; set; }
        }

        public static async Task<string> InvokeRequestResponseService(string[,] values)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() { 
                        { 
                            "input1", 
                            new StringTable() 
                            {
                                ColumnNames = new string[] {"HR", "ST", "Label"},
                                Values = values
                                //Values = new string[,] {  
                                //                          { "70", "27", "" },
                                //                          { "72", "33", "" },
                                //                          {"73","28",""}
                                //                        }
                            }
                        },
                                        },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };


                //Drawing Test
                //const string apiKey = "2wquoba9kdSu8jCVwfmjSDExy43A45EJQ9IBJNaZLgq8mXvoVhNwPlUYGRzCFY1xR8LznUaqqQkcT20HXUqfcQ=="; // Replace this with the API key for the web service
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                //client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/fb7ca60e21874824a86e3cc4eea95c81/services/dd59ff3435724461aaee5ed095c69640/execute?api-version=2.0&details=true");
                //HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                //Resting Tremor
                //Two-class Boosted Decision Tree
                const string apiKey = "flNxbI6JsvZ9uzAmRDPjB5o24djpqiy4q9gB4JZdmFnxiXg4syT9xf+XmSwkge0Udk8OE1FyhhjYdhA5/iPaQg=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/fb7ca60e21874824a86e3cc4eea95c81/services/e0a21415194940239ca0acf29c1d3f2a/execute?api-version=2.0&details=true");
                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
                if (response.IsSuccessStatusCode)
                //Two-class Neural Network
                //const string apiKey = "WHbmQepyDaXMj6HTIBbmc5wMJrE+toZDCJkGSfom182qaGwPeZpIp/yCBFlqlHCqw6EtxK4o59hmdf1fmgfHsw=="; // Replace this with the API key for the web service
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                //client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/fb7ca60e21874824a86e3cc4eea95c81/services/2b92beaab807400e8a799564486687bb/execute?api-version=2.0&details=true");
                //HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
                //if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    return jsonString;
                }
                else
                {
                    throw new Exception(string.Format("Failed with status code: {0}", response.StatusCode));
                }
            }
        }
    }

}






