using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPFZOOManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["WPFZOOManager.Properties.Settings.TutorialDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            ShowZoos();
            showallAnimals();
        }

        private void showallAnimals()
        {
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalDataTable = new DataTable();
                    sqlDataAdapter.Fill(animalDataTable);
                    listAllAnimals.DisplayMemberPath = "Name";
                    listAllAnimals.SelectedValuePath = "Id";
                    listAllAnimals.ItemsSource = animalDataTable.DefaultView;
                }
            }
            catch (Exception e)
            {

                MessageBox.Show( e.ToString());
            }
          
        }

        private void ShowZoos()
        {

            try
            {
                string query = "select * from zoo";
                // use this to make a table interface like a image to be usable in c# object.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);

                    // information that will be shown in the list view 
                    listZoos.DisplayMemberPath = "Location";
                    // value that should be deliver 
                    listZoos.SelectedValuePath = "Id";
                    // reference of which table the data show be populate
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ShowAssociatedAnimals()
        {

            try
            {
                string query = "select * from Animal a inner join ZooAnimal "  + 
                    " za on a.Id = za.AnimalId where za.zooId = @ZooId";

                SqlCommand sqlCommand = new SqlCommand( query, sqlConnection);
                // use this to make a table interface like a image to be usable in c# object.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@Zooid", listZoos.SelectedValue);

                    DataTable animalTable = new DataTable();

                    sqlDataAdapter.Fill(animalTable);

                    // information that will be shown in the list view 
                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    // value that should be deliver 
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    // reference of which table the data show be populate
                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
            showSlectedZoointheTexBox();
        }

        private void Delete_Zoo(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = "delete from Zoo where id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception v)
            {
                MessageBox.Show(v.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception v)
            {
                MessageBox.Show(v.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void addAnimaltoZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAllAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception v)
            {
                MessageBox.Show(v.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void showSlectedZoointheTexBox()
        {
            try
            {
                string query = "select Location from Zoo where id = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // use this to make a table interface like a image to be usable in c# object.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@Zooid", listZoos.SelectedValue);

                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);

                    myTextBox.Text = zooTable.Rows[0]["Location"].ToString();
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void showSlectedAnimalintheTexBox()
        {
            try
            {
                string query = "select Name from Animal where id = @AnimalId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                // use this to make a table interface like a image to be usable in c# object.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@AnimalId", listAllAnimals.SelectedValue);

                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);

                    myTextBox.Text = zooTable.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void removeAssociatedAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where id = @Name";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", listAssociatedAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception v)
            {
                MessageBox.Show(v.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void addAssociatedAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception v)
            {
                MessageBox.Show(v.ToString());
            }
            finally
            {
                sqlConnection.Close();
                showallAnimals();
            }
        }

        private void deleteAnamimalFromZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAllAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception v)
            {
                MessageBox.Show(v.ToString());
            }
            finally
            {
                sqlConnection.Close();
                showallAnimals();
            }
        }

        private void listAllAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showSlectedAnimalintheTexBox();
        }

        private void updateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Zoo Set Location = @Location where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception v)
            {
                MessageBox.Show(v.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }
    }
}
