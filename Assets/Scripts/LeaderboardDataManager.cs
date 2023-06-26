using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class LeaderboardDataManager : MonoBehaviour
{
    [SerializeField] string url;
    [SerializeField] int entryLimit;
    [SerializeField] string version;
    [SerializeField] Transform entryContainer;
    [SerializeField] Transform entryTemplate;
    [SerializeField] float templateHeight;
    [SerializeField] int listYStartPos;
    Leaderboard leaderboard;
    bool isShowing;
    public bool gotData;


    private void OnEnable() 
    {
        ShowHideLeaderboard();
    }

    private void OnDisable() 
    {
        HideLeaderboard();    
    }

    private void ShowHideLeaderboard()
    {
        if (isShowing)
        {
            HideLeaderboard();
        } else
        {
            StartCoroutine(FetchData());
            StartCoroutine(ShowLeaderboard());
        }
    }

    public void PostData()
    {
        StartCoroutine(SendData());
    }

    public bool IsTop100(float playerScore)
    {
        if (leaderboard.data.Count == 99)
        {
            if (playerScore > leaderboard.data[99].score)
            {
                return true;
            } else
            {
                return false;
            }
        
        } else if (leaderboard.data.Count < 99)
        {
            return true;
        } else
        {
            Debug.Log("Too many leaderboard entries");
            return false;
        }
        
    }

    public int Ranking(float playerScore)
    {
        int ranking = 1;
        for (int i = 0; i < leaderboard.data.Count; i++)
        {
            ranking++;
            if (playerScore > leaderboard.data.Count)
            {   
                break;
            }
        }

        if (leaderboard.data.Count == 99)
        {
            // Remove 100ths
        }

        // and post new data

        return ranking;
    }

    private IEnumerator ShowLeaderboard()
    {
        yield return new WaitUntil(() => gotData);
        gotData = false;
        
        for (int i = 0; i < leaderboard.data.Count; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            // RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            // entryRectTransform.anchoredPosition = new Vector2 (0, listYStartPos + (-templateHeight * i));

            entryTransform.Find("RankingText").GetComponent<TextMeshProUGUI>().text = i + 1 + "";
            entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = leaderboard.data[i].user;
            entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = leaderboard.data[i].score + "";
        }

        isShowing = true;
    }

    public IEnumerator FetchData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url + "limit=" + entryLimit + "&version=" + version))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log("Connection Error");
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log("Processing Error");
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Protocol Error");
                    break;

                case UnityWebRequest.Result.Success:
                    leaderboard = JsonConvert.DeserializeObject<Leaderboard>(webRequest.downloadHandler.text); 

                    for (int x = 0; x < leaderboard.data.Count; x++)
                    {
                        for (int y = x + 1; y < leaderboard.data.Count; y++)
                        {
                            if (leaderboard.data[y].score > leaderboard.data[x].score)
                            {
                                Data tmp = leaderboard.data[x];
                                leaderboard.data[x] = leaderboard.data[y];
                                leaderboard.data[y] = tmp;
                            }
                        }
                    }
                    gotData = true; 
                    break;
            }
        }
    }   

    private void HideLeaderboard()
    {
        foreach (Transform child in entryContainer) 
        {
            Destroy(child.gameObject);
        }

        isShowing = false;
    }

    private IEnumerator SendData()
    {
        Data data = new Data();
        data.user = "Peter01";
        data.score = 10;
        data.wave = 7;
        data.damage = 90;
        data.difficulty = "normal";
        data.kills = 1;
        data.version = version;

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest webPost = UnityWebRequest.Post(url, json, "application/json"))
        {    
            yield return webPost.SendWebRequest();

            if (webPost.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webPost.error);
            }
        }
    }
}

public class Leaderboard
{
    public int results;
    public List<Data> data;
}

[System.Serializable] public class Data
{
    public string user;
    public int score;
    public int wave;
    public int damage;
    public string difficulty;
    public int kills;
    public string version;
}