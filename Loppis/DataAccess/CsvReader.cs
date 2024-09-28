using System;
using System.Collections.Generic;
using loppis.Model;
namespace Loppis.DataAccess;

public class CsvReader(string fileContent)
{
    private static readonly string[] NewLineSeparators = ["\n", "\r\n", "\r"];
    bool m_bagEntryInFile = false;
    bool m_cardEntryInFile = false;

    public Dictionary<int, Seller> Parse()
    {
        Dictionary<int, Seller> parsedLines = [];
        foreach (string line in fileContent.Split(NewLineSeparators, System.StringSplitOptions.RemoveEmptyEntries))
        {
            var (sellerId, seller) = ParseLine(line);
            parsedLines.Add(sellerId, seller);
        }

        if (!m_bagEntryInFile)
        {
            throw new System.FormatException("File must contain entry for \"Kasse\" with default price.");
        }
        if (!m_cardEntryInFile)
        {
            throw new System.FormatException("File must contain entry for \"Vykort\" with default price.");
        }

        return parsedLines;
    }

    private Tuple<int, Seller> ParseLine(string line)
    {
        string[] data = line.Split(";");
        if (data.Length < 2 || data.Length > 3)
        {
            throw new System.FormatException($"The line was incorrectly formatted: {line}");
        }

        int sellerId = int.Parse(data[0]);
        if (sellerId == SaleEntry.RoundUpId)
        {
            throw new System.FormatException($"Id 999 is reserved. Please choose another id for row: {line}");
        }

        string sellerName = data[1];
        int? defaultPrice = data.Length > 2 ? int.Parse(data[2]) : null;

        if (sellerName == "Kasse" && defaultPrice != null)
        {
            m_bagEntryInFile = true;
            SaleEntry.BagId = sellerId;
        }
        if (sellerName == "Vykort" && defaultPrice != null)
        {
            m_cardEntryInFile = true;
            SaleEntry.CardId = sellerId;
        }

        return Tuple.Create(sellerId, new Seller { DefaultPrice = defaultPrice, Name = sellerName, Id = data[0] });
    }
}