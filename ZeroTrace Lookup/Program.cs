using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine(@"
__________         ___________                            ._____________
\____    /____  ___\__    ___/___________    ____  ____   |   \______   \
  /     // __ \/  _ \|    |  \_  __ \__  \ _/ ___\/ __ \  |   ||     ___/
 /     /\  ___(  <_> )    |   |  | \// __ \\  \__\  ___/  |   ||    |
/_______ \___  >____/|____|   |__|  (____  /\___  >___  > |___||____|
        \/   \/                          \/     \/    \/
");

        Console.Write("Entrez une adresse IP : ");
        string? ip = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(ip) || !IPAddress.TryParse(ip, out var address))
        {
            Console.WriteLine("❌ Adresse IP invalide.");
            PauseConsole();
            return;
        }

        if (IsPrivateIP(address))
        {
            Console.WriteLine("⚠️ Cette IP est privée.");
            Console.WriteLine($"🔹 Hostname : {Dns.GetHostEntry(address).HostName}");
            PauseConsole();
            return;
        }

        await LookupIp(ip);
        PauseConsole();
    }

    static bool IsPrivateIP(IPAddress ip)
    {
        byte[] bytes = ip.GetAddressBytes();
        return (bytes[0] == 10) ||
               (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
               (bytes[0] == 192 && bytes[1] == 168);
    }

    static async Task LookupIp(string ip)
    {
        Console.WriteLine($"\n🔎 Recherche d'informations sur {ip}...\n");

        // API 1: ip-api.com
        string apiUrl1 = $"http://ip-api.com/json/{ip}?fields=status,message,country,regionName,city,zip,lat,lon,isp,org,as,query";
        await GetIpInfo(apiUrl1, "IP-API");

        // API 2: ipinfo.io
        string apiUrl2 = $"https://ipinfo.io/{ip}/json";
        await GetIpInfo(apiUrl2, "IPInfo");
    }

    static async Task GetIpInfo(string url, string source)
    {
        using HttpClient client = new HttpClient();
        try
        {
            string response = await client.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<IpInfo>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                Console.WriteLine($"❌ {source} : Erreur lors de la récupération des données.");
                return;
            }

            Console.WriteLine($" Pays : {data.Country ?? "Non disponible"}");
            Console.WriteLine($" Ville : {data.City ?? "Non disponible"}");
            Console.WriteLine($" Région : {data.RegionName ?? data.Region ?? "Non disponible"}");
            Console.WriteLine($" Code Postal : {data.Zip ?? "Non disponible"}");
            Console.WriteLine($" Latitude : {data.Lat} Longitude : {data.Lon}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ {source} : Erreur - {ex.Message}");
        }
    }

    static void PauseConsole()
    {
        Console.WriteLine("\nAppuyez sur une touche pour quitter...");
        Console.ReadKey();
    }
}

class IpInfo
{
    public string Status { get; set; } = "";
    public string Message { get; set; } = "";
    public string Country { get; set; } = "";
    public string RegionName { get; set; } = "";
    public string Region { get; set; } = "";
    public string City { get; set; } = "";
    public string Zip { get; set; } = "";
    public float Lat { get; set; }
    public float Lon { get; set; }
    public string Isp { get; set; } = "";
    public string Org { get; set; } = "";
    public string As { get; set; } = "";
    public string Query { get; set; } = "";
    public string Ip { get; set; } = "";
}