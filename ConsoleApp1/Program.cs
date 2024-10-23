using HtmlAgilityPack;

internal class Program
{
    private static async Task Main(string[] args)
    {
        //Setting up var
        List<string> juniorJobs = new List<string>();
        int maxJobCount = 20;
        int currentCount = 0;
        int pageNum = 1;

        //Setting up connections
        HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        var doc = new HtmlDocument();

        while (currentCount < maxJobCount)
        {
            string baseUrl = $"https://www.seek.com.au/junior-developer-jobs-in-information-communication-technology/in-Brisbane-QLD-4000?page={pageNum}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(baseUrl);

                if (response.IsSuccessStatusCode)
                {
                    string pageHtml = await response.Content.ReadAsStringAsync();
                    doc.LoadHtml(pageHtml);

                    //Getting job card
                    var jrRoles = doc.DocumentNode.SelectNodes("//article[@data-testid='job-card']");

                    //If there are no more job card, break
                    if (jrRoles == null || jrRoles.Count == 0)
                    {
                        break;
                    }

                    //Selecting single node from nodes, saving it to the list.
                    foreach (var jrRole in jrRoles)
                    {
                        var jobTitle = jrRole.SelectSingleNode(".//a[@data-automation='jobTitle' and contains(translate(., 'JUNIOR', 'junior'), 'junior')]");
                        if (jobTitle != null)
                        {
                            var stringJobTitle = jobTitle.InnerText.Trim();
                            juniorJobs.Add(stringJobTitle);
                            currentCount++;

                            if (currentCount >= maxJobCount)
                            {
                                break;
                            }
                        }

                    }

                }
                else
                {
                    Console.WriteLine(Console.Error);
                    break;
                }
                Console.WriteLine($"Accessing page {pageNum}");
                pageNum++;
            }
            catch
            {
                Console.WriteLine(Console.Error);
                break;
            }




        }
        //Printing out the list
        for (int i = 0; i < juniorJobs.Count; i++)
        {
            Console.WriteLine(juniorJobs[i]);
        }
    }
}