using Dapper;
using Dapper32210.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace Dapper32210
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GetAllCaller();
            //Insert(new Player
            //{
            //     Name="Ulvi",
            //      IsStar=false,
            //       Score=25
            //});

        }

        public void Insert(Player player)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;

            using (var connection = new SqlConnection(conn))
            {

                connection.Execute(@"
    INSERT INTO PLayers(Name,Score,IsStar)
    VALUES(@PName,@PScore,@PIsStar)
", new { PName = player.Name, PScore = player.Score, PIsStar = player.IsStar });

                MessageBox.Show("Player Added Successfully");

                GetAllCaller();
            }
        }

        private async void GetAllCaller()
        {
            var players = await GetPlayers();
            myDataGrid.ItemsSource = players;
        }

        private async Task<List<Player>> GetPlayers()
        {
            List<Player> players = new List<Player>();
            var conn = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;

            using (var connection = new SqlConnection(conn))
            {
                var data = await connection.QueryAsync<Player>("SELECT * FROM Players");
                players = data.ToList();
            }
            return players;
        }

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = myDataGrid.SelectedItem as Player;

            if (item != null)
            {
                Name.Text = item.Name;
                Score.Value = item.Score;
                IsStar.IsChecked = item.IsStar;
                var currentPlayer = GetById(item.Id);

                ////MessageBox.Show(currentPlayer.Name);
            }
        }


        public Player GetById(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;

            using (var connection = new SqlConnection(conn))
            {
                var player = connection.QueryFirstOrDefault<Player>("SELECT * FROM Players WHERE Id=@PId",
                    new { PId = id });
                return player;
            }
        }

        public void Update(Player player)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;

            using (var connection = new SqlConnection(conn))
            {
                connection.Execute(@"
            UPDATE Players
            SET Name=@PName,Score=@PScore,IsStar=@PIsStar
            WHERE Id=@PId
", new { PName = player.Name, PScore = player.Score, PIsStar = player.IsStar, PId = player.Id });
            }
            GetAllCaller();
            MessageBox.Show("Updated Succesfully");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var player = myDataGrid.SelectedItem as Player;
            player.Name = Name.Text;
            player.IsStar = IsStar.IsChecked.GetValueOrDefault();
            player.Score = Score.Value;

            Update(player);
        }

        public void Delete(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;

            using (var connection = new SqlConnection(conn))
            {
                connection.Execute(@"DELETE FROM Players
                                    WHERE Id=@PId", new { PId = id });
            }
            GetAllCaller();
        }

        private void myDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var player = myDataGrid.SelectedItem as Player;
            if (player != null)
            {
                Delete(player.Id);
            }
        }
    }
}
