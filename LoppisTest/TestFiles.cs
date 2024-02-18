using System.IO;

namespace LoppisTest;

internal class TestFiles
{
    public string TransactionsFile = GenerateUniqueTestFileName();
    public static int count = 0;
    private static string GenerateUniqueTestFileName()
    {

        return $@".\mytestfile_{count++}.xml";
    }

    public string FirstErrorFile => $"{Path.GetFileNameWithoutExtension(TransactionsFile)}_error1{Path.GetExtension(TransactionsFile)}";
    public string SeccondErrorFile => $"{Path.GetFileNameWithoutExtension(TransactionsFile)}_error2{Path.GetExtension(TransactionsFile)}";

    public void RemoveTransactionsFile()
    {
        if (File.Exists(TransactionsFile))
        {
            File.Delete(TransactionsFile);
        }
    }

    public void SetupTransactionsFile(string contents)
    {
        RemoveTransactionsFile();

        File.Create(TransactionsFile).Close();
        File.WriteAllText(TransactionsFile, contents);
    }

    public void RemoveErrorFiles()
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
