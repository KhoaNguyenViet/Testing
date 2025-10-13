using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipEntry
{
    public string name;       // Tên âm thanh
    public AudioClip clip;    // Đoạn âm thanh
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("Hit Sounds")]
    public List<AudioClipEntry> hitSounds; // Danh sách âm thanh va chạm
    private Dictionary<string, AudioClip> hitSoundDict;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Đảm bảo không bị phá hủy khi chuyển Scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Khởi tạo Dictionaries
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        hitSoundDict = new Dictionary<string, AudioClip>();
        foreach (var sound in hitSounds)
        {
            if (!hitSoundDict.ContainsKey(sound.name))
                hitSoundDict.Add(sound.name, sound.clip);
        }
    }

    #region Hit Sound Methods
    public void PlaySound(string name)
    {
        if (hitSoundDict.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Hit sound '{name}' not found!");
        }
    }

    #endregion
}
