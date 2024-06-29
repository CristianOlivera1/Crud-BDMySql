using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BaseDatosMySQL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        } // Establecer la conexión a la base de datos
        static string conexion = "server=localhost;port=3306;database=dbstorage;pwd='';uid=root;";
        MySqlConnection conectado = new MySqlConnection(conexion);

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvTabla.DataSource = llenar();
        }
        public DataTable llenar()
        {
            conectado.Open();
            DataTable dt = new DataTable();
            string llenar = "select * from tperson";
            MySqlCommand cmd = new MySqlCommand(llenar, conectado);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            conectado.Close();
            return dt;
        }
        //REGISTRAR
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            // Expresiones regulares para validación
            string dniPattern = @"^\d{8}$"; // DNI debe tener 8 dígitos
            string namePattern = @"^[a-zA-Zñáéíóú]+(?: [a-zA-Zñáéíóú]+)*$";  // Nombres y apellidos deben contener solo letras

            // Validar campos
            if (!Regex.IsMatch(txtDni.Text, dniPattern))
            {
                MessageBox.Show("DNI no válido. Debe contener 8 dígitos sin caracteres especiales.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(txtNombre.Text, namePattern))
            {
                MessageBox.Show("Nombre no válido. Debe contener solo letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(txtApellido.Text, namePattern))
            {
                MessageBox.Show("Apellido no válido. Debe contener solo letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try { 
                    conectado.Open();

                    // Verificar si el ID ya existe
                    string verificar = "SELECT COUNT(1) FROM tperson WHERE idPerson = @idPerson";
                    using (MySqlCommand cmdVerificar = new MySqlCommand(verificar, conectado))
                    {
                        cmdVerificar.Parameters.AddWithValue("@idPerson", txtId.Text);
                        int count = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("El ID ya existe. No se puede insertar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            conectado.Close();
                            return;
                        }
                    }

                // Insertar un nuevo registro
                string registrar = "INSERT INTO tperson (idPerson, dni, firstName, surName, birthDate) " +
                                   "VALUES (@idPerson, @dni, @firstName, @surName, @birthDate)";
                    using (MySqlCommand cmd = new MySqlCommand(registrar, conectado))
                    {
                        cmd.Parameters.AddWithValue("@idPerson", txtId.Text);
                        cmd.Parameters.AddWithValue("@dni", txtDni.Text);
                        cmd.Parameters.AddWithValue("@firstName", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@surName", txtApellido.Text);
                        cmd.Parameters.AddWithValue("@birthDate", dtpFecha.Value);

                        cmd.ExecuteNonQuery();
                    }

                    conectado.Close();
                    dgvTabla.DataSource = llenar();
                    MessageBox.Show("Los Datos fueron Registrados correctamente");
            }catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Código de error para duplicado de clave única
                {
                    MessageBox.Show("El ID ya existe. No se puede insertar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al insertar el registro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al insertar el registro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conectado.State == ConnectionState.Open)
                {
                    conectado.Close();
                }
            }
        }
        //ACTUALIZAR
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            // Expresiones regulares para validación
            string dniPattern = @"^\d{8}$"; // DNI debe tener 8 dígitos
            string namePattern = @"^[a-zA-Zñáéíóú]+(?: [a-zA-Zñáéíóú]+)*$";  // Nombres y apellidos deben contener solo letras

            // Validar campos
            if (!Regex.IsMatch(txtDni.Text, dniPattern))
            {
                MessageBox.Show("DNI no válido. Debe contener 8 dígitos sin caracteres especiales.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(txtNombre.Text, namePattern))
            {
                MessageBox.Show("Nombre no válido. Debe contener solo letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(txtApellido.Text, namePattern))
            {
                MessageBox.Show("Apellido no válido. Debe contener solo letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            conectado.Open();
            // actualizar un registro
            string actualizar = "update tperson set dni=@dni, firstName = @firstName,surName=@surName,birthDate=@birthDate where idPerson=@idPerson";
            using (MySqlCommand cmd = new MySqlCommand(actualizar, conectado))
            {
                cmd.Parameters.AddWithValue("@idPerson", txtId.Text);
                cmd.Parameters.AddWithValue("@dni", txtDni.Text);
                cmd.Parameters.AddWithValue("@firstName", txtNombre.Text);
                cmd.Parameters.AddWithValue("@surName", txtApellido.Text);
                cmd.Parameters.AddWithValue("@birthDate", dtpFecha.Value);

                cmd.ExecuteNonQuery();
                conectado.Close();

                dgvTabla.DataSource = llenar();
                MessageBox.Show("Su registro fue realizado correctamente.", "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
             {
                 txtId.Text = dgvTabla.CurrentRow.Cells[0].Value.ToString();
                 txtDni.Text = dgvTabla.CurrentRow.Cells[1].Value.ToString();
                 txtNombre.Text = dgvTabla.CurrentRow.Cells[2].Value.ToString();
                 txtApellido.Text = dgvTabla.CurrentRow.Cells[3].Value.ToString();
                 dtpFecha.Text = dgvTabla.CurrentRow.Cells[4].Value.ToString();
             }
             catch
             {
                MessageBox.Show("Error ");

             }

        }
        //ELIMINAR
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            conectado.Open(); // eliminar un registro
            string eliminar = "DELETE FROM tperson WHERE idPerson= @idPerson";
            using (MySqlCommand cmd = new MySqlCommand(eliminar, conectado))
            {
                cmd.Parameters.AddWithValue("@idPerson", txtId.Text);

                cmd.ExecuteNonQuery();

                conectado.Close();

                dgvTabla.DataSource = llenar();
                MessageBox.Show("Datos eliminados Correctamente");
            }
        }
        //CREAR
        private void btnCrear_Click(object sender, EventArgs e)
        {
            txtId.Clear();
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();    
        }

        private void dgvTabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtId.Text = dgvTabla.CurrentRow.Cells[0].Value.ToString();
                txtDni.Text = dgvTabla.CurrentRow.Cells[1].Value.ToString();
                txtNombre.Text = dgvTabla.CurrentRow.Cells[2].Value.ToString();
                txtApellido.Text = dgvTabla.CurrentRow.Cells[3].Value.ToString();
                dtpFecha.Text = dgvTabla.CurrentRow.Cells[4].Value.ToString();
            }
            catch
            {
                MessageBox.Show("Error ");

            }


        }
    }
}
