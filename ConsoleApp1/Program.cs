using HtmlAgilityPack;

internal class Program
{
    private static async Task Main(string[] args)
    {
        HttpClient client = new HttpClient();
        var doc = new HtmlDocument();

        HttpResponseMessage response = await client.GetAsync("https://www.seek.com.au/junior-developer-jobs-in-information-communication-technology/in-Brisbane-QLD-4000");

        if (response.IsSuccessStatusCode)
        {
            string pageHtml = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Fetched success");
            doc.LoadHtml(pageHtml);

            var jrRoles = doc.DocumentNode.SelectNodes("//article[@data-testid='job-card']");

            if (jrRoles.Count > 0)
            {
                foreach (var jrRole in jrRoles)
                {
                    var jobTitle = jrRole.SelectSingleNode(".//a[@data-automation='jobTitle' and contains(translate(., 'JUNIOR', 'junior'), 'junior')]");
                    if(jobTitle != null)
                    {
                        var stringJobTitle = jobTitle.InnerText.Trim();
                        Console.WriteLine(jobTitle.InnerText);
                    }

                }

            }
            else
            {
                Console.WriteLine("Not find");
            }



        }
        else
        {
            Console.WriteLine(response.StatusCode.ToString());
        }
    }
}