using HtmlAgilityPack;
using static ConsoleApp1.@class;

internal class Program
{
    private static async Task Main(string[] args)
    {
        //Setting up var
        List<Job> juniorJobs = new List<Job>();
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
                            string stringJobTitle = jobTitle.InnerText.Trim();
                            string jobURL = jobTitle.GetAttributeValue("href", "");
                            juniorJobs.Add(new Job { Title = stringJobTitle, Url = $"https://www.seek.com{jobURL}" });
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
        Console.Clear();
        for (int i = 0; i < juniorJobs.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {juniorJobs[i].Title}");
        }
        Console.Write("Please enter the index of the job you would like to read: ");


        if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= juniorJobs.Count)
        {
            var selectedJob = juniorJobs[selectedIndex - 1];
            Console.WriteLine($"fetching {selectedJob.Title}....");

            HttpResponseMessage response = await client.GetAsync(selectedJob.Url);
            if (response.IsSuccessStatusCode)
            {
                string jobPageHtml = await response.Content.ReadAsStringAsync();
                var jobDoc = new HtmlDocument();
                jobDoc.LoadHtml(jobPageHtml);

                var descriptionNode = jobDoc.DocumentNode.SelectSingleNode(".//div[@data-automation='jobAdDetails']");
                if (descriptionNode != null)
                {
                    string jobDescription = descriptionNode.InnerText.Trim();
                    Console.Clear();
                    Console.WriteLine($"{jobDescription}");
                }
                else
                {
                    Console.WriteLine("No description");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid input");
        }
    }
}