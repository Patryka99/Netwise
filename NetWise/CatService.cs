using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetWise
{
    public class CatService
    {
        private readonly IHttpClientFactory _clientFactory;

        public CatService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task GetAndSaveCatFactAsync(string filePath)
        {
            var client = _clientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync("https://catfact.ninja/fact");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response from server: {responseContent}");

                    using var stream = await response.Content.ReadAsStreamAsync();
                    var catFact = await JsonSerializer.DeserializeAsync<CatFact>(stream);

                    if (catFact != null)
                    {
                        Console.WriteLine($"Cat Fact: {catFact.Fact}, Length: {catFact.Length}");

                        if (!File.Exists(filePath))
                        {
                            using (File.Create(filePath)) { }
                            Console.WriteLine($"File {filePath} does not exist, created new file.");
                        }

                        try
                        {
                            var factLine = $"Fact: \"{catFact.Fact}\", Length: {catFact.Length}";
                            if (new FileInfo(filePath).Length > 0)
                            {
                                factLine = Environment.NewLine + factLine;
                            }
                            await File.AppendAllTextAsync(filePath, factLine);
                            Console.WriteLine("Cat fact saved to file.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred while saving to file: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to deserialize cat fact.");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve cat fact. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while making the HTTP request: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"An error occurred while deserializing JSON response: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
