using Android.OS;
using Xamarin.Forms;

[assembly: Dependency(typeof(Battleship.Droid.CloseApplication))]
namespace Battleship.Droid
{

    public class CloseApplication : ICloseApplication
    {
        public void ExitApp()
        {
            Process.KillProcess(Process.MyPid());
        }
    }
}
