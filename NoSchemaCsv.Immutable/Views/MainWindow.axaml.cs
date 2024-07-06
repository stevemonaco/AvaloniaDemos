using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CommunityToolkit.Mvvm.Messaging;
using NoSchemaCsv.Immutable.Services;
using NoSchemaCsv.Immutable.ViewModels;
using System.Linq;

namespace NoSchemaCsv.Immutable.Views;
public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    public MainWindow()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<CsvChangedMessage>(this, CsvChanged);
    }

    private void CsvChanged(object sender, CsvChangedMessage message)
    {
        if (ViewModel.CsvModel is null)
            return;

        tree.Source = CreateSource(ViewModel.CsvModel);
    }

    private FlatTreeDataGridSource<CsvRecord> CreateSource(CsvModel model)
    {
        var source = new FlatTreeDataGridSource<CsvRecord>([]);

        // Id is a guaranteed part of the "schema"
        source.Columns.Add(new TextColumn<CsvRecord, int>(model.Header.HeaderNames[0], record => record.Id));

        // The rest of the "schema" is unknown and displaying as unformatted strings is ok.
        // There's no specific type information provided (eg. numeric types), but this is an immutable viewer.
        // You would need to write an autodetection heuristic or allow the user to specify how columns operate.
        source.Columns.AddRange(model.Header.HeaderNames.Skip(1).Select((x, i) =>
                new TextColumn<CsvRecord, string>(x, record => record.Items[i], width: GridLength.Star)
        ));

        source.Items = model.Records;

        return source;
    }
}