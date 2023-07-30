using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Imaging;
namespace WhistleSharp.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();

        NoteReferenceTable.Columns.Clear();
        NoteReferenceTable.Columns.Add(new DataGridTextColumn {
            Header = "Hole",
            FontSize = 15,
            Binding = new Binding("[0]")
        });
        NoteReferenceTable.Columns.Add(new DataGridTextColumn {
            Header = "Note",
            FontSize = 15,
            Binding = new Binding("[1]")
        });
    }

    public void UpdatePreviewImage(Bitmap? bitmap) {
        if (bitmap is null) {
            return;
        }

        PreviewImage.Source = bitmap;
        PreviewImage.Height = bitmap.PixelSize.Height;
        PreviewImage.Width = bitmap.PixelSize.Width;
    }

    public void UpdateReferenceTable(ObservableCollection<string[]> noteReferences) => NoteReferenceTable.ItemsSource = noteReferences;
}