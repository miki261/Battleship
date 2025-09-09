using Model;

namespace GUI.Services
{
    public class GameService
    {
        public int GridRows { get; private set; } = 10;
        public int GridColumns { get; private set; } = 10;
        public int[] ShipLengths { get; private set; } = new[] { 5, 4, 3, 3, 2 };

        public Fleet ComputerFleet { get; private set; }
        public List<(int row, int col, SquareState state)> PlayerShots { get; } = new();

        public string StatusText { get; private set; } = "Ready to start";
        public bool IsGameOver => ComputerFleet?.Ships.All(ship => ship.Squares.All(square => square.IsHit)) ?? false;

        public GameService()
        {
            // Initialize with a default fleet
            var fleetBuilder = new FleetBuilder(GridRows, GridColumns, ShipLengths);
            ComputerFleet = fleetBuilder.CreateFleet();
        }

        public void NewGame(int rows, int cols, int[] shipLengths)
        {
            GridRows = rows;
            GridColumns = cols;
            ShipLengths = shipLengths;

            var fleetBuilder = new FleetBuilder(GridRows, GridColumns, ShipLengths);
            ComputerFleet = fleetBuilder.CreateFleet();

            PlayerShots.Clear();
            StatusText = "Game started - Click to shoot!";
        }

        public void PlayerShoot(int row, int col)
        {
            if (IsGameOver) return;

            // Check if already shot here
            if (PlayerShots.Any(s => s.row == row && s.col == col))
            {
                StatusText = "Already shot there!";
                return;
            }

            var result = ComputerFleet.Hit(row, col);
            var squareState = result switch
            {
                HitResult.Hit => SquareState.Hit,
                HitResult.Sunken => SquareState.Sunken,
                _ => SquareState.Missed
            };

            PlayerShots.Add((row, col, squareState));

            StatusText = result switch
            {
                HitResult.Hit => $"Hit at ({row},{col})!",
                HitResult.Sunken => $"Ship sunk at ({row},{col})!",
                _ => $"Miss at ({row},{col})"
            };

            if (IsGameOver)
            {
                StatusText = "Congratulations! You won!";
            }
        }
    }
}
