using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BibliotecaApp
{
    public partial class DetalleAdminLibroWindow : Window
    {
        private int _idLibro;

        public DetalleAdminLibroWindow(int idLibro)
        {
            InitializeComponent();

            // Asegura que la ventana aparezca centrada con respecto al padre que es HomeLector
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            _idLibro = idLibro;
            CargarDatosLibro();
        }


        private void CargarDatosLibro()
        {
            try
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT * FROM libros WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _idLibro);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtTitulo.Text = reader["titulo"].ToString();
                                txtAutor.Text = "Autor: " + reader["autor"].ToString();
                                txtEditorial.Text = "Editorial: " + reader["editorial"].ToString();
                                txtPaginas.Text = "Páginas: " + reader["paginas"].ToString();
                                txtIdioma.Text = "Idioma: " + reader["idioma"].ToString();
                                txtCategoria.Text = "Categoría: " + reader["categoria"].ToString();
                                txtAnioPublicacion.Text = "Año: " + reader["año"].ToString();
                                txtDescripcion.Text = reader["descripcion"].ToString();


                                string urlPortada = reader["portada_link"].ToString();

                                if (!string.IsNullOrEmpty(urlPortada))
                                {
                                    try
                                    {
                                        BitmapImage bitmap = new BitmapImage();
                                        bitmap.BeginInit();
                                        bitmap.UriSource = new Uri(urlPortada, UriKind.Absolute);
                                        bitmap.CacheOption = BitmapCacheOption.OnLoad; // Para evitar problemas de caching
                                        bitmap.EndInit();
                                        imgPortada.Source = bitmap;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Error al cargar imagen de portada: " + ex.Message);
                                        // Opcional: asignar imagen por defecto
                                        // imgPortada.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/portada-default.jpg"));
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("El link de portada está vacío.");
                                    // Opcional: asignar imagen por defecto aquí también
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el libro: " + ex.Message);
            }
        }

        // Funcion para Editar libro
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            EditarLibroWindow editar = new EditarLibroWindow();

            // Hacer que la ventana Editar tome la posición, tamaño y estado de la ventana actual
            editar.WindowStartupLocation = WindowStartupLocation.Manual;
            editar.Left = this.Left;
            editar.Top = this.Top;
            editar.Width = this.ActualWidth;
            editar.Height = this.ActualHeight;
            editar.WindowState = this.WindowState;

            // Opcional: poner la ventana detalle como owner para que no se pierda foco
            editar.Owner = this;

            editar.ShowDialog();

            CargarDatosLibro();
        }




        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("¿Estás seguro que quieres eliminar este libro?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        string query = "DELETE FROM libros WHERE id = @id";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", _idLibro);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Libro eliminado correctamente.");
                    this.Close();  // Cierra la ventana tras eliminar
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar libro: " + ex.Message);
                }
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

