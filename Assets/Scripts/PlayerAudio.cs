using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Walking Sounds")]
    public AudioClip[] walkSoundsLevel1;
    public AudioClip[] walkSoundsLevel2;
    [SerializeField] private float walkVolume = 8f;
    public float walkSoundInterval = 0.3f;

    [Header("SFX Sounds")]
    public AudioClip[] attackSounds;
    public AudioClip[] specialSounds;
    public AudioClip[] dashSounds;
    [SerializeField] private float sfxVolume = 1f;

    [Header("Background Music - Level 1")]
    public AudioClip calmMusicLevel1;
    public AudioClip combatMusicLevel1;

    [Header("Background Music - Level 2")]
    public AudioClip calmMusicLevel2;
    public AudioClip combatMusicLevel2;

    [SerializeField] private float musicVolume = 0.3f;
    public float fadeSpeed = 1f;
    public float combatLingerTime = 3f;


    [Header("Settings")]
    public int currentLevel = 1;

    // Audio Sources
    private AudioSource sfxSource;
    private AudioSource walkSource;
    private AudioSource calmSource;
    private AudioSource combatSource;

    // State
    private float walkTimer;
    private float combatTimer;
    private bool inCombat = false;

    private void Awake()
    {
        // SFX source — uses the existing AudioSource on the GameObject
        sfxSource = GetComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        // Footstep source
        walkSource = gameObject.AddComponent<AudioSource>();
        walkSource.loop = false;
        walkSource.playOnAwake = false;

        // Calm music source
        calmSource = gameObject.AddComponent<AudioSource>();
        calmSource.loop = true;
        calmSource.playOnAwake = false;
        calmSource.volume = musicVolume;

        // Combat music source
        combatSource = gameObject.AddComponent<AudioSource>();
        combatSource.loop = true;
        combatSource.playOnAwake = false;
        combatSource.volume = 0f;
    }

    private void Start()
    {
        currentLevel = 1;

        if (calmMusicLevel1 != null)
        {
            calmSource.clip = calmMusicLevel1;
            calmSource.volume = musicVolume;
            calmSource.Play();
        }

        if (combatMusicLevel1 != null)
        {
            combatSource.clip = combatMusicLevel1;
            combatSource.volume = 0f;
            combatSource.Play();
        }
    }
    private void Update()
    {
        HandleFootsteps();
        HandleMusicFade();
        HandleCombatTimer();
    }

    // ── Footsteps ────────────────────────────────────────

    private void HandleFootsteps()
    {
        bool isMoving = PlayerMovement.Instance.isMoving;

        if (!isMoving)
        {
            if (walkSource.isPlaying)
                walkSource.Stop();
            return;
        }

        if (!walkSource.isPlaying)
        {
            PlayWalkSound();
        }
    }

    private void PlayWalkSound()
    {
        AudioClip[] clips = currentLevel == 1 ? walkSoundsLevel1 : walkSoundsLevel2;
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        walkSource.clip = clip;
        walkSource.loop = true;   // ← keep looping while moving
        walkSource.volume = walkVolume;
        walkSource.Play();
    }

    // ── SFX ──────────────────────────────────────────────

    public void PlayAttack() => PlaySFX(attackSounds);
    public void PlaySpecial() => PlaySFX(specialSounds);
    public void PlayDash() => PlaySFX(dashSounds);

    private void PlaySFX(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length)], sfxVolume);
    }

    // ── Combat Music ─────────────────────────────────────

    public void EnterCombat()
    {
        inCombat = true;
        combatTimer = combatLingerTime;
    }

    private void ExitCombat()
    {
        inCombat = false;
    }

    private void HandleCombatTimer()
    {
        if (!inCombat) return;
        combatTimer -= Time.deltaTime;
        if (combatTimer <= 0f) ExitCombat();
    }

    private void HandleMusicFade()
    {
        float targetCalm = inCombat ? 0f : musicVolume;
        float targetCombat = inCombat ? musicVolume : 0f;

        calmSource.volume = Mathf.MoveTowards(calmSource.volume, targetCalm, fadeSpeed * Time.deltaTime);
        combatSource.volume = Mathf.MoveTowards(combatSource.volume, targetCombat, fadeSpeed * Time.deltaTime);
    }

    public void SwitchLevelMusic(int level)
    {
        AudioClip newCalm = level == 1 ? calmMusicLevel1 : calmMusicLevel2;
        AudioClip newCombat = level == 1 ? combatMusicLevel1 : combatMusicLevel2;

        if (newCalm != null && calmSource.clip != newCalm)
        {
            calmSource.clip = newCalm;
            calmSource.volume = inCombat ? 0f : musicVolume;
            calmSource.Play();
        }

        if (newCombat != null && combatSource.clip != newCombat)
        {
            combatSource.clip = newCombat;
            combatSource.volume = inCombat ? musicVolume : 0f;
            combatSource.Play();
        }
    }
}