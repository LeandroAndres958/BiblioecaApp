using BibliotecaAp;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows;

namespace BibliotecaApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Evento que se ejecuta al dar clic en "Iniciar sesión"
        private void btnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            // Obtener valores ingresados por el usuario
            string correo = txtCorreo.Text.Trim();
            string contraseña = txtContraseña.Password.Trim();

            // Validar que no estén vacíos
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor ingresa correo y contraseña.");
                return;
            }

            // Detectar el tipo de usuario según el dominio del correo
            // Si termina en @admin.com, es admin; si no, lector
            string tipoBusqueda = correo.EndsWith("@admin.com") ? "admin" : "lector";

            try
            {
                // Crear conexión a la base de datos usando la cadena del App.config
                using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString))
                {
                    conexion.Open();

                    // Consulta para buscar usuario con ese correo, contraseña y tipo
                    string query = "SELECT id, tipo FROM usuarios WHERE correo = @correo AND contraseña = @contraseña AND tipo = @tipoBusqueda";

                    using (MySqlCommand comando = new MySqlCommand(query, conexion))
                    {
                        // Agregar parámetros para evitar inyección SQL
                        comando.Parameters.AddWithValue("@correo", correo);
                        comando.Parameters.AddWithValue("@contraseña", contraseña);
                        comando.Parameters.AddWithValue("@tipoBusqueda", tipoBusqueda);

                        // Ejecutar consulta y leer resultados
                        using (MySqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Si encontró usuario, obtener su ID y tipo
                                int idUsuario = reader.GetInt32("id");
                                string tipo = reader.GetString("tipo");

                                // Guardar ID del usuario para usar en otras partes de la app
                                App.Current.Properties["idUsuario"] = idUsuario;

                                // Abrir ventana correspondiente según tipo y cerrar login
                                if (tipo == "lector")
                                {
                                    LectorWindow lector = new LectorWindow();
                                    lector.Show();
                                    this.Close();
                                }
                                else if (tipo == "admin")
                                {
                                    AdminWindow admin = new AdminWindow();
                                    admin.Show();
                                    this.Close();
                                }
                            }
                            else
                            {
                                // Si no encontró usuario, mostrar error
                                MessageBox.Show("Correo o contraseña incorrectos, o tipo de usuario incorrecto.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar cualquier error de conexión o consulta
                MessageBox.Show("Error al conectar a la base de datos: " + ex.Message);
            }
        }

        // Evento que se ejecuta al dar clic en "Crear cuenta"
        private void btnCrearCuenta_Click(object sender, RoutedEventArgs e)
        {
            // Obtener datos ingresados
            string correo = txtCorreo.Text.Trim();
            string contraseña = txtContraseña.Password.Trim();

            // Validar que no estén vacíos
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor ingresa correo y contraseña para crear la cuenta.");
                return;
            }

            try
            {
                // Conexión a la base de datos
                using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString))
                {
                    conexion.Open();

                    // Verificar si el correo ya está registrado
                    string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE correo = @correo";

                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conexion))
                    {
                        checkCmd.Parameters.AddWithValue("@correo", correo);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            // Si ya existe, mostrar mensaje y salir
                            MessageBox.Show("El correo ya está registrado.");
                            return;
                        }
                    }

                    // Insertar nuevo usuario con tipo lector (por defecto)
                    string insertQuery = "INSERT INTO usuarios (nombre, correo, contraseña, tipo) VALUES (@nombre, @correo, @contraseña, 'lector')";

                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conexion))
                    {
                        // Por simplicidad, usamos el correo como nombre
                        insertCmd.Parameters.AddWithValue("@nombre", correo);
                        insertCmd.Parameters.AddWithValue("@correo", correo);
                        insertCmd.Parameters.AddWithValue("@contraseña", contraseña);

                        int result = insertCmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            // Mostrar mensaje de bienvenida
                            MessageBox.Show("Cuenta creada exitosamente. ¡Bienvenido a tu Biblioteca!");

                            // Obtener ID del usuario recién creado
                            string getIdQuery = "SELECT id FROM usuarios WHERE correo = @correo";

                            using (MySqlCommand getIdCmd = new MySqlCommand(getIdQuery, conexion))
                            {
                                getIdCmd.Parameters.AddWithValue("@correo", correo);
                                int idUsuario = Convert.ToInt32(getIdCmd.ExecuteScalar());

                                // Guardar ID para usar en la app
                                App.Current.Properties["idUsuario"] = idUsuario;

                                // Abrir ventana lector y cerrar login
                                LectorWindow lector = new LectorWindow();
                                lector.Show();
                                this.Close();
                            }
                        }
                        else
                        {
                            // Si algo falla al insertar
                            MessageBox.Show("Error al crear la cuenta.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar errores de conexión o SQL
                MessageBox.Show("Error al conectar a la base de datos: " + ex.Message);
            }
        }
    }
}
