using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Header("In Game")]
    [SerializeField] public GameObject inGameUI;
    [SerializeField] Text scoreJ1; 
    [SerializeField] Text scoreJ2;
    [SerializeField] Text level;

    [Header("Materials")]
    [SerializeField] Material player1;
    [SerializeField] Material player2;

    [Header("Transition")]
    [SerializeField] Image fadeImage;
    [SerializeField] GameObject startText;
    [SerializeField] public Text countDownText;
    [SerializeField] public Text countDownEndGame;

    public static Dictionary<int, TankController> players = new Dictionary<int, TankController>();

    public bool gameLaunched = false;

    [Header("End Level")]
    [SerializeField] GameObject endLevelPanel;
    [SerializeField] Text p1Score;
    [SerializeField] Text p2Score;


    [Header("Score Screen")]
    [SerializeField] public GameObject scoreScreen;
    [SerializeField] Text scoreScreenTitle;
    [SerializeField] Text p1ScoreScreen;
    [SerializeField] Text p2ScoreScreen;

    public AudioSource audioSource;
    [Header("Musics/Sounds")]
    [SerializeField] public AudioClip menuMusic;
    [SerializeField] public AudioClip gameMusic;
    [SerializeField] public AudioClip endVictorySound;
    [SerializeField] public AudioClip endDefeatSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        ResetDefaults();
    }

    public void ResetDefaults()
    {
        level.text = "Level 1";
        inGameUI.SetActive(false);
        startText.SetActive(false);
        endLevelPanel.SetActive(false);
        scoreScreen.SetActive(false);
        countDownText.gameObject.SetActive(false);
        players.Clear();
        gameLaunched = false;

        if (audioSource.clip != menuMusic)
        {
            audioSource.clip = menuMusic;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void RegisterPlayer(TankController _tc)
    {
        if (players.Count == 0)
        {
            players.Add(1, _tc);
            _tc.name = "Player1";

            MeshRenderer[] mrs = _tc.gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in mrs)
            {
                mr.material = player1;
            }
        }
        else
        {
            players.Add(2, _tc);
            _tc.name = "Player2";

            MeshRenderer[] mrs = _tc.gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in mrs)
            {
                mr.material = player2;
            }

            if (!gameLaunched)
            {
                DecreaseVolumeMenu();
                StartCoroutine(GameStart());
            }
        }
    }

    void DecreaseVolumeMenu()
    {
        if (audioSource.volume > 0f)
        {
            audioSource.volume -= 0.01f;
        }
        else
        {
            audioSource.Stop();
        }
    }

    public void UnregisterPlayer(int _id)
    {
        players.Remove(_id);
    }

    private void Update()
    {
        if (inGameUI.activeSelf)
        {
            scoreJ1.text = "J1\n" + players[1].Score.ToString();
            scoreJ2.text = "J2\n" + players[2].Score.ToString();
        }        
    }

    IEnumerator GameStart()
    {
        startText.SetActive(true);

        yield return new WaitForSeconds(3f);

        float timerFade = 0f;
        while (timerFade <= 1f)
        {
            timerFade += Time.deltaTime;
            SetFadeAlpha(timerFade);

            yield return null;
        }
        SetFadeAlpha(1f);

        startText.SetActive(false);
        audioSource.Stop();
        audioSource.clip = gameMusic;

        LevelManager.Instance.LaunchGame();
        gameLaunched = true;

        yield return new WaitForSeconds(1f);

        timerFade = 0f;
        while (timerFade <= 1f)
        {
            timerFade += Time.deltaTime;
            SetFadeAlpha(1f - timerFade);

            yield return null;
        }
        SetFadeAlpha(0f);

        inGameUI.SetActive(true);
    }

    public void SetFadeAlpha(float _alpha)
    {
        Color c = fadeImage.color;
        c.a = _alpha;
        fadeImage.color = c;
    }

    public void SetLevel(int _level)
    {
        level.text = "Level " + _level;
    }

    public void SetPlayerTransform(int _id, Transform _pos)
    {
        players[_id].transform.position = _pos.position;
        players[_id].transform.rotation = _pos.rotation;
    }

    public void ActivationEndLevelUI(bool _b)
    {
        p1Score.text = players[1].Score.ToString();
        p2Score.text = players[2].Score.ToString();

        endLevelPanel.SetActive(_b);
    }

    public void DisplayScoreScreen(bool _victory)
    {
        scoreScreenTitle.text = (_victory ? "Victory" : "Defeat");
        p1ScoreScreen.text = players[1].Score.ToString();
        p2ScoreScreen.text = players[2].Score.ToString();
        scoreScreen.SetActive(true);
    }
}
