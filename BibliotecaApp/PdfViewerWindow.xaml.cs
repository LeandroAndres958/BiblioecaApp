using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using PdfiumViewer;
using BibliotecaApp.Utils;


namespace BibliotecaApp
{
    public partial class PdfViewerWindow : Window
    {
        private string tempPdfPath;
        private PdfViewer pdfViewerControl;

        public PdfViewerWindow()
        {
            InitializeComponent();

            // Crear el control PdfViewer y asignarlo al WindowsFormsHost
            pdfViewerControl = new PdfViewer();
            wfHost.Child = pdfViewerControl;
        }

        // Método para descargar y cargar PDF desde URL
        public async Task LoadPdfFromUrl(string pdfUrl)
        {
            try
            {
                // Crear carpeta temporal para guardar el PDF
                string tempFolder = Path.Combine(Path.GetTempPath(), "MiAppTemp");
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                // Generar ruta temporal única para el PDF
                tempPdfPath = Path.Combine(tempFolder, $"temp_{Guid.NewGuid()}.pdf");

                using (HttpClient client = new HttpClient())
                {
                    // Descargar archivo PDF en bytes
                    byte[] pdfBytes = await client.GetByteArrayAsync(pdfUrl);

                    // Guardar archivo temporal
                    File.WriteAllBytes(tempPdfPath, pdfBytes);

                }

                // Cargar el PDF en el control PdfViewer
                var pdfDocument = PdfDocument.Load(tempPdfPath);
                pdfViewerControl.Document = pdfDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Borrar archivo temporal cuando se cierra la ventana
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            try
            {
                if (!string.IsNullOrEmpty(tempPdfPath) && File.Exists(tempPdfPath))
                    File.Delete(tempPdfPath);
            }
            catch { /* No hacer nada si falla */ }
        }
    }
}
