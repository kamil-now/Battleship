
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Battleship
{
    public class GamePanel : PropertyChangedBase
    {
        int size;
        Grid grid;
        Ship currentShip;
        List<Ship> ships = new List<Ship>();
        public bool shipsVisible;

        public Action StartGameAction { get; set; }
        public Action HitAction { get; set; }
        public Action GameOverAction { get; set; }
        public IEnumerable<Button> Buttons => grid.Children.OfType<Button>();

        public List<Point> HitPoints => ships.SelectMany(n => n.HitPoints).ToList();
        public List<Point> MissPoints = new List<Point>();

        public bool Initialized => ships.All(n => n.Initialized);

        public bool ShipsVisible
        {
            get { return shipsVisible; }
            set
            {
                shipsVisible = value;
                if (value)
                {
                    ships.ForEach(ship =>
                                    {
                                        ship.Points.Except(ship.HitPoints).ForEach(p =>
                                        {
                                            var btn = Buttons.First(b => p == b.Coordinates());
                                            btn.BackgroundColor = Color.Black;
                                        });
                                    });
                }
                else
                {
                    ships.ForEach(ship =>
                    {
                        ship.Points.Except(ship.HitPoints).ForEach(p =>
                        {
                            var btn = Buttons.First(b => p == b.Coordinates());
                            btn.BackgroundColor = Color.White;
                        });
                    });
                }
            }
        }


        public string PlayerName { get; }
        public GamePanel(Grid grid, int size, string playerName)
        {
            PlayerName = playerName;
            this.grid = grid;
            this.size = size;
            InitializeGrid(grid, size + 1);
        }
        public void Hit(object btn, EventArgs args)
        {
            Hit((btn as Button).Coordinates());
            HitAction?.Invoke();
        }
        public void Hit(Point point)
        {
            var ship = ships.FirstOrDefault(n => n.Points.Any(p => p == point));
            if (ship == null)
            {
                MissPoints.Add(point);
                var btn = Buttons.First(n => n.Coordinates() == point);
                btn.IsEnabled = false;
                btn.BackgroundColor = Color.DarkBlue;
                btn.BorderColor = Color.DarkBlue;
            }
            else
            {
                ship.HitPoints.Add(point);
                if (ship.Sank)
                {
                    var btns = new List<Button>();
                    ship.Points.ForEach(n => btns.Add(Buttons.First(b => b.Coordinates() == n)));
                    btns.ForEach(b =>
                    {
                        b.IsEnabled = false;
                        b.BackgroundColor = Color.DarkRed;

                        b.FontAttributes = FontAttributes.Bold;
                    });
                }
                else
                {
                    var btn = Buttons.First(b => b.Coordinates() == point);
                    btn.IsEnabled = false;
                    btn.BackgroundColor = Color.Red;
                }
            }
            if (ships.All(n => n.Sank))
            {
                GameOverAction.Invoke();
            }
        }
        public void PlaceShips(IEnumerable<Ship> newShips, bool random = false)
        {
            newShips.ForEach(n => ships.Add(new Ship(n.Size)));
            if (!Initialized)
            {
                currentShip = ships.First(n => !n.Initialized);
            }
            if (random)
            {
                SetRandom();
            }

        }
        public void HitRandom()
        {
            Random random = new Random();
            Button button;
            Point randomPoint;

            do
            {
                randomPoint = new Point(random.Next(1, size + 1), random.Next(1, size + 1));
                button = Buttons.First(n => n.Coordinates() == randomPoint);

            } while (!button.IsEnabled);

            Hit(randomPoint);
        }
        void SetRandom()
        {
            Random random = new Random();
            while (!Initialized)
            {
                var randomPoint = new Point(random.Next(1, size + 1), random.Next(1, size + 1));
                var button = Buttons.First(n => n.Coordinates() == randomPoint);
                if (button.IsEnabled)
                {
                    button.BackgroundColor = Color.Black;
                    button.IsEnabled = false;
                    SetShip(randomPoint);
                }
            }

        }
        void SetShip(object btn, EventArgs args)
        {
            var button = btn as Button;
            button.BackgroundColor = Color.Black;
            button.IsEnabled = false;

            SetShip(button.Coordinates());
        }

        void SetShip(Point point)
        {
            currentShip.AddPoint(point);
            SetEnabledPoints(currentShip);

            if (currentShip.Initialized)
            {
                if (currentShip.IsVertical)
                {
                    var top = Buttons.First(n => n.Coordinates() == currentShip.Top);
                    var bottom = Buttons.First(n => n.Coordinates() == currentShip.Bottom);
                    top.Text = "⎯"; 
                    bottom.Text = "⎯";
                }
                else
                {
                    var left = Buttons.First(n => n.Coordinates() == currentShip.Left);
                    var right = Buttons.First(n => n.Coordinates() == currentShip.Right);
                    left.Text = "|";
                    right.Text = "|";
                }

                currentShip = ships.FirstOrDefault(n => !n.Initialized);
                if (currentShip == null)
                {
                    Buttons.ForEach(n =>
                    {
                        n.IsEnabled = true;
                        n.Clicked -= SetShip;
                        n.Clicked += Hit;
                        n.BackgroundColor = Color.White;
                    });
                    NotifyOfPropertyChange(() => Initialized);
                    StartGameAction?.Invoke();
                }
                else
                    EnableEmpty();
            }


        }
        void SetEnabledPoints(Ship ship)
        {
            Buttons.ForEach(btn =>
            {
                var btnPoint = btn.Coordinates();
                if (ship.Points.Count() == 1)
                {
                    var point = ship.Points.First();
                    btn.IsEnabled = btnPoint.IsAbove(point)
                                 || btnPoint.IsLeftTo(point)
                                 || btnPoint.IsRightTo(point)
                                 || btnPoint.IsUnder(point);
                }
                else if (ship.IsVertical)
                    btn.IsEnabled = btnPoint.IsUnder(ship.Bottom) || btnPoint.IsAbove(ship.Top);
                else
                    btn.IsEnabled = btnPoint.IsLeftTo(ship.Left) || btnPoint.IsRightTo(ship.Right);
            });

        }
        void EnableEmpty()
        {
            Buttons.ForEach(btn =>
            {
                var btnPoint = new Point(Grid.GetColumn(btn), Grid.GetRow(btn));
                btn.IsEnabled = !ships.Any(n => n.Points.Any(p => p == btnPoint));
            });
        }
        void InitializeGrid(Grid grid, int size)
        {
            var letter = 'A';
            for (int row = 0; row < size; row++)
            {

                for (int column = 0; column < size; column++)
                {
                    if (row == 0 && column == 0)
                    {
                        continue;
                    }
                    if (row >= 1 && column == 0)
                    {
                        grid.Children.Add(
                        new Label
                        {
                            Text = (letter++).ToString(),
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.End,
                            FontAttributes = FontAttributes.Bold
                        }, row, column);
                    }
                    else if (row == 0 && column >= 1)
                    {
                        grid.Children.Add(
                        new Label
                        {
                            Text = column.ToString(),
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.Center,
                            FontAttributes = FontAttributes.Bold
                        }, row, column);
                    }
                    else
                    {
                        var button = new Button
                        {

                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Fill,
                            BackgroundColor = Color.White,
                            BorderColor = Color.Black,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Color.White,
                            BorderWidth = 2,
                            Margin = 0
                        };
                        button.Clicked += SetShip;
                        Grid.SetRow(button, row);
                        Grid.SetColumn(button, column);
                        grid.Children.Add(button);
                    }

                }
            }
        }
    }
}
