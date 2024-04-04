using UnityEngine;

public class ImportSounds : MonoBehaviour
{
    [SerializeField] private string folder_path = "Sounds/snow_footsteps";
    private Object[] sounds;

    void Start()
    {
        sounds = Resources.LoadAll(folder_path, typeof(AudioClip));
    }

    public AudioClip get_random() {
        return (AudioClip) sounds[Random.Range(0, sounds.Length)];
    }
}