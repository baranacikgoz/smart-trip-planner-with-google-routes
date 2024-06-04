using Microsoft.VisualBasic.FileIO;

namespace SmartTripPlanner.API.Services;

public class CsvService
{
    public IEnumerable<T> ReadFromCsv<T>(string csvPath, Func<string[], T> mapper, bool fieldsEnclosedInQuotes = true)
        => ReadFields(csvPath, fieldsEnclosedInQuotes)
          .Select(mapper);

    private static IEnumerable<string[]> ReadFields(string csvPath, bool fieldsEnclosedInQuotes)
    {
        using var csvParser = new TextFieldParser(csvPath);

        csvParser.CommentTokens = ["#"];
        csvParser.SetDelimiters([","]);
        csvParser.HasFieldsEnclosedInQuotes = fieldsEnclosedInQuotes;

        // Skip the row with the column names
        csvParser.ReadLine();

        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields() ?? throw new Exception("Fields are null");

            yield return fields;
        }
    }

    public void WriteToCsv(string csvPath, IEnumerable<string[]> fields, bool fieldsEnclosedInQuotes = true)
    {
        using var writer = new StreamWriter(csvPath);

        foreach (var fieldArray in fields)
        {
            var line = fieldsEnclosedInQuotes
                ? string.Join(",", fieldArray.Select(field => $"\"{field}\""))
                : string.Join(",", fieldArray);

            writer.WriteLine(line);
        }
    }
}
