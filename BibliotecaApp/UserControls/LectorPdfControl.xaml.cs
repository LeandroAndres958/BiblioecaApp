using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BibliotecaApp.Controls
{
    public partial class LectorPdfControl : UserControl
    {
        public event Action OnRegresar;

        public LectorPdfControl(string titulo, string pdfUrl)
        {
            InitializeComponent();
            txtTitulo.Text = titulo;

            InicializarWebView(pdfUrl);
        }

        private async void InicializarWebView(string pdfUrl)
        {
            try
            {
                // Inicializar el entorno WebView2
                await webView.EnsureCoreWebView2Async();

                // Cargar el PDF en el WebView2
                webView.CoreWebView2.Navigate(pdfUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar PDF: " + ex.Message);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            OnRegresar?.Invoke();
        }
    }
}

