using BibliotecaApp;
using BibliotecaApp.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using BibliotecaApp.Utils;


namespace MyLibraryApp.Windows.Lector
{
    public partial class MisLibrosWindow : Window
    {
        // Referencia a la ventana Lector anterior para poder mostrarla después
        private LectorWindow lectorWindowAnterior;

        // Guardamos el estado previo de la ventana anterior
        private WindowState estadoAnterior;

        // Detectar si esta ventana se maximizó
        private bool seMaximizo = false;

        public MisLibrosWindow(LectorWindow ventanaAnterior)
        {
            InitializeComponent();
            lectorWindowAnterior = ventanaAnterior;

            // Guardamos el estado actual de la ventana anterior
            estadoAnterior = lectorWindowAnterior.WindowState;

            CargarMisLibros();

            // Eventos para controlar estado de ventana y cierre
            this.StateChanged += MisLibrosWindow_StateChanged;
            this.Closing += MisLibrosWindow_Closing;
        }

        private void MisLibrosWindow_StateChanged(object sender, EventArgs e)
        {
            // Detectamos si esta ventana se maximizó
            seMaximizo = this.WindowState == WindowState.Maximized;
        }

        private void MisLibrosWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Al cerrar, mostramos la ventana anterior
            lectorWindowAnterior.Show();

            // Y restauramos el estado (maximizado si esta estaba maximizada)
            if (seMaximizo)
                lectorWindowAnterior.WindowState = WindowState.Maximized;
            else
                lectorWindowAnterior.WindowState = estadoAnterior;
        }

        // Aquí va el resto de tu código que ya tienes...

        public class PrestamoModel
        {
            public int Id { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public string PortadaLink { get; set; }
            public string Estado { get; set; }
            public string PdfLink { get; set; }
        }

        private void CargarMisLibros()
        {
            try
            {
                var libros = new List<PrestamoModel>();

                string correo = App.Current.Properties["usuarioCorreo"]?.ToString();
                if (string.IsNullOrEmpty(correo))
                {
                    MessageBox.Show("No se encontró usuario logueado.");
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT p.id, l.titulo, l.descripcion, l.portada_link, p.estado, l.pdf_link
                        FROM prestamos p
                        INNER JOIN libros l ON p.id_libro = l.id
                        INNER JOIN usuarios u ON p.id_usuario = u.id
                        WHERE u.correo = @correo;
                    ";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", correo);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                libros.Add(new PrestamoModel
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Titulo = reader["titulo"].ToString(),
                                    Descripcion = reader["descripcion"].ToString(),
                                    PortadaLink = reader["portada_link"].ToString(),
                                    Estado = reader["estado"].ToString(),
                                    PdfLink = reader["pdf_link"] == DBNull.Value ? null : reader["pdf_link"].ToString()
                                });
                            }
                        }
                    }
                }

                icMisLibrosItems.ItemsSource = libros;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando libros: " + ex.Message);
            }
        }

        private void btnInicio_Click(object sender, RoutedEventArgs e)
        {
            // Cerramos esta ventana para que se aplique el evento Closing
            this.Close();
        }

        private void BtnVerLibro_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is PrestamoModel libro)
            {
                if (!string.IsNullOrEmpty(libro.PdfLink))
                {
                    LeerLibro(libro.Titulo, libro.PdfLink);
                }
                else
                {
                    MessageBox.Show("No hay archivo PDF disponible para este libro.");
                }
            }
        }

        private void LeerLibro(string titulo, string pdfUrl)
        {
            string linkDirecto = GoogleDriveUtils.ConvertirLinkGoogleDrive(pdfUrl);

            var lectorPdf = new LectorPdfControl(titulo, linkDirecto);
            lectorPdf.OnRegresar += () =>
            {
                gridContenedorPdf.Children.Clear();
                CargarMisLibros();
                gridContenedorPdf.Visibility = Visibility.Collapsed;
                icMisLibros.Visibility = Visibility.Visible;
            };

            icMisLibros.Visibility = Visibility.Collapsed;
            gridContenedorPdf.Visibility = Visibility.Visible;
            gridContenedorPdf.Children.Clear();
            gridContenedorPdf.Children.Add(lectorPdf);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is PrestamoModel libro)
            {
                var result = MessageBox.Show("¿Estás seguro que quieres devolver este libro?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    CancelarPrestamo(libro.Id);
                    CargarMisLibros();
                }
            }
        }

        private void CancelarPrestamo(int prestamoId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "DELETE FROM prestamos WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", prestamoId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cancelando préstamo: " + ex.Message);
            }
        }
    }
}
