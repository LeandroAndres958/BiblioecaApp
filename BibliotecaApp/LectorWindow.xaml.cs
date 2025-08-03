using MyLibraryApp.Windows.Lector;
using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static BibliotecaAp.AdminWindow;

namespace BibliotecaApp
{
    public partial class LectorWindow : Window
    {
        public LectorWindow()
        {
            InitializeComponent();
            CargarLibrosDesdeBD();
        }

        // Constructor que recibe el nombre de usuario para mostrar saludo
        public LectorWindow(string nombreUsuario) : this()
        {
            txtNombreUsuario.Text = $"Hola, {nombreUsuario}";
        }

        // Carga libros desde la base de datos y los muestra en la interfaz
        private void CargarLibrosDesdeBD()
        {
            if (spNuevosLanzamientos == null || wpLibros == null)
            {
                MessageBox.Show("Los controles no están inicializados todavía.");
                return;
            }

            spNuevosLanzamientos.Children.Clear();
            wpLibros.Children.Clear();

            string categoriaSeleccionada = (cbCategorias.SelectedItem as ComboBoxItem)?.Content.ToString();

            try
            {
                using (var conexion = ConexionBD.ObtenerConexion())
                {
                    // Carga los nuevos lanzamientos (últimos 4 libros)
                    string queryNuevos = "SELECT * FROM libros ORDER BY fecha_lanzamiento DESC LIMIT 4";
                    var cmdNuevos = new MySqlCommand(queryNuevos, conexion);
                    using (var reader = cmdNuevos.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            spNuevosLanzamientos.Children.Add(
                                CrearControlLibroDesdeReader(reader)
                            );
                        }
                    }

                    // Carga todos los libros según la categoría seleccionada o todos si es "Todas"
                    string queryTodos = categoriaSeleccionada == "Todas"
                        ? "SELECT * FROM libros ORDER BY titulo"
                        : "SELECT * FROM libros WHERE categoria = @categoria ORDER BY titulo";

                    var cmdTodos = new MySqlCommand(queryTodos, conexion);
                    if (categoriaSeleccionada != "Todas")
                    {
                        cmdTodos.Parameters.AddWithValue("@categoria", categoriaSeleccionada);
                    }

                    using (var reader = cmdTodos.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            wpLibros.Children.Add(
                                CrearControlLibroDesdeReader(reader)
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar libros: " + ex.Message);
            }
        }

        // Crea visualmente el panel de cada libro con los datos desde el reader
        private Border CrearControlLibroDesdeReader(MySqlDataReader reader)
        {
            int id = Convert.ToInt32(reader["id"]);
            string titulo = reader["titulo"].ToString();
            string autor = reader["autor"].ToString();
            string descripcion = reader["descripcion"].ToString();
            string editorial = reader["editorial"].ToString();
            string paginas = reader["paginas"].ToString();
            string idioma = reader["idioma"].ToString();
            string categoria = reader["categoria"].ToString();
            string año = reader["año"].ToString();
            string estado = "Disponible"; // Aquí puedes agregar lógica para estado real si quieres
            string urlPortada = reader["portada_link"].ToString();


            return CrearControlLibro(
                id,
                titulo, autor, descripcion, editorial,
                paginas, idioma, categoria, año, estado, urlPortada
            );
        }

        // Método que arma el panel visual de cada libro, con botón para ver detalles
        private Border CrearControlLibro(int id, string titulo, string autor, string descripcion,
            string editorial, string paginas, string idioma, string categoria,
            string año, string estado, string urlPortada)
        {
            Border border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Width = 160,
                Margin = new Thickness(8),
                Background = System.Windows.Media.Brushes.White,
                Padding = new Thickness(8),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            StackPanel stack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Stretch
            };

            // Bloque para cargar la imagen portada
            Image img = new Image
            {
                Height = 140,
                Stretch = System.Windows.Media.Stretch.UniformToFill,
                Margin = new Thickness(0, 0, 0, 5)
            };


            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(urlPortada, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                img.Source = bitmap;
            }
            catch
            {
                // Si la imagen no se puede cargar, puedes ignorar o poner una por defecto
            }
            // Aquí la agregas al stack una sola vez
            stack.Children.Add(img);

            TextBlock txtTitulo = new TextBlock
            {
                Text = titulo,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };

            Button btnVerLibro = new Button
            {
                Content = "Ver Libro",
                Background = System.Windows.Media.Brushes.Green,
                Foreground = System.Windows.Media.Brushes.White,
                FontWeight = FontWeights.SemiBold,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Height = 30,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };

            // Evento para abrir ventana de detalles del libro
            btnVerLibro.Click += (s, e) =>
            {
                MainWindow detalleLibro = new MainWindow(
                    id,
                    titulo,
                    autor,
                    descripcion,
                    editorial,
                    paginas,
                    idioma,
                    categoria,
                    año,
                    estado,
                    urlPortada
                );

                // Copiar tamaño, posición y estado de esta ventana
                detalleLibro.Width = this.Width;
                detalleLibro.Height = this.Height;
                detalleLibro.WindowStartupLocation = WindowStartupLocation.Manual;
                detalleLibro.Left = this.Left;
                detalleLibro.Top = this.Top;
                detalleLibro.WindowState = this.WindowState;

                detalleLibro.Show();
            };

            // stack.Children.Add(img);
            stack.Children.Add(txtTitulo);
            stack.Children.Add(btnVerLibro);
            border.Child = stack;

            return border;
        }

        // Cambio en el filtro de categorías
        private void cbCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarLibrosDesdeBD();
        }

        // Botón Inicio (puedes personalizar su función)
        private void btnInicio_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Has presionado INICIO.");
        }

        // Botón Mis Libros: abre ventana MisLibros y oculta esta para no saturar ventanas abiertas
        private void btnMisLibros_Click(object sender, RoutedEventArgs e)
        {
            MisLibrosWindow misLibros = new MisLibrosWindow(this);  // PASAMOS ESTA VENTANA para poder volver luego
            misLibros.Show();
            this.Hide();  // Ocultamos la ventana anteriores
        }
    }
}

