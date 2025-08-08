using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;

namespace BibliotecaApp
{
    public partial class HomeAdminWindow : Window
    {
        // Cadena de conexión (ajústala con tu servidor, BD, usuario y contraseña)
        private string connectionString = "server=localhost;database=biblioteca_db;uid=root;pwd=Leandro18RR;";

        public HomeAdminWindow()
        {
            InitializeComponent();

            // Cargar libros al abrir ventana
            CargarLibrosDesdeBD();
        }

        // Método para cargar libros desde la base de datos
        private void CargarLibrosDesdeBD()
        {
            wpLibrosAdmin.Children.Clear();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Consulta con los nombres reales de columnas
                    string query = "SELECT id, titulo, portada_link FROM libros";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idLibro = reader.GetInt32("id");
                            string titulo = reader.GetString("titulo");
                            string portadaUrl = reader.GetString("portada_link");

                            // Crear Border para cada libro
                            Border borderLibro = new Border
                            {
                                Width = 160,
                                Height = 240,
                                Margin = new Thickness(5),
                                BorderBrush = Brushes.Gray,
                                BorderThickness = new Thickness(1),
                                CornerRadius = new CornerRadius(5),
                                Cursor = Cursors.Hand,
                                Tag = idLibro
                            };

                            borderLibro.MouseLeftButtonUp += BorderLibro_MouseLeftButtonUp;

                            StackPanel sp = new StackPanel();

                            // Crear imagen con portada
                            Image portada = new Image
                            {
                                Height = 180,
                                Stretch = Stretch.UniformToFill,
                                Margin = new Thickness(5)
                            };

                            try
                            {
                                portada.Source = new BitmapImage(new Uri(portadaUrl, UriKind.RelativeOrAbsolute));
                            }
                            catch
                            {
                                portada.Source = null; // Imagen default o vacía si falla
                            }

                            TextBlock txtTitulo = new TextBlock
                            {
                                Text = titulo,
                                FontWeight = FontWeights.Bold,
                                TextAlignment = TextAlignment.Center,
                                Margin = new Thickness(5, 5, 5, 0)
                            };

                            sp.Children.Add(portada);
                            sp.Children.Add(txtTitulo);
                            borderLibro.Child = sp;

                            wpLibrosAdmin.Children.Add(borderLibro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando libros: " + ex.Message);
            }
        }

        // Evento para abrir ventana detalle admin al hacer click en un libro
        private void BorderLibro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                int idLibro = (int)border.Tag;

                // Abrir ventana detalle admin del libro
                DetalleAdminLibroWindow detalle = new DetalleAdminLibroWindow(idLibro)
                {
                    Owner = this, // ventana padre
                    WindowStartupLocation = WindowStartupLocation.Manual, // Importante para que respete Left y Top
                    WindowState = this.WindowState, // Copia si está maximizada o no
                    Width = this.Width,
                    Height = this.Height,
                    Left = this.Left,
                    Top = this.Top
                };

                detalle.ShowDialog();


                // Recargar la lista después de cerrar el detalle
                CargarLibrosDesdeBD();
            }
        }

        // Evento para abrir ventana insertar libro
        private void btnAgregarLibro_Click(object sender, RoutedEventArgs e)
        {
            InsertarLibroWindow insertar = new InsertarLibroWindow() // abre ;a ventana de agregar libris
            {
                Owner = this,  // Establece ventana padre para control modal
                WindowStartupLocation = WindowStartupLocation.Manual, // Para respetar Left y Top
                WindowState = this.WindowState,  // Copia estado maximizado o normal
                Width = this.Width,              // Copia ancho
                Height = this.Height,            // Copia alto
                Left = this.Left,                // Copia posición X
                Top = this.Top                   // Copia posición Y
            };  
            bool? resultado = insertar.ShowDialog();

            if (resultado == true)
            {
                // Recargar la lista si se insertó un libro nuevo
                CargarLibrosDesdeBD();
            }
        }
    }
}

