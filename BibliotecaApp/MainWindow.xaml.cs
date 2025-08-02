using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BibliotecaApp
{
    public partial class MainWindow : Window
    {
        // Variables para almacenar la información real del libro
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

        // Constructor que recibe todos los datos del libro
        public MainWindow(
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

            // Asignamos los valores recibidos a las variables internas
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

            // Llamamos al método que llenará los datos en pantalla
            MostrarDetallesDelLibro();
        }

        // Método para mostrar todos los datos en los controles visuales
        private void MostrarDetallesDelLibro()
        {
            // Mostramos los datos en cada TextBlock/TextBox correspondiente
            txtTitulo.Text = titulo;
            txtAutor.Text = autor;
            txtDescripcion.Text = descripcion;

            txtEditorial.Text = $"Editorial: {editorial}";
            txtPaginas.Text = $"Páginas: {paginas}";
            txtIdioma.Text = $"Idioma: {idioma}";
            txtCategoria.Text = $"Categoría: {categoria}";
            txtAnio.Text = $"Año de publicación: {anio}";
            txtEstado.Text = $"Estado: {estado}";

            // Cargamos la imagen desde la URL (puede ser un enlace de Google Drive directo)
            try
            {
                imgPortada.Source = new BitmapImage(new Uri(portadaUrl));
            }
            catch (Exception ex)
            {
                // Si no se puede cargar la imagen, puedes mostrar una imagen de respaldo
                MessageBox.Show("No se pudo cargar la portada del libro. Revisa el enlace.\n" + ex.Message);
            }
        }
    }
}
