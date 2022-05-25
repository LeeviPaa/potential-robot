using UnityEngine;
using UnityEngine.SceneManagement;

namespace PotentialRobot.Main
{
    public class Init : MonoBehaviour
    {
        [SerializeField]
        private string _firstScene;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SceneManager.LoadScene(_firstScene);
        }
    }
}

