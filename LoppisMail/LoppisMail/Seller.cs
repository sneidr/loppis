// För varje par<säljarnummer, säljarnamn>
// Kopiera mailutkast.txt med filnamn: Säljarnamn.txt"
// Ersätt taggar med siffror

struct Seller
{
    public Seller(string[] data)
    {
        Id = int.Parse(data[0]);
        Name = data[1];
        Sum = 0;
        Count = 0;
        MailAddress = string.Empty;
    }
    public int Id;
    public string Name;
    public int Sum;
    public int Count;
    public string MailAddress;

    public int ToEFI => (int)(Sum * 0.3);
    public int ToSeller => Sum - ToEFI;
}

