using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;

namespace ClientesApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string connectionString =
            "Data Source=DESKTOP-QH67GH5\\SQLEXPRESS;" +
            "Initial Catalog=ClientesDB;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

        public MainWindow()
        {
            InitializeComponent();
        }

        // BOTÓN 1 - Listar con DataTable
        private void BtnListarDataTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using SqlConnection con = new(connectionString);
                using SqlDataAdapter da = new("SELECT * FROM Clientes", con);
                DataTable dt = new();
                da.Fill(dt);
                dgClientes.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar con DataTable: " + ex.Message);
            }
        }

        // BOTÓN 2 - Listar con DataReader
        private void BtnListarDataReader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Cliente> lista = new();
                using SqlConnection con = new(connectionString);
                con.Open();
                using SqlCommand cmd = new("SELECT * FROM Clientes", con);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Cliente
                    {
                        ClienteID = reader.GetInt32(0),
                        Nombres = reader.GetString(1),
                        Apellidos = reader.GetString(2),
                        DNI = reader.GetString(3)
                    });
                }
                dgClientes.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar con DataReader: " + ex.Message);
            }
        }
    

    // BOTÓN 3 - Buscar por DNI con procedimiento almacenado
        private void BtnBuscarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dni = txtDNI.Text.Trim();
                if (string.IsNullOrEmpty(dni))
                {
                    MessageBox.Show("Ingresa un DNI para buscar.");
                    return;
                }

                using SqlConnection con = new(connectionString);
                con.Open();
                using SqlCommand cmd = new("usp_BuscarClientePorDNI", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DNI", dni);

                using SqlDataReader reader = cmd.ExecuteReader();
                List<Cliente> resultado = new();
                while (reader.Read())
                {
                    resultado.Add(new Cliente
                    {
                        ClienteID = reader.GetInt32(0),
                        Nombres = reader.GetString(1),
                        Apellidos = reader.GetString(2),
                        DNI = reader.GetString(3)
                    });
                }

                if (resultado.Count == 0)
                    MessageBox.Show("Cliente no encontrado.");
                else
                    dgClientes.ItemsSource = resultado;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente: " + ex.Message);
            }
        }
    }
}