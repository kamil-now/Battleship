using Xamarin.Forms;

[assembly: Dependency(typeof(Battleship.UWP.CloseApplication))]
namespace Battleship.UWP
{
    public class CloseApplication : ICloseApplication
    {
        public void ExitApp()
        {
            Windows.UI.Xaml.Application.Current.Exit();
        }
    }
}
