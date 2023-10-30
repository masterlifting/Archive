using System.Text;
using System.Text.Json;
using Octokit;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        string token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        token = "<token>";

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("GITHUB_TOKEN environment variable not set!");
            return;
        }

        Console.WriteLine("Enter the GitHub organization name:");
        string orgName = Console.ReadLine();

        var client = new GitHubClient(new ProductHeaderValue("GitHubOrgReposFetcher"))
        {
            Credentials = new Credentials(token)
        };

        var repos = await client.Repository.GetAllForOrg(orgName);

        var repoMetaDataList = JsonSerializer.Deserialize<List<RepoMetaData>>(File.ReadAllText("metadata.json"), new JsonSerializerOptions(){PropertyNameCaseInsensitive = true});
        var metaDataLookup = new Dictionary<string, RepoMetaData>();
        foreach (var item in repoMetaDataList)
        {
            metaDataLookup[item.Repo] = item;
        }

        var markdownTable = GenerateMarkdownTable(repos, metaDataLookup);
        Console.WriteLine(markdownTable);
    }

    static string GenerateMarkdownTable(IEnumerable<Repository> repositories, Dictionary<string, RepoMetaData> metaDataLookup)
    {
        var sb = new StringBuilder();
        sb.AppendLine("| Name | DaprAppId | Prefix | DbName | SchemaId |");
        sb.AppendLine("|------|----------|-------|-------|---------|");

        foreach (var repo in repositories)
        {
            if (metaDataLookup.TryGetValue(repo.Name, out var metaData))
            {
                sb.AppendLine($"| [{repo.Name}]({repo.HtmlUrl}) | {metaData.DaprAppId} | {metaData.Prefix} | {metaData.DbName} | {metaData.SchemaId} |");
            }
            else
            {
                sb.AppendLine($"| [{repo.Name}]({repo.HtmlUrl}) | - | - | - | - |");
            }
        }

        return sb.ToString();
    }
}

public class RepoMetaData
{
    public string Repo { get; set; }
    public string DaprAppId { get; set; }
    public string Prefix { get; set; }
    public string DbName { get; set; }
    public string SchemaId { get; set; }
}
