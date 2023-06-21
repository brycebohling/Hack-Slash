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
    [SerializeField] GameObject leaderBoardTable;
    [SerializeField] Transform entryContainer;
    [SerializeField] Transform entryTemplate;
    [SerializeField] float templateHeight;
    [SerializeField] int listYStartPos;

    bool isShowing;

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isShowing)
            {
                HideLeaderboard();
                isShowing = false;
                
            } else
            {
                ShowLeaderboard();
                isShowing = true;
            }
            
        }    
        if (Input.GetKeyDown(KeyCode.N))
        {
            PostData();
        }
    }

    public void ShowLeaderboard()
    {
        StartCoroutine(FetchData());
    }

    private IEnumerator FetchData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
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
                    Leaderboard leaderboard = JsonConvert.DeserializeObject<Leaderboard>(webRequest.downloadHandler.text);  

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

                    for (int i = 0; i < leaderboard.data.Count; i++)
                    {
                        Transform entryTransform = Instantiate(entryTemplate, entryContainer);
                        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
                        entryRectTransform.anchoredPosition = new Vector2 (0, listYStartPos + (-templateHeight * i));

                        entryTransform.Find("RankingText").GetComponent<TextMeshProUGUI>().text = i + 1 + "";
                        entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = leaderboard.data[i].user;
                        entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = leaderboard.data[i].score + "";
                    }

                    leaderBoardTable.SetActive(true);
                    break;
            }
        }
    }   

    public void PostData()
    {
        StartCoroutine(SendData());
    }

    private IEnumerator SendData()
    {
        Data data = new Data();
        // data.id = 100;
        data.user = "Will";
        data.score = 5;
        data.wave = 9;
        data.damage = 90;
        data.difficulty = "hardcore";
        data.kills = 1;
        data.version = "test";

        string json = JsonUtility.ToJson(data);
        Debug.Log(json);

        using (UnityWebRequest webPost = UnityWebRequest.PostWwwForm(url, json))
        {    
            yield return webPost.SendWebRequest();

            if (webPost.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webPost.error);
            }
            else
            {
                Debug.Log("Data sent");
            }
        }
    }

    public void HideLeaderboard()
    {
        foreach (Transform child in entryContainer) 
        {
            Destroy(child.gameObject);
        }

        leaderBoardTable.SetActive(false);
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