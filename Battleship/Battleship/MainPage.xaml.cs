using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Battleship
{
    public partial class MainPage : ContentPage
    {
        public GamePanel PlayerGamePanel { get; set; }
        public GamePanel EnemyGamePanel { get; set; }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            Device.BeginInvokeOnMainThread(() => DisplayAlert("Battleship",
                           "\n\tBLUE = MISS"
                         + "\n\tRED = HIT"
                         + "\n\tDARK RED = DESTROYED"
                         + "\n\tBLACK = YOUR SHIPS", "PLAY")
                         .ContinueWith((x) => Device.BeginInvokeOnMainThread(() => StartNewGame(false))));

        }
        public void StartNewGame(bool random = false)
        {
            MainGrid.Children.Clear();
            EnemyGrid.Children.Clear();
            EnemyGrid.IsEnabled = random;
            MainGrid.IsEnabled = !random;

            var ships = new List<Ship>()
            {
                new Ship(5),
                new Ship(4),
                new Ship(3),
                new Ship(3),
                new Ship(2),
            };

            PlayerGamePanel = new GamePanel(MainGrid, 10, "PLAYER");

            PlayerGamePanel.PlaceShips(ships, random);

            PlayerGamePanel.ShipsVisible = true;
            if (!random)
            {
                Device.BeginInvokeOnMainThread(() => DisplayAlert("Battleship",
                           "\n\tPLACE YOUR SHIPS"
                         + "\n\tYOUR SHIPS SIZE:"
                         + "\n\t5, 4, 3, 3, 2", "OK"));
            }
            PlayerGamePanel.StartGameAction = () =>
            {
                MainGrid.IsEnabled = false;
                EnemyGrid.IsEnabled = true;
                PlayerGamePanel.ShipsVisible = true;
                EnemyGamePanel.Buttons.ForEach(n => n.Text = "");
            };
            PlayerGamePanel.GameOverAction += () => GameLost();

            EnemyGamePanel = new GamePanel(EnemyGrid, 10, "ENEMY");

            EnemyGamePanel.PlaceShips(ships, true);
            EnemyGamePanel.ShipsVisible = false;
            EnemyGamePanel.GameOverAction += () => GameWon();
            EnemyGamePanel.HitAction = () => PlayerGamePanel.HitRandom();
            OnPropertyChanged("PlayerGamePanel");
            OnPropertyChanged("AIGamePanel");
        }
        private async Task GameWon()
        {
            var result = await DisplayAlert("Battleship", "YOU WON!", "PLAY AGAIN", "EXIT");
            if (result == true)
                Device.BeginInvokeOnMainThread(() => StartNewGame());
            else
                DependencyService.Get<ICloseApplication>()?.ExitApp();

        }
        private async Task GameLost()
        {
            var result = await DisplayAlert("Battleship", "YOU LOST!", "PLAY AGAIN", "EXIT");
            if (result == true)
                Device.BeginInvokeOnMainThread(() => StartNewGame());
            else
                DependencyService.Get<ICloseApplication>()?.ExitApp();
        }
    }
}
