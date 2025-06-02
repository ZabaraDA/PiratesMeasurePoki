using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject _inputPanel;
    [SerializeField]
    private GameObject _nextPanel;
    [SerializeField]
    private GameObject _inputField;
    [SerializeField]
    private GameObject _outputField;
    [SerializeField]
    private int _currentCount;
    [SerializeField]
    private bool _isExactValue;


    public void Start()
    {
        Switch(false);
    }
    public void Switch(bool isCorrect)
    {
        _inputPanel.gameObject.SetActive(!isCorrect);
        _nextPanel.gameObject.SetActive(isCorrect);
    }

    public void CheckCorrectAndSwitch()
    {
        string inputText = _inputField.GetComponent<TMP_InputField>().text;
        if (int.TryParse(inputText, out int count))
        {
            int minCorrextCount = _currentCount - (_isExactValue ? 0 : _currentCount / 5);
            int maxCorrextCount = _currentCount + (_isExactValue ? 0 : _currentCount / 5);
            if (count >= minCorrextCount && count <= maxCorrextCount)
            {
                Switch(true);
                var outputText =_outputField.GetComponent<TMP_Text>();
                outputText.text = count + " " + outputText.text;
            }
        }
    }
}
