using UnityEngine;
using UnityEngine.UI; // Если хочешь, чтобы сладер тоже был "вечным"

public class BackgroundMusicManager : MonoBehaviour
{
    // Статическая переменная, чтобы убедиться, что у нас только один менеджер музыки
    public static BackgroundMusicManager Instance;

    // Ссылки на AudioSource и Slider (если хотим им тоже управлять из этого скрипта)
    private AudioSource audioSource;

    void Awake()
    {
        // Здесь фокус: проверяем, существует ли уже наш "вечный" менеджер.
        // Если да, то уничтожаем этот новый, чтобы не было дублирования звука.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; // Выходим из метода, чтобы не продолжать инициализацию
        }

        // Если это первый и единственный менеджер, то он становится "Instance"
        Instance = this;

        // Говорим Юнити: "Не трогай меня при загрузке новых сцен!"
        DontDestroyOnLoad(gameObject);

        // Получаем AudioSource на этом же объекте
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Бро, на объекте BackgroundMusicManager нет AudioSource!");
        }

        // Опционально: если ты хочешь, чтобы слайдер тоже был "вечным" и управлял этим звуком,
        // то тебе нужно будет найти его здесь или передать ссылку.
        // Проще всего, если он тоже находится на объекте с DontDestroyOnLoad.
        // Иначе, тебе нужно будет связать его через какой-то другой менеджер.
        // Пока что, будем считать, что слайдер может быть и на обычной сцене.
    }

    // Метод для установки громкости, который можно вызвать из других скриптов
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    // Метод для получения текущей громкости
    public float GetVolume()
    {
        if (audioSource != null)
        {
            return audioSource.volume;
        }
        return 0f; // Возвращаем 0, если AudioSource не найден
    }

    // Если хочешь управлять паузой/воспроизведением
    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
}


