using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MMController : MonoBehaviour
{
    public GameObject LoadingWindow;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        LoadingWindow.gameObject.SetActive(false);
    }

    public void OpenGame(int index)
    {
        SceneManager.LoadScene(index);
    }
}
