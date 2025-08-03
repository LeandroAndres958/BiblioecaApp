using System;
using System.Windows;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;

namespace BibliotecaApp
{
    public partial class MainWindow : Window
    {
        // Variables para almacenar la información real del libro
        private int idLibro;  // Nuevo: id del libro
        private string titulo;
        private string autor;
        private string descripcion;
        private string editorial;
        private string paginas;
        private string idioma;
        private string categoria;
        private string anio;
        private string estado;
        private string portadaUrl;

        // Constructor que recibe todos los datos del libro, incluido el id
        public MainWindow(
            int idLibro,
            string titulo,
            string autor,
            string descripcion,
            string editorial,
            string paginas,
            string idioma,
            string categoria,
            string anio,
            string estado,
            string portadaUrl)
        {
            InitializeComponent(); // Inicializa los componentes visuales de la ventana

            this.idLibro = idLibro;
            this.titulo = titulo;
            this.autor = autor;
            this.descripcion = descripcion;
            this.editorial = editorial;
            this.paginas = paginas;
            this.idioma = idioma;
            this.categoria = categoria;
            this.anio = anio;
            this.estado = estado;
            this.portadaUrl = portadaUrl;

            MostrarDetallesDelLibro();
        }

        // Método para mostrar todos los datos en los controles visuales
        private void MostrarDetallesDelLibro()
        {
            txtTitulo.Text = titulo;
            txtAutor.Text = autor;
            txtDescripcion.Text = descripcion;

            txtEditorial.Text = $"Editorial: {editorial}";
            txtPaginas.Text = $"Páginas: {paginas}";
            txtIdioma.Text = $"Idioma: {idioma}";
            txtCategoria.Text = $"Categoría: {categoria}";
            txtAnio.Text = $"Año de publicación: {anio}";
            txtEstado.Text = $"Estado: {estado}";

            try
            {
                imgPortada.Source = new BitmapImage(new Uri(portadaUrl));
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la portada del libro. Revisa el enlace.\n" + ex.Message);
            }

            // Esto llama a la funcion para actualizar el estado del boton
            ActualizarEstadoBoton();
        }



        private void ActualizarEstadoBoton()
        {
            // Verificar si hay usuario logueado
            if (!App.Current.Properties.Contains("idUsuario"))
            {
                btnSolicitar.Content = "Solicitar libro"; // Por defecto
                return;
            }

            int idUsuario = (int)App.Current.Properties["idUsuario"];

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM prestamos WHERE id_usuario = @idUsuario AND id_libro = @idLibro AND estado = 'activo'";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("@idLibro", idLibro);

                        int prestamos = Convert.ToInt32(cmd.ExecuteScalar());

                        if (prestamos > 0)
                        {
                            btnSolicitar.Content = "📖 Leer";
                        }
                        else
                        {
                            btnSolicitar.Content = "📥 Solicitar libro";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar estado de préstamo: " + ex.Message);
                btnSolicitar.Content = "📥 Solicitar libro"; // Por si hay error, dejar botón en solicitar
            }
        }



        // Evento para solicitar libro
        private void btnSolicitar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar usuario logueado y obtener su id
                if (!App.Current.Properties.Contains("idUsuario"))
                {
                    MessageBox.Show("No se encontró usuario logueado.");
                    return;
                }

                int idUsuario = (int)App.Current.Properties["idUsuario"];

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Validar que usuario no tenga más de 3 libros activos
                    string sqlValidacion = "SELECT COUNT(*) FROM prestamos WHERE id_usuario = @idUsuario AND estado = 'activo'";

                    using (var cmdValidacion = new MySqlCommand(sqlValidacion, conn))
                    {
                        cmdValidacion.Parameters.AddWithValue("@idUsuario", idUsuario);
                        int prestamosActivos = Convert.ToInt32(cmdValidacion.ExecuteScalar());

                        if (prestamosActivos >= 3)
                        {
                            MessageBox.Show("No puedes solicitar más de 3 libros al mismo tiempo.");
                            return;
                        }
                    }

                    // Insertar nuevo préstamo
                    string sqlInsert = "INSERT INTO prestamos (id_usuario, id_libro, estado) VALUES (@idUsuario, @idLibro, 'activo')";

                    using (var cmdInsert = new MySqlCommand(sqlInsert, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@idUsuario", idUsuario);
                        cmdInsert.Parameters.AddWithValue("@idLibro", idLibro);

                        int filas = cmdInsert.ExecuteNonQuery();

                        if (filas > 0)
                        {
                            MessageBox.Show("Libro solicitado con éxito.");
                            btnSolicitar.IsEnabled = false;
                            btnSolicitar.Content = "Solicitado";
                        }
                        else
                        {
                            MessageBox.Show("Error al solicitar el libro.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al solicitar libro: " + ex.Message);
            }
        }

        // Evento para volver (cerrar ventana)
        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

