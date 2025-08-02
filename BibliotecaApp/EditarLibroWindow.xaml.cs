using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BibliotecaApp
{
    public partial class EditarLibroWindow : Window
    {
        private string connectionString = "server=localhost;database=biblioteca_db;uid=root;pwd=Leandro18RR;";

        public EditarLibroWindow()
        {
            InitializeComponent();
            CargarLibrosEnComboBox();
        }

        // Carga los libros al comboBox para seleccionar
        private void CargarLibrosEnComboBox()
        {
            cbEditarLibros.Items.Clear();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id, titulo FROM libros";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbEditarLibros.Items.Add(new
                            {
                                Id = reader.GetInt32("id"),
                                Titulo = reader.GetString("titulo")
                            });
                        }
                    }
                }

                cbEditarLibros.DisplayMemberPath = "Titulo";
                cbEditarLibros.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar libros: " + ex.Message);
            }
        }

        // Cuando selecciona un libro, carga sus datos en los TextBoxes
        private void cbEditarLibros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEditarLibros.SelectedItem == null)
                return;

            int idLibro = (int)cbEditarLibros.SelectedValue;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM libros WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", idLibro);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEditTitulo.Text = reader["titulo"].ToString();
                                txtEditAutor.Text = reader["autor"].ToString();
                                txtEditEditorial.Text = reader["editorial"].ToString();
                                txtEditPaginas.Text = reader["paginas"].ToString();
                                txtEditIdioma.Text = reader["idioma"].ToString();
                                txtEditCategoria.Text = reader["categoria"].ToString();
                                txtEditAño.Text = reader["año"].ToString();
                                txtEditDescripcion.Text = reader["descripcion"].ToString();
                                txtEditPortadaLink.Text = reader["portada_link"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos del libro: " + ex.Message);
            }
        }



        // Guardar los cambios en la base de datos
        private void btnGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            if (cbEditarLibros.SelectedItem == null)
            {
                MessageBox.Show("Por favor selecciona un libro para editar.");
                return;
            }

            int idLibro = (int)cbEditarLibros.SelectedValue;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE libros SET 
                            titulo=@titulo, autor=@autor, editorial=@editorial, 
                            paginas=@paginas, idioma=@idioma, categoria=@categoria, 
                            año=@año, descripcion=@descripcion, portada_link=@portada 
                            WHERE id=@id";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@titulo", txtEditTitulo.Text);
                        cmd.Parameters.AddWithValue("@autor", txtEditAutor.Text);
                        cmd.Parameters.AddWithValue("@editorial", txtEditEditorial.Text);
                        cmd.Parameters.AddWithValue("@paginas", int.Parse(txtEditPaginas.Text));
                        cmd.Parameters.AddWithValue("@idioma", txtEditIdioma.Text);
                        cmd.Parameters.AddWithValue("@categoria", txtEditCategoria.Text);
                        cmd.Parameters.AddWithValue("@año", txtEditAño.Text);
                        cmd.Parameters.AddWithValue("@descripcion", txtEditDescripcion.Text);
                        cmd.Parameters.AddWithValue("@portada", txtEditPortadaLink.Text);
                        cmd.Parameters.AddWithValue("@id", idLibro);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Mostrar mensaje sin botón "Aceptar"
                txtMensaje.Visibility = Visibility.Visible;

                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += (s, ev) =>
                {
                    txtMensaje.Visibility = Visibility.Collapsed;
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar cambios: " + ex.Message);
            }
        }

    }
}
