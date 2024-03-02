using loppis.Model;
using System.IO;
using System.Xml.Serialization;
using SaveList = System.Collections.Generic.List<loppis.Model.Sale>;

namespace loppis.DataAccess;

public class FileDataAccess(string fileName)
{
    public void WriteSale(Sale sale)
    {
        SaveList entries = ReadFromXmlFile();
        entries.Add(sale);
        WriteToXmlFile(entries);
    }

    public void RemoveSale(Sale sale)
    {
        SaveList entries = ReadFromXmlFile();
        entries.Remove(sale);
        WriteToXmlFile(entries);
    }


    public void WriteToXmlFile(SaveList entries)
    {
        using var filestream = new FileStream(SaveFileName, FileMode.Truncate);
        var xmlwriter = new XmlSerializer(typeof(SaveList));
        xmlwriter.Serialize(filestream, entries);
    }

    public SaveList ReadFromXmlFile()
    {
        var entries = new SaveList();
        using (var filestream = new FileStream(SaveFileName, FileMode.OpenOrCreate))
        {
            if (filestream.Length > 0)
            {
                var xmlreader = new XmlSerializer(typeof(SaveList));
                try
                {
                    entries = (SaveList)xmlreader.Deserialize(filestream);
                }
                catch (System.InvalidOperationException)
                {
                    CopyFileToErrorBackup();
                }
            }
        }

        return entries;
    }

    private void CopyFileToErrorBackup()
    {
        int i = NextAvailableErrorFileNumber();
        File.Copy(SaveFileName, GetErrorFileName(i));
    }

    private int NextAvailableErrorFileNumber()
    {
        int i = 0;
        while (File.Exists(path: GetErrorFileName(++i)))
        {
            if (i > 100)
            {
                // Defensive
                // Should never happen
                throw new IOException("Too many error files!");
            }
        }

        return i;
    }

    // Adds "_error<num> to cSaveFileName
    private string GetErrorFileName(int i)
    {
        string dir = Path.GetDirectoryName(SaveFileName);
        string fileNameWoExt = Path.GetFileNameWithoutExtension(SaveFileName);
        string ext = Path.GetExtension(SaveFileName);

        return Path.Combine(dir, $"{fileNameWoExt}_error{i}{ext}");
    }

    public string SaveFileName { get; set; } = fileName;
}
