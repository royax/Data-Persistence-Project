using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;

    public InputField NameInput;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    //private int bestScore;
    //private string bestName;
    private BestScore bestScore;

    
    // Start is called before the first frame update
    void Start()
    {
        LoadScore();
        BestScoreText.text = $"Best Score: {bestScore.Name}: {bestScore.Score}";
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        
    }

    private void LoadScore()
    {
        string path = Application.persistentDataPath + "/bestscore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            bestScore = JsonUtility.FromJson<BestScore>(json);
        }
        else
        {
            bestScore = new BestScore();
        }
     
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        //if (m_Points > bestScore.Score)
        //{
        //    BestScoreText.text = $"Best Score: YOU: {m_Points}";
        //}
    }

    private void ShowBestScore()
    {
        throw new System.NotImplementedException();
    }

    public void OnNameEnter()
    {
        bestScore.Name = NameInput.text;
        bestScore.Score = m_Points;
        SaveScore();
        NameInput.gameObject.SetActive(false);
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void GameOver()
    {


        if (m_Points > bestScore.Score)
        {
            NameInput.gameObject.SetActive(true);
            NameInput.ActivateInputField();


        }
        else
        {
        m_GameOver = true;
        GameOverText.SetActive(true);
        }
        

    }

    private void SaveScore()
    {
        string json = JsonUtility.ToJson(bestScore);
        File.WriteAllText(Application.persistentDataPath+ "/bestscore.json", json);
    }

    [System.Serializable]
     class BestScore
    {
       public  string Name;
       public  int Score;
    }
}
