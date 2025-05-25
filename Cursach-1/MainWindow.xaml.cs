using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Controls;
using System.Data;


namespace Cursach_1;

public partial class MainWindow
{
    private DataTable _dataTableOriginal;

    private Matrix? _invertedMatrix;

    public MainWindow()
    {
        InitializeComponent();
        InitializeDataTable(3);
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

    private void BtnSetSize_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(MatrixSize.Text, out int size) || size <= 0 || size > 10)
        {
            MessageBox.Show("Невірний розмір. Введіть додатнє ціле число не більше 10.", "Помилка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            MatrixSize.Text = "";
            return;
        }

        InitializeDataTable(size);
        _invertedMatrix = null;
        StatusBar.Text = $"Розмір матриці встановлено: {size}x{size}";
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        ClearTableOriginal();
        InverseMatrixGrid.ItemsSource = null;
        _invertedMatrix = null;
        StatusBar.Text = "Матрицю очищено.";
    }

    private void ClearTableOriginal()
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

        var dialog = new SaveFileDialog { Filter = "Text Files|*.txt;*.csv|All|*.*", FileName = "inverse_matrix.txt" };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                FileHandler.SaveMatrix(_invertedMatrix, dialog.FileName);
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
        _invertedMatrix = null;
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

            var matrix = new Matrix(n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = Convert.ToDouble(_dataTableOriginal.Rows[i][j]);
                    if (matrix[i, j] != 0 && (Math.Abs(matrix[i, j]) < 1e-2 || Math.Abs(matrix[i, j]) > 1e5)) throw new InvalidOperationException($"Значення в a[{i+1}, {j+1}] занадто велике / мале.");
                }
            }

            var time = Stopwatch.StartNew();
            _invertedMatrix = InversionMethod.SelectedIndex == 0
                ? BorderingInverter.Invert(matrix)
                : LupInverter.Invert(matrix);
            time.Stop();

            var invertedMatrix = _invertedMatrix.ToDataTable();
            InverseMatrixGrid.ItemsSource = invertedMatrix.DefaultView;
            
            var iterations = InversionMethod.SelectedIndex == 0 
                ? BorderingInverter.IterationCounter.CurrentMethodIterations 
                : LupInverter.IterationCounter.CurrentMethodIterations;

            StatusBar.Text = $"Готово. Час виконання: {time.ElapsedMilliseconds} ms, Ітерацій: {iterations}";
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
            col.Binding.StringFormat = "F2";
        }
    }

    private void BtnShowInfo_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Ця програма призначена для обчислення оберненої матриці за допомогою методів окаймлення та LUP-розкладу.\n" +
            "Методи:\n" +
            "1. Метод окаймлення: Суть методу окаймленння полягає в тому, щоб поступово «додавати» до вже обчисленої оберненої меншої підматриці нові рядки й стовпці.\n" +
            "2. Метод LUP-розкладу: Використовує розкладання матриці на нижню, верхню та переставлену матриці для обчислення оберненої матриці.\n\n" +
            "Вимоги:\n" +
            "- Розмір матриці має бути додатним цілим числом і не бути не більше 10.\n" +
            "- У разі некоректного введення програма відобразить повідомлення про помилку.\n",
            "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}