using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip[] musics;
    [SerializeField] private AudioClip[] pathwayAudios;
    [SerializeField] private AudioClip uiMenuSound;

    private AudioSource musicSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private List<AudioSource> spatialSources = new List<AudioSource>();

    private bool mute;
    private float volume = .5f;

    private const int InitialPoolSize = 10;
    private const int InitialSpatialPoolSize = 5;

    private List<List<int>>[] harmonicMusics;
    private List<int>[] flattenedHarmonics;

    private List<int> notes;

    private int currentNormalMusicIndex = 0;
    private int currentNormalNoteIndex = 0;
    private int currentSpatialMusicIndex = 0;
    private int currentSpatialNoteIndex = 0;

    private void Start()
    {
        //selecting one of the melodies randomly
        notes = flattenedHarmonics[Random.Range(0, (flattenedHarmonics.Length - 1))];
        //Debug.Log("flattenedHarmonics.Length is:  " + notes.Count);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        // Initialize SFX pool
        for (int i = 0; i < InitialPoolSize; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.loop = false;
            sfxSources.Add(src);
        }

        // Initialize spatial SFX pool
        for (int i = 0; i < InitialSpatialPoolSize; i++)
        {
            GameObject spatialGO = new GameObject("SpatialSource_" + i);
            spatialGO.transform.parent = transform;
            AudioSource src = spatialGO.AddComponent<AudioSource>();
            src.loop = false;
            src.spatialBlend = 1f; // Full 3D spatial audio
            spatialSources.Add(src);
        }

        // Load settings
        //mute = PlayerPrefs.GetInt("Mute", 0) == 1;
        //volume = PlayerPrefs.GetFloat("Volume", 1f);
        mute = false;
        volume = 1f;
        UpdateVolumes();

        // Initialize the 3 harmonic musics as sequences of chords (each chord is a list of note indices from pathwayAudios)
        harmonicMusics = new List<List<int>>[3];
        harmonicMusics[0] = new List<List<int>>
        {
            new List<int> {0, 4, 7}, // C major (C E G)
            new List<int> {5, 9, 1}, // F major (F A C')
            new List<int> {7, 10, 2}, // G major (G B D)
            new List<int> {0, 4, 7}  // C major
        };
        harmonicMusics[1] = new List<List<int>>
        {
            new List<int> {9, 1, 4}, // A minor (A C' E)
            new List<int> {5, 9, 1}, // F major
            new List<int> {0, 4, 7}, // C major
            new List<int> {7, 10, 2} // G major
        };
        harmonicMusics[2] = new List<List<int>>
        {
            new List<int> {0, 4, 7}, // C major
            new List<int> {9, 1, 4}, // A minor
            new List<int> {5, 9, 1}, // F major
            new List<int> {7, 10, 2} // G major
        };

        // Flatten the harmonic musics into sequences of individual notes
        flattenedHarmonics = new List<int>[3];
        for (int m = 0; m < 3; m++)
        {
            flattenedHarmonics[m] = new List<int>();
            foreach (var chord in harmonicMusics[m])
            {
                flattenedHarmonics[m].AddRange(chord);
            }
        }
    }

    public bool Mute
    {
        get => mute;
        set
        {
            mute = value;
            PlayerPrefs.SetInt("Mute", value ? 1 : 0);
            if (mute)
            {
                StopAllSounds();
            }
        }
    }

    public float Volume
    {
        get => volume;
        set
        {
            volume = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat("Volume", volume);
            UpdateVolumes();
        }
    }

    // Updates the volume for all audio sources managed by this class
    private void UpdateVolumes()
    {
        musicSource.volume = volume;
        foreach (var src in sfxSources)
        {
            src.volume = volume;
        }
        foreach (var src in spatialSources)
        {
            src.volume = volume;
        }
    }

    // Stops all sounds including music, SFX, and spatial audio
    private void StopAllSounds()
    {
        musicSource.Stop();
        foreach (var src in sfxSources)
        {
            src.Stop();
        }
        foreach (var src in spatialSources)
        {
            src.Stop();
        }
    }

    // Plays the specified music clip, replacing any currently playing music
    public void PlayMusic(AudioClip clip)
    {
        if (mute || clip == null) return;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void PlayMusicByIndex(int index)
    {
        if (index < 0 || index >= musics.Length) return;
        PlayMusic(musics[index]);
    }

    // Plays a non-spatial SFX clip, optionally stopping all other non-spatial SFX first
    public void PlaySFX(AudioClip clip, bool stopPrevious = false)
    {
        if (mute || clip == null) return;

        if (stopPrevious)
        {
            foreach (var src in sfxSources)
            {
                if (src.isPlaying) src.Stop();
            }
        }

        AudioSource freeSource = sfxSources.Find(src => !src.isPlaying);
        if (freeSource == null)
        {
            freeSource = gameObject.AddComponent<AudioSource>();
            freeSource.loop = false;
            sfxSources.Add(freeSource);
        }

        freeSource.clip = clip;
        freeSource.volume = volume;
        freeSource.Play();
    }

    public void PlayPathwayAudioByIndex(int index, bool stopPrevious = false)
    {
        if (index < 0 || index >= pathwayAudios.Length) return;
        PlaySFX(pathwayAudios[index], stopPrevious);
    }

    public void PlayUIMenuSound(bool stopPrevious = false)
    {
        PlaySFX(uiMenuSound, stopPrevious);
    }

    // Plays a spatial SFX clip at the specified position without stopping other sounds
    public void PlaySpatial(AudioClip clip, Vector3 position)
    {
        if (mute || clip == null) return;

        AudioSource freeSource = spatialSources.Find(src => !src.isPlaying);
        if (freeSource == null)
        {
            GameObject spatialGO = new GameObject("SpatialSource_Dynamic");
            spatialGO.transform.parent = transform;
            freeSource = spatialGO.AddComponent<AudioSource>();
            freeSource.loop = false;
            freeSource.spatialBlend = 1f;
            spatialSources.Add(freeSource);
        }

        freeSource.transform.position = position;
        freeSource.clip = clip;
        freeSource.volume = volume;
        freeSource.Play();
    }

    // Plays a spatial SFX clip at the specified position, stopping all other SFX (non-spatial and spatial) first
    public void PlaySpatialWithStop(AudioClip clip, Vector3 position)
    {
        if (mute || clip == null) return;

        StopAllSFX();
        StopAllSpatial();

        AudioSource freeSource = spatialSources.Find(src => !src.isPlaying);
        if (freeSource == null)
        {
            GameObject spatialGO = new GameObject("SpatialSource_Dynamic");
            spatialGO.transform.parent = transform;
            freeSource = spatialGO.AddComponent<AudioSource>();
            freeSource.loop = false;
            freeSource.spatialBlend = 1f;
            spatialSources.Add(freeSource);
        }

        freeSource.transform.position = position;
        freeSource.clip = clip;
        freeSource.volume = volume;
        freeSource.Play();
    }

    // Plays the next note in the non-spatial harmonic melody sequence and advances to the next
    public void PlayHarmonicMelodies()
    {
        if (mute) return;



        if (currentNormalNoteIndex >= notes.Count)
        {
            currentNormalNoteIndex = 0;
            currentNormalMusicIndex = (currentNormalMusicIndex + 1) % 3;
            notes = flattenedHarmonics[currentNormalMusicIndex];
        }

        int noteIndex = notes[currentNormalNoteIndex];
        if (noteIndex < 0 || noteIndex >= pathwayAudios.Length)
        {
            currentNormalNoteIndex++;
            return;
        }
        PlaySFX(pathwayAudios[noteIndex], false);
        currentNormalNoteIndex++;
        if (currentNormalNoteIndex >= notes.Count)
        {
            currentNormalNoteIndex = 0;
            currentNormalMusicIndex = (currentNormalMusicIndex + 1) % 3;
        }
    }

    // Plays the next note in the spatial harmonic melody sequence at the specified position and advances to the next
    public void PlaySpatialHarmonicMelodies(Vector3 position)
    {
        if (mute) return;

        List<int> notes = flattenedHarmonics[currentSpatialMusicIndex];
        if (currentSpatialNoteIndex >= notes.Count)
        {
            currentSpatialNoteIndex = 0;
            currentSpatialMusicIndex = (currentSpatialMusicIndex + 1) % 3;
            notes = flattenedHarmonics[currentSpatialMusicIndex];
        }

        int noteIndex = notes[currentSpatialNoteIndex];
        if (noteIndex < 0 || noteIndex >= pathwayAudios.Length)
        {
            currentSpatialNoteIndex++;
            return;
        }

        PlaySpatial(pathwayAudios[noteIndex], position);

        currentSpatialNoteIndex++;
        if (currentSpatialNoteIndex >= notes.Count)
        {
            currentSpatialNoteIndex = 0;
            currentSpatialMusicIndex = (currentSpatialMusicIndex + 1) % 3;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopAllSFX()
    {
        foreach (var src in sfxSources)
        {
            src.Stop();
        }
    }

    // Stops all spatial audio sources
    public void StopAllSpatial()
    {
        foreach (var src in spatialSources)
        {
            src.Stop();
        }
    }
}