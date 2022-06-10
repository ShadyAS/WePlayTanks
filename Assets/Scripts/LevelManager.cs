using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LevelManager : NetworkBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance { get => instance; set => instance = value; }

    private void Awake()
    {
        instance = this;
    }

    [Header("Levels")]
    [SerializeField] GameObject[] levels;

    [Header("Enemies")]
    [SerializeField] GameObject[] enemies1;
    [SerializeField] GameObject[] enemies2;
    [SerializeField] GameObject[] enemies3;
    [SerializeField] GameObject[] enemies4;
    [SerializeField] GameObject[] enemies5;
    [SerializeField] GameObject[] enemies6;
    [SerializeField] GameObject[] enemies7;
    [SerializeField] GameObject[] enemies8;
    [SerializeField] GameObject[] enemies9;
    [SerializeField] GameObject[] enemies10;

    List<GameObject[]> enemies = new List<GameObject[]>();

    [Header("Spawns")]
    [SerializeField] Transform[] spawns1;
    [SerializeField] Transform[] spawns2;
    [SerializeField] Transform[] spawns3;
    [SerializeField] Transform[] spawns4;
    [SerializeField] Transform[] spawns5;
    [SerializeField] Transform[] spawns6;
    [SerializeField] Transform[] spawns7;
    [SerializeField] Transform[] spawns8;
    [SerializeField] Transform[] spawns9;
    [SerializeField] Transform[] spawns10;

    List<Transform[]> spawns = new List<Transform[]>();

    [SyncVar]
    int currentLevel = 1;

    public List<Projectile> proj = new List<Projectile>();
    public List<ProjectileEnemy> projEn = new List<ProjectileEnemy>();

    public List<Mine> mine = new List<Mine>();
    public List<MineEnemy> mineEn = new List<MineEnemy>();

    public List<GameObject> tankTraces = new List<GameObject>();
    public List<GameObject> tankCrosses = new List<GameObject>();

    float timer = 0f;
    int deadPlayers = 0;
    bool gameEnd = false;

    private void Start()
    {
        enemies.Add(enemies1);
        enemies.Add(enemies2);
        enemies.Add(enemies3);
        enemies.Add(enemies4);
        enemies.Add(enemies5);
        enemies.Add(enemies6);
        enemies.Add(enemies7);
        enemies.Add(enemies8);
        enemies.Add(enemies9);
        enemies.Add(enemies10);

        for (int i = 0; i < 10;i++)
        {
            levels[i].SetActive(false);
        }

        spawns.Add(spawns1);
        spawns.Add(spawns2);
        spawns.Add(spawns3);
        spawns.Add(spawns4);
        spawns.Add(spawns5);
        spawns.Add(spawns6);
        spawns.Add(spawns7);
        spawns.Add(spawns8);
        spawns.Add(spawns9);
        spawns.Add(spawns10);
    }

    public void ResetDefaults()
    {
        enemies.Clear();
        spawns.Clear();

        enemies.Add(enemies1);
        enemies.Add(enemies2);
        enemies.Add(enemies3);
        enemies.Add(enemies4);
        enemies.Add(enemies5);
        enemies.Add(enemies6);
        enemies.Add(enemies7);
        enemies.Add(enemies8);
        enemies.Add(enemies9);
        enemies.Add(enemies10);

        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
            {
                levels[i].SetActive(true);
            }
            else
            {
                levels[i].SetActive(false);
            }
        }

        spawns.Add(spawns1);
        spawns.Add(spawns2);
        spawns.Add(spawns3);
        spawns.Add(spawns4);
        spawns.Add(spawns5);
        spawns.Add(spawns6);
        spawns.Add(spawns7);
        spawns.Add(spawns8);
        spawns.Add(spawns9);
        spawns.Add(spawns10);

        currentLevel = 1;
        timer = 0f;
        deadPlayers = 0;
        gameEnd = false;

        ClearLists();
    }

    private void Update()
    {
        if (!gameEnd && CheckForPlayersDeath())
        {
            gameEnd = true;
            EntireEndingGame(false);
        }

        if (CheckForEnemiesDeath())
        {
            timer += Time.deltaTime;
            if (timer > 3f)
            {
                RpcChangeLevel();

                timer = 0f;
            }
        }
    }

    public void EntireEndingGame(bool _victory)
    {
        RpcDisplayEndGame(_victory);
        RpcPauseMenuOn(true);
    }

    [ClientRpc]
    void RpcDisplayEndGame(bool _victory)
    {
        if (_victory)
        {
            GameManager.Instance.audioSource.PlayOneShot(GameManager.Instance.endVictorySound);
        }
        else
        {
            GameManager.Instance.audioSource.PlayOneShot(GameManager.Instance.endDefeatSound);
        }

        GameManager.Instance.DisplayScoreScreen(_victory);

        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        GameManager.Instance.countDownEndGame.text = "5";
        yield return new WaitForSeconds(1f);

        GameManager.Instance.countDownEndGame.text = "4";
        yield return new WaitForSeconds(1f);

        GameManager.Instance.countDownEndGame.text = "3";
        yield return new WaitForSeconds(1f);

        GameManager.Instance.countDownEndGame.text = "2";
        yield return new WaitForSeconds(1f);

        GameManager.Instance.countDownEndGame.text = "1";
        yield return new WaitForSeconds(1f);

        RpcPauseMenuOn(false);
        RpcEndGame();
    }

    bool CheckForPlayersDeath()
    {
        foreach (TankController tc in GameManager.players.Values)
        {
            if (!tc.gameObject.activeSelf)
            {
                deadPlayers++;
            }
        }

        if (deadPlayers == 2)
        {
            return true;
        }

        deadPlayers = 0;
        return false;
    }

    bool CheckForEnemiesDeath()
    {
        int death = 0;

        foreach (GameObject go in enemies[currentLevel - 1])
        {
            if (!go)
            {
                death++;
            }
        }

        if (death == enemies[currentLevel - 1].Length)
        {
            RpcEndLevel(true);
            RpcPauseMenuOn(true);

            return true;
        }

        return false;
    }

    [ClientRpc]
    void RpcChangeLevel()
    {
        currentLevel++;

        if (currentLevel == 11)
        {
            EntireEndingGame(true);
            GameManager.Instance.gameLaunched = false;
        }
        else
        {                        
            RpcEndLevelFade();
        }        
    }

    [ClientRpc]
    void RpcEndLevelFade()
    {
        StartCoroutine(EndLevelFade());
    }

    IEnumerator EndLevelFade()
    {
        float timerFade = 0f;
        while (timerFade <= 1f)
        {
            timerFade += Time.deltaTime;
            GameManager.Instance.SetFadeAlpha(timerFade);

            yield return null;
        }
        GameManager.Instance.SetFadeAlpha(1f);

        GameManager.Instance.ActivationEndLevelUI(false);

        levels[currentLevel - 2].SetActive(false);
        levels[currentLevel - 1].SetActive(true);

        foreach (TankController tc in GameManager.players.Values)
        {
            tc.gameObject.SetActive(true);
        }

        ClearLists();

        GameManager.Instance.SetLevel(currentLevel);

        GameManager.Instance.SetPlayerTransform(1, spawns[currentLevel - 1][0]);
        GameManager.Instance.SetPlayerTransform(2, spawns[currentLevel - 1][1]);

        yield return new WaitForSeconds(1f);

        timerFade = 0f;
        while (timerFade <= 1f)
        {
            timerFade += Time.deltaTime;
            GameManager.Instance.SetFadeAlpha(1f - timerFade);

            yield return null;
        }
        GameManager.Instance.SetFadeAlpha(0f);

        RpcEndLevel(false);
    }

    [ClientRpc]
    void RpcEndLevel(bool _b)
    {
        GameManager.Instance.ActivationEndLevelUI(_b);

        if (!_b)
        {
            StartCoroutine(StartLevel());
        }
        else
        {            
            GameManager.Instance.audioSource.Stop();
        }    
    }

    [ClientRpc]
    void RpcPauseMenuOn(bool _b)
    {
        PauseMenu.isOn = _b;
    }

    public void LaunchGame()
    {
        RpcPauseMenuOn(true);

        ClearLists();        

        levels[currentLevel - 1].SetActive(true);

        GameManager.Instance.SetPlayerTransform(1, spawns[currentLevel - 1][0]);
        GameManager.Instance.SetPlayerTransform(2, spawns[currentLevel - 1][1]);

        StartCoroutine(StartLevel());
    }

    IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(1f);

        GameManager.Instance.countDownText.gameObject.SetActive(true);

        GameManager.Instance.countDownText.text = "3";
        yield return new WaitForSeconds(1f);

        GameManager.Instance.countDownText.text = "2";
        yield return new WaitForSeconds(1f);

        GameManager.Instance.countDownText.text = "1";
        yield return new WaitForSeconds(1f);

        GameManager.Instance.countDownText.text = "Start";
        RpcPauseMenuOn(false);
        GameManager.Instance.audioSource.Play();
        yield return new WaitForSeconds(0.5f);

        GameManager.Instance.countDownText.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcEndGame()
    {
        if (isClientOnly)
        {
            NetworkManager.singleton.StopClient();
        }
        else
        {
            NetworkManager.singleton.StopHost();
        }
        
        LevelManager.Instance.ResetDefaults();
        GameManager.Instance.ResetDefaults();
    }

    void ClearLists()
    {
        foreach (Projectile p in proj)
        {
            Destroy(p.gameObject);
        }
        proj.Clear();

        foreach (ProjectileEnemy p in projEn)
        {
            Destroy(p.gameObject);
        }
        projEn.Clear();

        foreach (Mine m in mine)
        {
            Destroy(m.gameObject);
        }
        mine.Clear();

        foreach (MineEnemy m in mineEn)
        {
            Destroy(m.gameObject);
        }
        mineEn.Clear();

        foreach (GameObject go in tankTraces)
        {
            Destroy(go);
        }
        tankTraces.Clear();

        foreach (GameObject go in tankCrosses)
        {
            Destroy(go);
        }
        tankCrosses.Clear();
    }
}
