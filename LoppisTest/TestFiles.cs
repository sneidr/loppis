using System.IO;

namespace LoppisTest;

internal class TestFiles
{
    private const string sellerFile = @".\sellers.csv";
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

    public static void RemoveConfigFile()
    {
        if (File.Exists(sellerFile))
        {
            File.Delete(sellerFile);
        }
    }

    public static void SetupConfigFile(string contents)
    {
        RemoveConfigFile();

        File.Create(sellerFile).Close();
        File.WriteAllText(sellerFile, contents);
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
