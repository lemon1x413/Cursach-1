using System;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Controls;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Cursach_1{
    public partial class MainWindow : Window
    {
        private DataTable _dataTableOriginal;

        private Matrix? _invertedMatrix;
        
        private const int N = 10;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMatrix(3);
        }
        private void Formating(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(double))
            {
                var col = e.Column as DataGridTextColumn;
                col.Binding.StringFormat = "F" + N;
            }
        }
        
        private void BtnSetSize_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(MatrixSize.Text, out int size) || size <= 0 )
            {
                MessageBox.Show("Невірний розмір.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            InitializeMatrix(size);
            StatusBar.Text = $"Розмір матриці встановлено: {size}x{size}";
        }
        
        private void InitializeMatrix(int size)
        {
            _dataTableOriginal = new DataTable();
            for (int j = 0; j < size; j++)
                _dataTableOriginal.Columns.Add(j.ToString(), typeof(double));
            for (int i = 0; i < size; i++)
            {
                var row = _dataTableOriginal.NewRow();
                for (int j = 0; j < size; j++)
                    row[j] = 0.0;
                _dataTableOriginal.Rows.Add(row);
            }

            OriginalMatrixGrid.ItemsSource = _dataTableOriginal.DefaultView;
            InverseMatrixGrid.ItemsSource = null;
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
            if (_invertedMatrix == null) { MessageBox.Show("Спочатку обчисліть обернену матрицю.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
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
                    MessageBox.Show("Помилка при збереженні: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var matrix = Matrix.RandomGenerate(_dataTableOriginal.Columns.Count);
            LoadFromMatrix(matrix);
            StatusBar.Text = "Матрицю згенеровано.";
        }

        private void LoadFromMatrix(Matrix m)
        {
            InitializeMatrix(m.N);
            for (int i = 0; i < m.N; i++)
            for (int j = 0; j < m.N; j++)
                _dataTableOriginal.Rows[i][j] = m[i, j];
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
    }
}