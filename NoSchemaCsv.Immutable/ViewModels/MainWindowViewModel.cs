using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NoSchemaCsv.Immutable.Services;

namespace NoSchemaCsv.Immutable.ViewModels;
public partial class MainWindowViewModel(CsvService csvService, CsvGeneratorService generatorService) : ViewModelBase
{
    private readonly CsvService _csvService = csvService;
    private readonly CsvGeneratorService _generatorService = generatorService;

    // CsvModel itself is immutable, so lack of INPC and breaking MVVM by
    // exposing the model to the View is low impact
    public CsvModel? CsvModel { get; private set; }

    [ObservableProperty] private int _rowsToGenerate = 200;

    [RelayCommand]
    public void LoadRandomizedCsv()
    {
        var csvContent = _generatorService.GenerateCsv(RowsToGenerate);
        var records = _csvService.ReadCsvContent(csvContent);

        CsvModel = records;
        WeakReferenceMessenger.Default.Send(new CsvChangedMessage());
    }
}
