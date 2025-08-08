using BibliotecaApp;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BibliotecaAp
{
    public partial class AdminWindow : Window
    {
        private ObservableCollection<Libro> libros = new ObservableCollection<Libro>();

        public AdminWindow()
        {
            InitializeComponent();

            cbEditarLibros.ItemsSource = libros;
            cbEliminarLibros.ItemsSource = libros;
            cbEditarLibros.DisplayMemberPath = "Titulo";
            cbEliminarLibros.DisplayMemberPath = "Titulo";

            CargarLibros();
        }

        public class Libro
        {
            public int Id { get; set; }  // <-- ID agregado para identificar en BD
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public string Editorial { get; set; }
            public string Paginas { get; set; }
            public string Idioma { get; set; }
            public string Categoria { get; set; }
            public string Año { get; set; }
            public string Descripcion { get; set; }
            public string PortadaLink { get; set; }
            public string PdfLink { get; set; }

        }

        private void btnInsertarLibro_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCamposInsertar())
            {
                using (var conexion = ConexionBD.ObtenerConexion())
                {
                    // Parametros importantes
                    string query = "INSERT INTO libros (titulo, autor, editorial, paginas, idioma, categoria, año, descripcion, portada_link, pdf_link, fecha_lanzamiento) " +
                                  "VALUES (@titulo, @autor, @editorial, @paginas, @idioma, @categoria, @año, @descripcion, @portada_link, @pdf_link, @fecha_lanzamiento)";


                    MySqlCommand cmd = new MySqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@titulo", txtInsertTitulo.Text);
                    cmd.Parameters.AddWithValue("@autor", txtInsertAutor.Text);
                    cmd.Parameters.AddWithValue("@editorial", txtInsertEditorial.Text);
                    cmd.Parameters.AddWithValue("@paginas", txtInsertPaginas.Text);
                    cmd.Parameters.AddWithValue("@idioma", txtInsertIdioma.Text);
                    cmd.Parameters.AddWithValue("@categoria", txtInsertCategoria.Text);
                    cmd.Parameters.AddWithValue("@año", txtInsertAño.Text);
                    cmd.Parameters.AddWithValue("@descripcion", txtInsertDescripcion.Text);
                    cmd.Parameters.AddWithValue("@portada_link", txtInsertPortadaLink.Text);
                    cmd.Parameters.AddWithValue("@pdf_link", txtInsertPdfLink.Text);
                    // Fechas al insertar libros
                    cmd.Parameters.AddWithValue("@fecha_lanzamiento", DateTime.Now.Date);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("📚 Libro insertado correctamente en la base de datos.");
                }

                LimpiarCamposInsertar();
                CargarLibros();
            }
            else
            {
                MessageBox.Show("⚠️ Completa todos los campos.");
            }
        }

        private void CargarLibros()
        {
            libros.Clear();

            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT * FROM libros";
                MySqlCommand cmd = new MySqlCommand(query, conexion);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        libros.Add(new Libro
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Titulo = reader["titulo"].ToString(),
                            Autor = reader["autor"].ToString(),
                            Editorial = reader["editorial"].ToString(),
                            Paginas = reader["paginas"].ToString(),
                            Idioma = reader["idioma"].ToString(),
                            Categoria = reader["categoria"].ToString(),
                            Año = reader["año"].ToString(),
                            Descripcion = reader["descripcion"].ToString(),
                            PortadaLink = reader["portada_link"].ToString(),
                            PdfLink = reader["pdf_link"].ToString()

                        });
                    }
                }
            }

            cbEditarLibros.Items.Refresh();
            cbEliminarLibros.Items.Refresh();
        }

        private bool ValidarCamposInsertar()
        {
            return !string.IsNullOrWhiteSpace(txtInsertTitulo.Text) &&
                   !string.IsNullOrWhiteSpace(txtInsertAutor.Text) &&
                   !string.IsNullOrWhiteSpace(txtInsertEditorial.Text) &&
                   !string.IsNullOrWhiteSpace(txtInsertPaginas.Text) &&
                   !string.IsNullOrWhiteSpace(txtInsertIdioma.Text) &&
                   !string.IsNullOrWhiteSpace(txtInsertCategoria.Text) &&
                   !string.IsNullOrWhiteSpace(txtInsertAño.Text) &&
                   !string.IsNullOrWhiteSpace(txtInsertDescripcion.Text) &&
                   !string.IsNullOrWhiteSpace(txtInsertPortadaLink.Text)&&
                   !string.IsNullOrWhiteSpace(txtInsertPdfLink.Text); ;
        }

        private void LimpiarCamposInsertar()
        {
            txtInsertTitulo.Clear();
            txtInsertAutor.Clear();
            txtInsertEditorial.Clear();
            txtInsertPaginas.Clear();
            txtInsertIdioma.Clear();
            txtInsertCategoria.Clear();
            txtInsertAño.Clear();
            txtInsertDescripcion.Clear();
            txtInsertPortadaLink.Clear();
            txtInsertPdfLink.Clear();  

        }

        private void cbEditarLibros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Libro seleccionado = cbEditarLibros.SelectedItem as Libro;
            if (seleccionado != null)
            {
                txtEditTitulo.Text = seleccionado.Titulo;
                txtEditAutor.Text = seleccionado.Autor;
                txtEditEditorial.Text = seleccionado.Editorial;
                txtEditPaginas.Text = seleccionado.Paginas;
                txtEditIdioma.Text = seleccionado.Idioma;
                txtEditCategoria.Text = seleccionado.Categoria;
                txtEditAño.Text = seleccionado.Año;
                txtEditDescripcion.Text = seleccionado.Descripcion;
                txtEditPortadaLink.Text = seleccionado.PortadaLink;

            }
        }

        private void btnGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            Libro seleccionado = cbEditarLibros.SelectedItem as Libro;
            if (seleccionado != null && ValidarCamposEditar())
            {
                seleccionado.Titulo = txtEditTitulo.Text;
                seleccionado.Autor = txtEditAutor.Text;
                seleccionado.Editorial = txtEditEditorial.Text;
                seleccionado.Paginas = txtEditPaginas.Text;
                seleccionado.Idioma = txtEditIdioma.Text;
                seleccionado.Categoria = txtEditCategoria.Text;
                seleccionado.Año = txtEditAño.Text;
                seleccionado.Descripcion = txtEditDescripcion.Text;
                seleccionado.PortadaLink = txtEditPortadaLink.Text;

                MessageBox.Show("✅ Libro actualizado (en memoria).");
                cbEditarLibros.Items.Refresh();
                cbEliminarLibros.Items.Refresh();
            }
            else
            {
                MessageBox.Show("⚠️ Por favor, completa todos los campos.");
            }
        }

        private bool ValidarCamposEditar()
        {
            return !string.IsNullOrWhiteSpace(txtEditTitulo.Text) &&
                   !string.IsNullOrWhiteSpace(txtEditAutor.Text) &&
                   !string.IsNullOrWhiteSpace(txtEditEditorial.Text) &&
                   !string.IsNullOrWhiteSpace(txtEditPaginas.Text) &&
                   !string.IsNullOrWhiteSpace(txtEditIdioma.Text) &&
                   !string.IsNullOrWhiteSpace(txtEditCategoria.Text) &&
                   !string.IsNullOrWhiteSpace(txtEditAño.Text) &&
                   !string.IsNullOrWhiteSpace(txtEditDescripcion.Text) &&
                   !string.IsNullOrWhiteSpace(txtEditPortadaLink.Text);
        }

        private void LimpiarCamposEditar()
        {
            txtEditTitulo.Clear();
            txtEditAutor.Clear();
            txtEditEditorial.Clear();
            txtEditPaginas.Clear();
            txtEditIdioma.Clear();
            txtEditCategoria.Clear();
            txtEditAño.Clear();
            txtEditDescripcion.Clear();
            txtEditPortadaLink.Clear();
        }

        private void btnEliminarLibro_Click(object sender, RoutedEventArgs e)
        {
            Libro seleccionado = cbEliminarLibros.SelectedItem as Libro;
            if (seleccionado != null)
            {
                MessageBoxResult res = MessageBox.Show($"¿Seguro que quieres eliminar '{seleccionado.Titulo}'?", "Confirmar eliminación", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conexion = ConexionBD.ObtenerConexion())
                        {
                            string query = "DELETE FROM libros WHERE id = @id";
                            MySqlCommand cmd = new MySqlCommand(query, conexion);
                            cmd.Parameters.AddWithValue("@id", seleccionado.Id);

                            int filasAfectadas = cmd.ExecuteNonQuery();

                            if (filasAfectadas > 0)
                            {
                                MessageBox.Show("🗑️ Libro eliminado correctamente de la base de datos.");

                                libros.Remove(seleccionado);

                                if (cbEditarLibros.SelectedItem == seleccionado)
                                {
                                    cbEditarLibros.SelectedIndex = -1;
                                    LimpiarCamposEditar();
                                }

                                cbEditarLibros.Items.Refresh();
                                cbEliminarLibros.Items.Refresh();
                            }
                            else
                            {
                                MessageBox.Show("⚠️ No se encontró el libro en la base de datos.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar libro: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("⚠️ Selecciona un libro para eliminar.");
            }
        }

        private void txtInsertEditorial_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
