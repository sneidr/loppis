// För varje par<säljarnummer, säljarnamn>
// Kopiera mailutkast.txt med filnamn: Säljarnamn.txt"
// Ersätt taggar med siffror
using SellerList = System.Collections.Generic.Dictionary<int, Seller>;
using MailList = System.Collections.Generic.Dictionary<string, string>;

var sellers = ReadSellersFromFile();
var mailAddresses = ReadAddressesFromFile();


PopulateSellersWithSalesData(ref sellers, mailAddresses);

CreateMailText(sellers);

MailList ReadAddressesFromFile()
{
    MailList addresses = new();
    foreach (string line in File.ReadAllLines(@"C:\Users\eider\Source\Repos\loppis\LoppisMail\Data\HT22\mailaddresser.csv"))
    {
        var data = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (data.Length < 2)
        {
            throw new Exception("Error!");
        }
        addresses.Add(data[1].ToLower().TrimEnd(), data[0].ToLower().TrimEnd());
    }
    return addresses;
}


void CreateMailText(SellerList sellers)
{
    foreach (var kv in sellers)
    {
        string text = File.ReadAllText(@"C:\Users\eider\Source\Repos\loppis\LoppisMail\Data\HT22\mailutkast.txt");
        var seller = kv.Value;
        text = text.Replace("<Utbetalt>", $"{seller.ToSeller}");
        text = text.Replace("<EFI-fadder>", $"{seller.ToEFI}");
        text = text.Replace("<Summa>", $"{seller.Sum}");
        text = text.Replace("<Antal>", $"{seller.Count}");

        string filename = string.Empty;
        if (string.IsNullOrEmpty(seller.MailAddress))
        {
            if (seller.Sum == 0) continue;
            filename = $@"special\{seller.Name}";
        }
        else
        {
            filename = seller.MailAddress;
        }
        var newFileName = $@"C:\Users\eider\Source\Repos\loppis\LoppisMail\Data\HT22\{filename}";
        File.WriteAllText(newFileName, text);
    }
}

SellerList ReadSellersFromFile()
{
    SellerList sellers = new();
    foreach (string line in File.ReadAllLines(@"C:\Users\eider\Source\Repos\loppis\LoppisMail\Data\HT22\sellers.csv"))
    {
        var data = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (data.Length < 2)
        {
            throw new Exception("Error!");
        }
        sellers.Add(int.Parse(data[0]), new Seller(data));
    }
    return sellers;
}

void PopulateSellersWithSalesData(ref SellerList sellers, MailList addresses)
{
    foreach (string line in File.ReadAllLines(@"C:\Users\eider\Source\Repos\loppis\LoppisMail\Data\HT22\alltransactions.csv"))
    {
        var data = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (data.Length < 4)
        {
            throw new Exception("Error!");
        }

        int id = int.Parse(data[2]);
        int price = int.Parse(data[3]);

        var seller = sellers[id];
        seller.Sum += price;
        seller.Count += 1;

        string sellerName = seller.Name.ToLower().TrimEnd();
        if (mailAddresses.ContainsKey(sellerName))
        {
            seller.MailAddress = mailAddresses[sellerName];
        }
        
        sellers[id] = seller;
    }
}

