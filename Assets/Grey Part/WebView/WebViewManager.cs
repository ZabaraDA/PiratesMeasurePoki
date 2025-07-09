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
    /// ���������� ������ �� ������� ������ ��� ��������� ���������� IP-������.
    /// </summary>
    private IEnumerator GetExternalIpAddress()
    {
        // ���������� ������, ������� ���������� IP � ���� �������� ������
        string url = "https://api.ipify.org";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {

            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // �������! ����� ������ - ��� ��� IP
                ExternalIpAddress = request.downloadHandler.text;
                _text.text = "��� ������� IP-�����: " + ExternalIpAddress;
                Debug.Log("��� ������� IP-�����: " + ExternalIpAddress);

                // �������� ���� �����������, ��� IP �������
                //OnIpAddressResolved?.Invoke(ExternalIpAddress);
            }
            else
            {
                // ��������� ������
                Debug.LogError("�� ������� �������� ������� IP-�����. ������: " + request.error);
                ExternalIpAddress = "Error";
                //OnIpAddressResolved?.Invoke(null); // �������� �� ������
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
                _text.text += "URL ���������� (404), ����� ��������� ����";
                Debug.Log("URL ���������� (404), ����� ��������� ����");
                LoadMainGame();
            }
            else
            {
                _text.text += $"������ �������� URL {_url}: {request.error} (���: {request.responseCode})";
                Debug.LogError($"������: {request.error} (���: {request.responseCode})");
                // ��� ������ ��� ����� ��������� ����
                LoadMainGame();
            }
        }
    }

    //private IEnumerator CheckUrlBeforeLoad()
    //{
    //    string currentUrl = _url;
    //    const int maxRedirects = 10000; // �����������, ����� �� ���� � ����������� ����

    //    for (int i = 0; i < maxRedirects; i++)
    //    {
    //        _text.text += i.ToString();
    //        using (UnityWebRequest request = UnityWebRequest.Get(currentUrl))
    //        {
    //            // ��������� �������������� ���������� ����������
    //            request.redirectLimit = 0;
    //            request.certificateHandler = new BypassCertificateHandler();

    //            yield return request.SendWebRequest();

    //            if (request.responseCode == 404)
    //            {
    //                _text.text += "URL ���������� (404), ����� ��������� ����";
    //                Debug.Log("URL ���������� (404), ����� ��������� ����");
    //                LoadMainGame();
    //                yield break;
    //            }

    //            // ���������, �������� �� ����� ����������
    //            if (request.responseCode == 301 || request.responseCode == 302)
    //            {
    //                string redirectUrl = request.GetResponseHeader("Location");
    //                if (string.IsNullOrEmpty(redirectUrl))
    //                {
    //                    _text.text += "������ ������ ��������, �� �� ������ ����� URL.";
    //                    Debug.LogError("������ ������ ��������, �� �� ������ ����� URL.");
    //                    LoadMainGame();
    //                    yield break; // ������� �� ��������
    //                }

    //                // ����������: ��������� ������������� URL
    //                if (!redirectUrl.StartsWith("http"))
    //                {
    //                    var originalUri = new System.Uri(currentUrl);
    //                    redirectUrl = $"{originalUri.Scheme}://{originalUri.Host}{redirectUrl}";
    //                }
    //                _text.text += $"��������� �������� ��: {redirectUrl}";

    //                Debug.Log($"��������� �������� ��: {redirectUrl}");
    //                currentUrl = redirectUrl; // ��������� URL ��� ��������� �������� �����
    //                continue; // ��������� � ��������� �������� ��� �������� ������ URL
    //            }

    //            // ���� ��� �� ��������, ������������ ��� ������
    //            if (request.result == UnityWebRequest.Result.Success)
    //            {
    //                _text.text += $"�������� URL {currentUrl} �������. ��������� WebView.";
    //                Debug.Log($"�������� URL {currentUrl} �������. ��������� WebView.");
    //                _webViewBackgroundPanel.SetActive(true);
    //                // �����: ��������� � WebView ����� ��������� URL
    //                LoadWebView(currentUrl);
    //            }
    //            else
    //            {
    //                _text.text += $"������ �������� URL {currentUrl}: {request.error} (���: {request.responseCode})";
    //                Debug.LogError($"������ �������� URL {currentUrl}: {request.error} (���: {request.responseCode})");
    //                LoadMainGame();
    //            }
    //            yield break; // ������� �� �������� ����� ��������� ������� ��� ������
    //        }
    //    }
    //    _text.text += "��������� ������������ ���������� ����������.";
    //    Debug.LogError("��������� ������������ ���������� ����������.");
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
                // ���� ������������ ������ WebView
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
        _webView.SetMargins(0, 0, 0, 0); // �� ���� �����
        _isWebViewOpen = true;
#else
            // ��� ��������� � ������ �������� ������ ��������� � ��������
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
        // ��������� ������ "�����" �� Android
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
        // ������ ���������� true, ����� ������� ����� ����������.
        // ������ ��� ����������, ���� �� �� ������������� URL!
        return true;
    }
}