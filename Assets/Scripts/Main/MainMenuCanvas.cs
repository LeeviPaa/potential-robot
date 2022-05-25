using UnityEngine.SceneManagement;
using UnityEngine;

namespace PotentialRobot.Main
{
    public class MainMenuCanvas : OverlayCanvas
    {
        public void Button_LoadMultipeen()
        {
            SceneManager.LoadScene("MultipeenLobby");
        }

        public void Button_Quit()
        {
            Application.Quit();
        }
    }
}
