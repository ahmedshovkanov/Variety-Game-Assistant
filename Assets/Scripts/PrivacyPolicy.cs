using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PrivacyPolicy : MonoBehaviour
{
    public string Link;
    private UniWebView _privacyScreen;

    public void LoadURL()
    {
        if (_privacyScreen == null)
        {
            GameObject viewGO = new GameObject("PrivacyPolicyScreen");
            _privacyScreen = viewGO.AddComponent<UniWebView>();
            _privacyScreen.Frame = Screen.safeArea;
            _privacyScreen.OnPageFinished += OnPageFinished;
            _privacyScreen.OnShouldClose += OnShouldClose;
            _privacyScreen.EmbeddedToolbar.Show();
            _privacyScreen.SetAllowBackForwardNavigationGestures(true);
        }

        _privacyScreen.Load(Link);
        _privacyScreen.Show();
    }

    GameObject _bg;

    private void OnPageFinished(UniWebView view, int statusCode, string url)
    {
        GameObject bg = new GameObject("BG");
        bg.AddComponent<Image>();
        bg.GetComponent<Image>().color = Color.black;
        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 5000);
        bg.transform.SetParent(GameObject.Find("UI").transform, false);
        _bg = bg;
        StartCoroutine(CloseScreen());
    }

    private bool OnShouldClose(UniWebView webView)
    {
        if (webView != null)
            Destroy(webView.gameObject);
        webView = null;
        if (_bg != null)
            Destroy(_bg);
        StopAllCoroutines();
        return true;
    }

    public void CloseWebView()
    {
        if (_privacyScreen != null)
        {
            _privacyScreen.Hide();
            Destroy(_privacyScreen.gameObject);
            _privacyScreen = null;
        }
        if (_bg != null)
            Destroy(_bg);
        StopAllCoroutines();
    }

    IEnumerator CloseScreen()
    {
        Vector3 firstTouchPos, secondTouchPos;
        while (true)
        {
            yield return new WaitWhile(() => !Input.GetMouseButtonDown(0));
            firstTouchPos = Input.mousePosition;
            Debug.Log(firstTouchPos);
            yield return new WaitWhile(() => !Input.GetMouseButtonUp(0));
            secondTouchPos = Input.mousePosition;
            Debug.Log(secondTouchPos);
            if ((secondTouchPos - firstTouchPos).x > 200)
            {
                CloseWebView();
            }
        }
    }
}