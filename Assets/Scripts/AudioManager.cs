using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public AudioSource audioBackground;
    public List<AudioSource> audioList = new List<AudioSource>();
    // Start is called before the first frame update
    void Start()
    {
        audioBackground.Play();
    }
}
