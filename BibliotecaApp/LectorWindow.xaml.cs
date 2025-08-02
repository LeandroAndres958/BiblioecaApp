using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BibliotecaApp
{
    public partial class LectorWindow : Window
    {
        public LectorWindow()
        {
            InitializeComponent();
            CargarLibrosDesdeBD();
        }

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
                    // Nuevos lanzamientos
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

                    // Todos los libros
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

        // Crea el panel de libro leyendo todos los campos directamente desde el DataReader
        private Border CrearControlLibroDesdeReader(MySqlDataReader reader)
        {
            string titulo = reader["titulo"].ToString();
            string autor = reader["autor"].ToString();
            string descripcion = reader["descripcion"].ToString();
            string editorial = reader["editorial"].ToString();
            string paginas = reader["paginas"].ToString();
            string idioma = reader["idioma"].ToString();
            string categoria = reader["categoria"].ToString();
            string año = reader["año"].ToString();  // Aquí está el cambio
            string estado = "Disponible"; // No hay columna estado, pon un valor por defecto o crea columna si quieres
            string urlPortada = reader["portada_link"].ToString();

            return CrearControlLibro(
                titulo, autor, descripcion, editorial,
                paginas, idioma, categoria, año, estado, urlPortada
            );
        }


        // Método general para crear el panel visual con botón y portada
        private Border CrearControlLibro(string titulo, string autor, string descripcion,
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
                // Si la imagen no se puede cargar, ignoramos o cargamos una por defecto
            }

            TextBlock txtTitulo = new TextBlock
            {
                Text = titulo,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };

            Button btnSolicitar = new Button
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

            // Evento del botón para abrir MainWindow con los detalles del libro
            btnSolicitar.Click += (s, e) =>
            {
                MainWindow detalleLibro = new MainWindow(
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

                // Copiar tamaño, posición y estado de LectorWindow
                detalleLibro.Width = this.Width;
                detalleLibro.Height = this.Height;
                detalleLibro.WindowStartupLocation = WindowStartupLocation.Manual;
                detalleLibro.Left = this.Left;
                detalleLibro.Top = this.Top;
                detalleLibro.WindowState = this.WindowState;

                detalleLibro.Show();
            };

            stack.Children.Add(img);
            stack.Children.Add(txtTitulo);
            stack.Children.Add(btnSolicitar);
            border.Child = stack;

            return border;
        }

        // Filtro de categorías
        private void cbCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarLibrosDesdeBD();
        }

        private void btnInicio_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Has presionado INICIO.");
        }

        private void btnMisLibros_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Has presionado MIS LIBROS.");
        }
    }
}