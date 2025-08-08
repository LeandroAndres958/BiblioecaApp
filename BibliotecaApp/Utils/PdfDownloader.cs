using System;
using System.IO;
using System.Net;

namespace BibliotecaApp.Utils
{
    public static class PdfDownloader
    {
        // Método para descargar el PDF y devolver la ruta del archivo temporal
        public static string DescargarPdfTemporal(string url)
        {
            try
            {
                string nombreArchivo = Path.GetRandomFileName() + ".pdf";
                string rutaTemporal = Path.Combine(Path.GetTempPath(), nombreArchivo);

                using (var client = new WebClient())
                {
                    client.DownloadFile(url, rutaTemporal);
                }

                return rutaTemporal;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al descargar el PDF: " + ex.Message);
            }
        }

        // Método para borrar el archivo temporal
        public static void BorrarArchivoTemporal(string rutaArchivo)
        {
            try
            {
                if (File.Exists(rutaArchivo))
                {
                    File.Delete(rutaArchivo);
                }
            }
            catch
            {
                // Ignora errores al eliminar archivo
            }
        }
    }
}
