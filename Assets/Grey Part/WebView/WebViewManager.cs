using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class WebViewManager : MonoBehaviour
{
    [SerializeField]
    private int _loadSceneId;
    [SerializeField] 
    private string _url;
    [SerializeField] 
    private TMP_Text _text;
    private WebViewObject _webView;
    [SerializeField]
    private bool _isDebug;
    private bool _isWebViewOpen = false;
    private string _externalIpAddress;


    private void Start()
    {
       TryLoadWebView();
    }

    private void TryLoadWebView()
    {
        try
        {
            _text.gameObject.SetActive(_isDebug);
            if (_isDebug)
            {
                StartCoroutine(GetExternalIpAddress());
            }
            StartCoroutine(CheckUrlBeforeLoad());
        }
        catch (Exception e)
        {
            WebViewLog(e.Message);
            LoadMainGame();
        }
    }

    private IEnumerator GetExternalIpAddress()
    {
        // Используем сервис, который возвращает IP в виде простого текста
        string url = "https://api.ipify.org";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Успешно! Текст ответа - это наш IP
                _externalIpAddress = request.downloadHandler.text;
                WebViewLog("Внешний IP-адрес: " + _externalIpAddress);
            }
            else
            {
                WebViewLog("Не удалось получить внешний IP-адрес. Ошибка: " + request.error);
                _externalIpAddress = "Error";
            }
        }
    }


    private IEnumerator CheckUrlBeforeLoad()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_url))
        {
            //request.certificateHandler = new BypassCertificateHandler();
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Linux; Android 10; SM-G975F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.106 Mobile Safari/537.36");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LoadWebView();
            }
            else if (request.responseCode == 404)
            {
                WebViewLog("URL недоступен (404), сразу загружаем игру");
                LoadMainGame();
            }
            else
            {
                WebViewLog($"Ошибка проверки URL {_url}: {request.error} (Код: {request.responseCode})");
                LoadMainGame();
            }
        }
    }

    private void LoadWebView()
    {
        WebViewLog("Load WebView!");
#if UNITY_ANDROID || UNITY_IOS
        _webView = (new GameObject("WebView")).AddComponent<WebViewObject>();
        _webView.Init(
            cb: (msg) =>
            {
                WebViewLog($"Callback: {msg}");
                // Если пользователь закрыл WebView
                if (msg == "close" || msg.Contains("error"))
                {
                    CloseWebView();
                    LoadMainGame();
                }
            },
            err: (msg) =>
            {
                WebViewLog($"Error: {msg}");
                CloseWebView();
                LoadMainGame();
            },
            enableWKWebView: true
        );
        _webView.LoadURL(_url);
        _webView.SetVisibility(true);
        _webView.SetMargins(0, 0, 0, 0); // На весь экран
        _isWebViewOpen = true;
#else
            // Для редактора и других платформ просто открываем в браузере
            Application.OpenURL(_url);
            LoadMainGame();
#endif
    }

    private void LoadMainGame()
    {
        SceneManager.LoadScene(_loadSceneId);
    }

    private void Update()
    {
        // Обработка кнопки "Назад" на Android
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isWebViewOpen)
            {
                if (_webView.CanGoBack())
                {
                    _webView.GoBack();
                }
                //else
                //{
                //    CloseWebView();
                //    LoadMainGame();
                //}
            }
        }
    }

    private void CloseWebView()
    {
        if (_webView != null)
        {
            _webView.SetVisibility(false);
            Destroy(_webView.gameObject);
            _isWebViewOpen = false;
        }
    }

    private void OnDestroy()
    {
        CloseWebView();
    }

    private void WebViewLog(string text)
    {
        if (_isDebug)
        {
            _text.text += $"\n{text}";
        }
        Debug.Log(text);
    }
}


public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Просто возвращаем true, чтобы принять ЛЮБОЙ сертификат.
        // ОПАСНО для продакшена, если вы не контролируете URL!
        return true;
    }
}