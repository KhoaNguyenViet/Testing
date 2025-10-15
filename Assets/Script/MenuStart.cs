using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour
{
   public void OnStartClick()
    {
        SceneManager.LoadScene("Level1");
    }
}
