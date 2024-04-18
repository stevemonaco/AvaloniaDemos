using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSchemaCsv.Immutable.Services;
public class CsvGeneratorService
{
    private List<Func<Faker, object>> _generators = [];
    private List<string> _columnNames = [];
    private double _threshold = 0.5;

    /// <summary>
    /// Generates CSV with a random assortment of randomized fields
    /// </summary>
    /// <param name="recordsToGenerate">Number of records to generate</param>
    /// <returns>A CSV string content with column names and then the records</returns>
    public string GenerateCsv(int recordsToGenerate)
    {
        _generators = [];
        _columnNames = [];

        List<object[]> items = [];
        int id = 1;

        _columnNames.Add("Id");
        _generators.Add((Faker faker) => id++); // Id is always present in output, not randomized

        AddFieldGenerators();

        var records = Enumerable.Range(0, recordsToGenerate)
            .Select(x => new Faker())
            .Select(faker => _generators.Select(gen => gen(faker)).ToArray())
            .ToList();

        var columnText = string.Join(", ", _columnNames);
        var sb = new StringBuilder(columnText);
        sb.AppendLine();

        foreach (var record in records)
        {
            for (int i = 0; i < record.Length - 1; i++)
            {
                sb.Append(record[i].ToString());
                sb.Append(", ");
            }

            var last = record[^1]; // Always at least one item present due to the id
            sb.AppendLine(last.ToString());
        }

        return sb.ToString();
    }

    // Adds the full set of field generators
    private void AddFieldGenerators()
    {
        AddFieldGenerator("FirstName", (Faker faker) => faker.Person.FirstName);
        AddFieldGenerator("LastName", (Faker faker) => faker.Person.LastName);
        AddFieldGenerator("DateOfBirth", (Faker faker) => faker.Person.DateOfBirth);
        AddFieldGenerator("Phone", (Faker faker) => faker.Person.Phone);
        AddFieldGenerator("UserName", (Faker faker) => faker.Person.UserName);
        AddFieldGenerator("Website", (Faker faker) => faker.Person.Website);

        AddFieldGenerator("StreetAddress", (Faker faker) => faker.Address.StreetAddress());
        AddFieldGenerator("StreetName", (Faker faker) => faker.Address.StreetName());
        AddFieldGenerator("City", (Faker faker) => faker.Address.City());
        AddFieldGenerator("Country", (Faker faker) => faker.Address.Country());
        AddFieldGenerator("ZipCode", (Faker faker) => faker.Address.ZipCode());
    }

    // Adds one field generator if RNG is below the threshold
    private void AddFieldGenerator(string name, Func<Faker, object> generator, double? addThreshold = null)
    {
        addThreshold ??= _threshold;

        if (Random.Shared.NextDouble() < addThreshold)
        {
            _columnNames.Add(name);
            _generators.Add(generator);
        }
    }
}
