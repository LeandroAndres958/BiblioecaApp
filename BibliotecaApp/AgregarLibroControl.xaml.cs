using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace BibliotecaApp
{
    public partial class AgregarLibroControl : UserControl
    {
        public AgregarLibroControl()
        {
            InitializeComponent();
        }

        // Evento que se ejecuta al presionar el botón "Insertar Libro"
        private void btnInsertarLibro_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtener la cadena de conexión desde App.config para seguridad
            string conexion = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            try
            {
                // 2. Usamos "using" para abrir y cerrar conexión automáticamente
                using (MySqlConnection conn = new MySqlConnection(conexion))
                {
                    conn.Open(); // Abrimos la conexión

                    // 3. Consulta SQL con parámetros para evitar inyección SQL
                    string query = @"INSERT INTO libros 
                                    (titulo, autor, editorial, paginas, idioma, categoria, anio, descripcion, portada, pdf)
                                    VALUES 
                                    (@titulo, @autor, @editorial, @paginas, @idioma, @categoria, @anio, @descripcion, @portada, @pdf)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // 4. Agregamos valores a los parámetros desde los TextBox
                        cmd.Parameters.AddWithValue("@titulo", txtTitulo.Text);
                        cmd.Parameters.AddWithValue("@autor", txtAutor.Text);
                        cmd.Parameters.AddWithValue("@editorial", txtEditorial.Text);
                        cmd.Parameters.AddWithValue("@paginas", txtPaginas.Text);
                        cmd.Parameters.AddWithValue("@idioma", txtIdioma.Text);
                        cmd.Parameters.AddWithValue("@categoria", txtCategoria.Text);
                        cmd.Parameters.AddWithValue("@anio", txtAnio.Text);
                        cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text);
                        cmd.Parameters.AddWithValue("@portada", txtPortadaLink.Text);
                        cmd.Parameters.AddWithValue("@pdf", txtPdfLink.Text);

                        // 5. Ejecutamos la consulta SQL para insertar el libro
                        cmd.ExecuteNonQuery();

                        // 6. Confirmamos éxito con mensaje
                        MessageBox.Show("✅ Libro insertado correctamente.");

                        // 7. Limpiamos el formulario para insertar otro libro
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error, mostramos mensaje con detalles
                MessageBox.Show("❌ Error al insertar libro: " + ex.Message);
            }
        }

        // Método para limpiar los TextBox después de insertar
        private void LimpiarCampos()
        {
            txtTitulo.Clear();
            txtAutor.Clear();
            txtEditorial.Clear();
            txtPaginas.Clear();
            txtIdioma.Clear();
            txtCategoria.Clear();
            txtAnio.Clear();
            txtDescripcion.Clear();
            txtPortadaLink.Clear();
            txtPdfLink.Clear();
        }
    }
}
