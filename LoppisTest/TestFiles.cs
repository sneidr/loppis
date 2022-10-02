using System.IO;

namespace LoppisTest;

internal class TestFiles
{
    public static string SettingsFile = @".\config\settings.xml";
    public static string SellerFile = @".\sellers.csv";
    public static string TransactionsFile => @".\mytestfile.xml";

    public static string FirstErrorFile = $"{Path.GetFileNameWithoutExtension(TestFiles.TransactionsFile)}_error1{Path.GetExtension(TestFiles.TransactionsFile)}";
    public static string SeccondErrorFile = $"{Path.GetFileNameWithoutExtension(TestFiles.TransactionsFile)}_error2{Path.GetExtension(TestFiles.TransactionsFile)}";

    public static void RemoveTransactionsFile()
    {
        if (File.Exists(TransactionsFile))
        {
            File.Delete(TransactionsFile);
        }
    }

    public static void SetupTransactionsFile(string contents)
    {
        RemoveTransactionsFile();

        File.Create(TransactionsFile).Close();
        File.WriteAllText(TransactionsFile, contents);
    }

    public static void RemoveSellerListFile()
    {
        if (File.Exists(SellerFile))
        {
            File.Delete(SellerFile);
        }
    }

    public static void SetupSellerListFile(string contents)
    {
        RemoveSellerListFile();

        File.Create(SellerFile).Close();
        File.WriteAllText(SellerFile, contents);
    }

    public static void RemoveSettingsFile()
    {
        if (File.Exists(SettingsFile))
        {
            File.Delete(SettingsFile);
        }
    }

    public static void SetupSettingsFile(string contents)
    {
        RemoveSettingsFile();

        Directory.CreateDirectory(@".\config");
        File.Create(SettingsFile).Close();
        File.WriteAllText(SettingsFile, $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n{contents}");
    }

    public static void RemoveErrorFiles()
    {
        if (File.Exists(FirstErrorFile))
        {
            File.Delete(FirstErrorFile);
        }
        if (File.Exists(SeccondErrorFile))
        {
            File.Delete(SeccondErrorFile);
        }
    }
}
