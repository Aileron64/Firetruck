using System.Windows;

namespace FireTruck
{
    public partial class MainWindow : Window
    {
        readonly ViewModel VM = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = VM;
        }

        private void DropFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                VM.AddFile(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
            }
        }

        private void FileSelected(object sender, RoutedEventArgs e)
        {
            VM.ParseInput(VM.InputArrays[VM.FileIndex]);
        }
    }
}
