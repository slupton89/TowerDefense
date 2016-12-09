using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus {
    next, play, gameover, win
}

public class GameManager : Singleton<GameManager> {

    [SerializeField]
    private int totalWaves = 10;
    [SerializeField]
    private Text totalMoneyLbl;
    [SerializeField]
    private Text currentWaveLbl;
    [SerializeField]
    private Text totalEscapedLbl;
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private Enemy[] enemies;
    [SerializeField]
    private int totalEnemies = 3;
    [SerializeField]
    private int enemiesPerSpawn;
    [SerializeField]
    private Text playBtnLbl;
    [SerializeField]
    private Button playBtn;
    [SerializeField] 
    private Image banner;
    [SerializeField] private Text bannerText;

    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int enemiesToSpawn = 0;
    private gameStatus currentState = gameStatus.play;
    private AudioSource audioSource;

    const float spawnDelay = 0.5f;

    public List<Enemy> EnemyList = new List<Enemy>();

    public int TotalMoney {
        get {
            return totalMoney;
        }
        set {
            totalMoney = value;
            totalMoneyLbl.text = totalMoney.ToString();
        }
    }

    public AudioSource AudioSource {
        get {
            return audioSource;
        }
    }

    public int TotalEscaped {
        get {
            return totalEscaped;
        }
        set {
            totalEscaped = value;
        }
    }

    public int RoundEscaped {
        get {
            return roundEscaped;
        }
        set {
            roundEscaped = value;
        }
    }

    public int TotalKilled {
        get {
            return totalKilled;
        }
        set {
            totalKilled = value;
        }
    }

    void Start () {
        playBtn.gameObject.SetActive(false);
        banner.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        ShowMenu();

        Debug.Log(bannerText.text);
	}
	
	void Update () {
      HandleEscape();
	}

    IEnumerator Spawn() {
        if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies) {
            for (int i = 0; i < enemiesPerSpawn; i++) {
                if (EnemyList.Count < totalEnemies) {
                    Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)]);
                    newEnemy.transform.position = spawnPoint.transform.position;
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(Spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy) {
        EnemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy) {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DestroyAllEnemies() {
        foreach(Enemy enemy in EnemyList) {
            Destroy(enemy.gameObject);
        }

        EnemyList.Clear();
    }

    public void AddMoney(int amount) {
        TotalMoney += amount;
    }

    public void SubtractMoney(int amount) {
        TotalMoney -= amount;
    }

    public void IsWaveOver() {
        totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
        if ((RoundEscaped + TotalKilled) == totalEnemies) {
            if ( waveNumber <= enemies.Length) {
                enemiesToSpawn = waveNumber;
            }
            SetCurrentGameState();
            ShowMenu();
        }
    }

    public void SetCurrentGameState() {
        if (TotalEscaped >= totalEnemies) {
            currentState = gameStatus.gameover;
        } else if (waveNumber == 0 && (TotalKilled + RoundEscaped) == 0) {
            currentState = gameStatus.play;
        } else if (waveNumber >= totalWaves) {
            currentState = gameStatus.win;
        } else {
            currentState = gameStatus.next;
        }
    }

    public void ShowMenu() {
        switch (currentState) {
            case gameStatus.gameover:
                playBtn.gameObject.SetActive(true);
                playBtnLbl.text = "Play Again!";
                AudioSource.PlayOneShot(SoundManager.Instance.Gameover);
                banner.gameObject.SetActive(true);
                bannerText.text = "You Lose!";
                break;
            case gameStatus.next:
                playBtnLbl.text = "Start Next Wave";
                break;
            case gameStatus.play:
                playBtnLbl.text = "Play";
                break;
            case gameStatus.win:
                playBtn.gameObject.SetActive(true);
                playBtnLbl.text = "Play";
                banner.gameObject.SetActive(true);
                bannerText.text = "You Win!";
                break;
        }
        playBtn.gameObject.SetActive(true);
    }

    public void PlayBtnPressed() {
        switch(currentState) {
            case gameStatus.next:
                waveNumber +=1;
                totalEnemies += waveNumber;
                break;
            default:
                totalEnemies = 3;
                TotalEscaped = 0;
                TotalMoney = 10;
                enemiesToSpawn = 0;
                TowerManager.Instance.DestroyAllTower();
                TowerManager.Instance.RenameTagsBuildSites();
                totalMoneyLbl.text = TotalMoney.ToString();
                totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
                audioSource.PlayOneShot(SoundManager.Instance.NewGame);
                break;
        }
        DestroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWaveLbl.text = "Wave " + (waveNumber + 1) + "/" + (totalWaves + 1);
        StartCoroutine(Spawn());
        playBtn.gameObject.SetActive(false);
        banner.gameObject.SetActive(false);
    }

    private void HandleEscape() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TowerManager.Instance.disableDragSprite();
            TowerManager.Instance.towerButtonPressed = null;
        }
    }
}
