using UnityEngine;
using UnityEngine.UI;

public class VolumeControlUI : MonoBehaviour
{
    [SerializeField]
    private Slider volumeSlider; // Сюда перетащи свой UI Slider из Inspector

    void Start()
    {
        if (volumeSlider == null)
        {
            Debug.LogError("VolumeControlUI: Слайдер не назначен в инспекторе, бро!");
            return;
        }

        // Убеждаемся, что наш вечный музыкальный менеджер существует
        if (BackgroundMusicManager.Instance == null)
        {
            Debug.LogError("VolumeControlUI: BackgroundMusicManager не найден на сцене! Убедись, что он существует и использует DontDestroyOnLoad.");
            return;
        }

        // Устанавливаем начальное значение слайдера по текущей громкости вечного звука
        volumeSlider.value = BackgroundMusicManager.Instance.GetVolume();

        // Добавляем слушателя к слайдеру
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float newVolume)
    {
        // Здесь мы вызываем метод SetVolume нашего BackgroundMusicManager
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.SetVolume(newVolume);
        }
    }

    void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }
}


//using UnityEngine;
//using UnityEngine.UI;

//public class VolumeControl : MonoBehaviour
//{
//    [SerializeField]
//    private AudioSource audioSource;

//    [SerializeField]
//    private Slider volumeSlider;

//    void Start()
//    {
//        if (audioSource == null)
//        {
//            audioSource = GetComponent<AudioSource>();
//            if (audioSource == null)
//            {
//                return;
//            }
//        }

//        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

//        volumeSlider.value = audioSource.volume;
//    }

//    void OnVolumeChanged(float newVolume)
//    {
//        audioSource.volume = newVolume;
//    }

//    void OnDestroy()
//    {
//        if (volumeSlider != null)
//        {
//            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
//        }
//    }
//}