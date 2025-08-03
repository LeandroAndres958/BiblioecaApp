using System;
using System.Windows;
using System.Windows.Controls;

namespace BibliotecaApp.Controls
{
    public partial class LectorPdfControl : UserControl
    {
        public event Action OnRegresar;

        public LectorPdfControl(string titulo, string urlPdf)
        {
            InitializeComponent();
            txtTitulo.Text = titulo;

            // Cargar el PDF en el WebBrowser
            if (!string.IsNullOrEmpty(urlPdf))
            {
                try
                {
                    webBrowserPdf.Navigate(urlPdf);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar el PDF: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Evento para regresar a la vista anterior
            OnRegresar?.Invoke();
        }
    }
}

