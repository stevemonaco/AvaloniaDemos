using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NoSchemaCsv.Immutable.Services;

public record CsvModel(CsvHeader Header, IReadOnlyList<CsvRecord> Records);
public record CsvHeader(IReadOnlyList<string> HeaderNames);
public record CsvRecord(int Id, IReadOnlyList<string> Items);

public class CsvService
{
    /// <summary>
    /// Reads the supplied CSV content
    /// </summary>
    /// <param name="csvContent"></param>
    /// <returns>A model representing the CSV, including headers and records</returns>
    public CsvModel ReadCsvContent(string csvContent)
    {
        using var stringReader = new StringReader(csvContent);
        using var reader = new CsvReader(stringReader, CultureInfo.InvariantCulture);

        var records = new List<CsvRecord>();
        reader.Read();
        reader.ReadHeader();
        var header = reader.HeaderRecord;

        while(reader.Read())
        {
            int id = reader.GetField<int>("Id");
            var items = header.Skip(1).Select(x => reader.GetField(x)).ToArray();
            var record = new CsvRecord(id, items);
            records.Add(record);
        }

        return new CsvModel(new(reader.HeaderRecord), records);
    }
}
