using System;
using System.Windows;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;

namespace BibliotecaApp
{
    public partial class MainWindow : Window
    {
        // Variables para almacenar la información real del libro
        private int idLibro;
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

        // Referencia a la ventana home (LectorWindow) para poder manejar su estado
        private LectorWindow homeWindow;

        // Estado real actual de esta ventana detalle
        private WindowState estadoActualDetalle;

        // Guardamos el estado que tenía Home antes de abrir Detalle
        private WindowState estadoHomeAntesDeDetalle;

        // Constructor que recibe todos los datos del libro, la ventana home y el estado previo de Home
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
            string portadaUrl,
            LectorWindow homeVentana,
            WindowState estadoHomeAnterior)
        {
            InitializeComponent();

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

            this.homeWindow = homeVentana;
            this.estadoHomeAntesDeDetalle = estadoHomeAnterior;

            // Mostrar detalles en la interfaz
            MostrarDetallesDelLibro();

            // Detectar cambios de estado para actualizar variable
            this.StateChanged += MainWindow_StateChanged;

            // Guardamos el estado inicial
            estadoActualDetalle = this.WindowState;
        }

        // Detecta cambio de estado ventana Detalle
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            estadoActualDetalle = this.WindowState;
        }

        // Al cerrar la ventana detalle
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (homeWindow != null)
            {
                // Mostrar ventana Home
                homeWindow.Show();

                // Actualizar tamaño y posición de Home igual que Detalle
                homeWindow.Width = this.Width;
                homeWindow.Height = this.Height;
                homeWindow.Left = this.Left;
                homeWindow.Top = this.Top;

                // Controlamos estado para evitar que Home cambie a minimizado si Detalle estaba minimizado
                if (estadoActualDetalle == WindowState.Maximized)
                {
                    homeWindow.WindowState = WindowState.Maximized;
                }
                else if (estadoActualDetalle == WindowState.Minimized)
                {
                    // En vez de poner Home minimizado, ponemos el estado previo que tenía Home
                    homeWindow.WindowState = estadoHomeAntesDeDetalle;
                }
                else
                {
                    // Si Detalle estaba en Normal, dejamos el estado actual de Home intacto
                }

                homeWindow.Activate();
            }
        }

        // Mostrar los datos en los controles de la interfaz
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
                MessageBox.Show("No se pudo cargar la portada del libro.\n" + ex.Message);
            }

            ActualizarEstadoBoton();
        }

        // Método para actualizar estado del botón "Solicitar"
        private void ActualizarEstadoBoton()
        {
            if (!App.Current.Properties.Contains("idUsuario"))
            {
                btnSolicitar.Content = "Solicitar libro";
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
                btnSolicitar.Content = "📥 Solicitar libro";
            }
        }

        // Evento para solicitar libro
        private void btnSolicitar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

        // Botón para volver (cerrar ventana detalle)
        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
