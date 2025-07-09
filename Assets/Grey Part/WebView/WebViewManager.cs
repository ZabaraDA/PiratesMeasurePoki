using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class WebViewManager : MonoBehaviour
{
    [SerializeField] private string _url;
    [SerializeField] private GameObject _webViewBackgroundPanel;
    [SerializeField] private TMP_Text _text;
    private WebViewObject _webView;
    private bool _isWebViewOpen = false;
    public static string ExternalIpAddress { get; private set; }

    private void Start()
    {
        try
        {
            StartCoroutine(GetExternalIpAddress());
            StartCoroutine(CheckUrlBeforeLoad());
        }
        catch
        {
            _text.text += " Catch! ";
            LoadMainGame();
        }
    }

    /// <summary>
    /// Отправляет запрос на внешний сервис для получения публичного IP-адреса.
    /// </summary>
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
                ExternalIpAddress = request.downloadHandler.text;
                _text.text = "Мой внешний IP-адрес: " + ExternalIpAddress;
                Debug.Log("Мой внешний IP-адрес: " + ExternalIpAddress);

                // Сообщаем всем подписчикам, что IP получен
                //OnIpAddressResolved?.Invoke(ExternalIpAddress);
            }
            else
            {
                // Произошла ошибка
                Debug.LogError("Не удалось получить внешний IP-адрес. Ошибка: " + request.error);
                ExternalIpAddress = "Error";
                //OnIpAddressResolved?.Invoke(null); // Сообщаем об ошибке
            }
        }
    }


    private IEnumerator CheckUrlBeforeLoad()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_url))
        {
            request.certificateHandler = new BypassCertificateHandler();
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Linux; Android 10; SM-G975F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.106 Mobile Safari/537.36");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _webViewBackgroundPanel.SetActive(true);
                LoadWebView();
            }
            else if (request.responseCode == 404)
            {
                _text.text += "URL недоступен (404), сразу загружаем игру";
                Debug.Log("URL недоступен (404), сразу загружаем игру");
                LoadMainGame();
            }
            else
            {
                _text.text += $"Ошибка проверки URL {_url}: {request.error} (Код: {request.responseCode})";
                Debug.LogError($"Ошибка: {request.error} (Код: {request.responseCode})");
                // При ошибке все равно загружаем игру
                LoadMainGame();
            }
        }
    }

    //private IEnumerator CheckUrlBeforeLoad()
    //{
    //    string currentUrl = _url;
    //    const int maxRedirects = 10000; // Ограничение, чтобы не уйти в бесконечный цикл

    //    for (int i = 0; i < maxRedirects; i++)
    //    {
    //        _text.text += i.ToString();
    //        using (UnityWebRequest request = UnityWebRequest.Get(currentUrl))
    //        {
    //            // Отключаем автоматическое следование редиректам
    //            request.redirectLimit = 0;
    //            request.certificateHandler = new BypassCertificateHandler();

    //            yield return request.SendWebRequest();

    //            if (request.responseCode == 404)
    //            {
    //                _text.text += "URL недоступен (404), сразу загружаем игру";
    //                Debug.Log("URL недоступен (404), сразу загружаем игру");
    //                LoadMainGame();
    //                yield break;
    //            }

    //            // Проверяем, является ли ответ редиректом
    //            if (request.responseCode == 301 || request.responseCode == 302)
    //            {
    //                string redirectUrl = request.GetResponseHeader("Location");
    //                if (string.IsNullOrEmpty(redirectUrl))
    //                {
    //                    _text.text += "Сервер вернул редирект, но не указал новый URL.";
    //                    Debug.LogError("Сервер вернул редирект, но не указал новый URL.");
    //                    LoadMainGame();
    //                    yield break; // Выходим из корутины
    //                }

    //                // ИСПРАВЛЕНО: Обработка относительных URL
    //                if (!redirectUrl.StartsWith("http"))
    //                {
    //                    var originalUri = new System.Uri(currentUrl);
    //                    redirectUrl = $"{originalUri.Scheme}://{originalUri.Host}{redirectUrl}";
    //                }
    //                _text.text += $"Обнаружен редирект на: {redirectUrl}";

    //                Debug.Log($"Обнаружен редирект на: {redirectUrl}");
    //                currentUrl = redirectUrl; // Обновляем URL для следующей итерации цикла
    //                continue; // Переходим к следующей итерации для проверки нового URL
    //            }

    //            // Если это не редирект, обрабатываем как обычно
    //            if (request.result == UnityWebRequest.Result.Success)
    //            {
    //                _text.text += $"Проверка URL {currentUrl} успешна. Загружаем WebView.";
    //                Debug.Log($"Проверка URL {currentUrl} успешна. Загружаем WebView.");
    //                _webViewBackgroundPanel.SetActive(true);
    //                // ВАЖНО: загружать в WebView нужно финальный URL
    //                LoadWebView(currentUrl);
    //            }
    //            else
    //            {
    //                _text.text += $"Ошибка проверки URL {currentUrl}: {request.error} (Код: {request.responseCode})";
    //                Debug.LogError($"Ошибка проверки URL {currentUrl}: {request.error} (Код: {request.responseCode})");
    //                LoadMainGame();
    //            }
    //            yield break; // Выходим из корутины после успешного запроса или ошибки
    //        }
    //    }
    //    _text.text += "Превышено максимальное количество редиректов.";
    //    Debug.LogError("Превышено максимальное количество редиректов.");
    //    LoadMainGame();
    //}


    private void LoadWebView()
    {
        _text.text += " \nLoad WebView! \n";
#if UNITY_ANDROID || UNITY_IOS
        _webView = (new GameObject("WebView")).AddComponent<WebViewObject>();
        _webView.Init(
            cb: (msg) => {
                _text.text += $"Callback: {msg}";
               Debug.Log($"Callback: {msg}");
                // Если пользователь закрыл WebView
                if (msg == "close" || msg.Contains("error"))
                {
                    CloseWebView();
                    LoadMainGame();
                }
            },
            err: (msg) => {
                _text.text += $"Error: {msg}";
                Debug.Log($"Error: {msg}");
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
        _webViewBackgroundPanel.SetActive(false);
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
                else
                {
                    CloseWebView();
                    LoadMainGame();
                }
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