using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace BibliotecaApp
{
    public partial class InsertarLibroWindow : Window
    {
        public InsertarLibroWindow()
        {
            InitializeComponent();
        }

        // Evento clic del botón para insertar libro
        private void btnInsertar_Click(object sender, RoutedEventArgs e)
        {
            // Validar que ningún campo esté vacío
            if (string.IsNullOrWhiteSpace(txtTitulo.Text) ||
                string.IsNullOrWhiteSpace(txtAutor.Text) ||
                string.IsNullOrWhiteSpace(txtEditorial.Text) ||
                string.IsNullOrWhiteSpace(txtPaginas.Text) ||
                string.IsNullOrWhiteSpace(txtIdioma.Text) ||
                string.IsNullOrWhiteSpace(txtCategoria.Text) ||
                string.IsNullOrWhiteSpace(txtInsertAnio.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                string.IsNullOrWhiteSpace(txtLinkPortada.Text) ||
                string.IsNullOrWhiteSpace(txtLinkPDF.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return; // Salir si hay campos vacíos
            }

            // Validar que el campo 'Páginas' sea un número entero válido
            if (!int.TryParse(txtPaginas.Text, out int paginas))
            {
                MessageBox.Show("Por favor, ingresa un número válido en Páginas.");
                return; // Salir si la validación falla
            }

            try
            {
                // Obtener la cadena de conexión desde el archivo App.config
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                // Crear conexión con la base de datos usando la cadena de conexión
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open(); // Abrir la conexión

                    // Consulta SQL para insertar los datos en la tabla 'libros'
                    string query = @"INSERT INTO libros 
                        (titulo, autor, editorial, paginas, idioma, categoria, año, descripcion, portada_link, pdf_link)
                        VALUES (@titulo, @autor, @editorial, @paginas, @idioma, @categoria, @anio, @descripcion, @portada_link, @pdf_link)";

                    // Crear comando SQL y agregar parámetros para evitar inyección SQL
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@titulo", txtTitulo.Text);
                        cmd.Parameters.AddWithValue("@autor", txtAutor.Text);
                        cmd.Parameters.AddWithValue("@editorial", txtEditorial.Text);
                        cmd.Parameters.AddWithValue("@paginas", paginas);
                        cmd.Parameters.AddWithValue("@idioma", txtIdioma.Text);
                        cmd.Parameters.AddWithValue("@categoria", txtCategoria.Text);
                        cmd.Parameters.AddWithValue("@anio", txtInsertAnio.Text);
                        cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text);
                        cmd.Parameters.AddWithValue("@portada_link", txtLinkPortada.Text);
                        cmd.Parameters.AddWithValue("@pdf_link", txtLinkPDF.Text);

                        cmd.ExecuteNonQuery(); // Ejecutar la consulta
                    }
                }

                MessageBox.Show("Libro insertado correctamente."); // Confirmar éxito
                this.Close(); // Cerrar ventana Insertar libro
            }
            catch (Exception ex)
            {
                // Mostrar cualquier error ocurrido durante la inserción
                MessageBox.Show("Error al insertar el libro: " + ex.Message);
            }


        }

        // 🆕 Evento del botón "Volver al Home"
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Cierra solo esta ventana, HomeAdminWindow sigue abierto
        }
    }
}
