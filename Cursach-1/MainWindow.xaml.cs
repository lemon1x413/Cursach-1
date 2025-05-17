using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Controls;
using System.Data;


namespace Cursach_1
{
    public partial class MainWindow : Window
    {
        private DataTable _dataTableOriginal;

        private Matrix? _invertedMatrix;

        private const int N = 2;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDataTable(3);
        }

        private void BtnSetSize_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(MatrixSize.Text, out int size) || size <= 0 || size > 25)
            {
                MessageBox.Show("Невірний розмір.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                MatrixSize.Text = "";
                return;
            }

            InitializeDataTable(size);
            StatusBar.Text = $"Розмір матриці встановлено: {size}x{size}";
        }

        private void InitializeDataTable(int size)
        {
            _dataTableOriginal = new DataTable();
            for (int j = 0; j < size; j++)
            {
                _dataTableOriginal.Columns.Add(j.ToString(), typeof(double));
            }
            for (int i = 0; i < size; i++)
            {
                var row = _dataTableOriginal.NewRow();
                for (int j = 0; j < size; j++)
                {
                    row[j] = 0.0;
                }
                _dataTableOriginal.Rows.Add(row);
            }

            OriginalMatrixGrid.ItemsSource = _dataTableOriginal.DefaultView;
            InverseMatrixGrid.ItemsSource = null;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearMatrixInput();
            StatusBar.Text = "Матриця очищена.";
        }
        private void ClearMatrixInput()
        {
            var size = _dataTableOriginal.Columns.Count;
            for (int i = 0; i < size; i++)
            {
                var row = _dataTableOriginal.Rows[i];
                for (int j = 0; j < size; j++)
                {
                    row[j] = 0.0;
                }
            }
        }
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Text Files|*.txt" };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var matrix = FileHandler.LoadMatrix(dialog.FileName);
                    LoadFromMatrix(matrix);
                    MatrixSize.Text = matrix.N.ToString();
                    StatusBar.Text = "Матрицю завантажено.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_invertedMatrix == null)
            {
                MessageBox.Show("Спочатку обчисліть обернену матрицю.", "Увага", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var dlg = new SaveFileDialog { Filter = "Text Files|*.txt;*.csv|All|*.*", FileName = "inverse_matrix.txt" };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    FileHandler.SaveMatrix(_invertedMatrix, dlg.FileName);
                    StatusBar.Text = "Обернену матрицю збережено.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при збереженні: " + ex.Message, "Помилка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var matrix = Matrix.RandomGenerate(_dataTableOriginal.Columns.Count);
            LoadFromMatrix(matrix);
            StatusBar.Text = "Матрицю згенеровано.";
        }

        private void LoadFromMatrix(Matrix matrix)
        {
            InitializeDataTable(matrix.N);
            for (int i = 0; i < matrix.N; i++)
            {
                for (int j = 0; j < matrix.N; j++)
                {
                    _dataTableOriginal.Rows[i][j] = matrix[i, j];
                }
            }
        }

        private void BtnInvert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int n = _dataTableOriginal.Columns.Count;
                if (n >= 8 && InversionMethod.SelectedIndex == 0)
                {
                    if (MessageBox.Show(
                            "Через великий розмір матриці метод окаймлення може не працювати. Чи бажаєте використати методу LUP-розкладу?",
                            "Попередження", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        InversionMethod.SelectedIndex = 1;
                }

                var matrix = new Matrix(n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        matrix[i, j] = Convert.ToDouble(_dataTableOriginal.Rows[i][j]);
                    }
                }

                var time = Stopwatch.StartNew();
                _invertedMatrix = InversionMethod.SelectedIndex == 0
                    ? CofactorInverter.Invert(matrix)
                    : LupInverter.Invert(matrix);
                time.Stop();

                var invertedMatrix = _invertedMatrix.ToDataTable();
                InverseMatrixGrid.ItemsSource = invertedMatrix.DefaultView;
                StatusBar.Text = $"Готово. Час виконання: {time.ElapsedMilliseconds} ms";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void Formating(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(double))
            {
                var col = e.Column as DataGridTextColumn;
                col.Binding.StringFormat = "F" + N;
            }
        }

        private void BtnShowInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Ця програма призначена для обчислення оберненої матриці за допомогою методів окаймлення та LUP-розкладу.\n" +
                "Методи:\n" +
                "1. Метод окаймлення: Використовується для обчислення оберненої матриці шляхом розділення матриці на менші підматриці.\n" +
                "2. Метод LUP-розкладу: Використовує розкладання матриці на нижню, верхню та переставлену матриці для обчислення оберненої матриці.\n\n" +
                "Вимоги:\n" +
                "- Розмір матриці має бути додатним цілим числом і не бути не більше 25.\n" +
                "- Програма перевіряє, чи введене значення є дійсним цілим числом і чи воно більше нуля.\n" +
                "- У разі некоректного введення програма відобразить повідомлення про помилку.\n\n" +
                "Примітка: Для матриці розміром більше 8 метод окаймлення може не працювати, тому програма запропонує використання методу LUP-розкладу.",
                "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
    }
}