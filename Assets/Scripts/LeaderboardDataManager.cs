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
    // Versions used test01, alpha01
    string version = "alpha01";
    [SerializeField] Transform entryContainer;
    [SerializeField] Transform entryTemplate;
    [SerializeField] Color alternateBG;
    [SerializeField] Color backgroundNameHighlight;
    public string leaderboardDifficulty;
    Leaderboard leaderboard;
    bool isShowing;
    bool isShowingExtraInfo;
    public bool gotData;
    bool addedEntries;
    int rank;


    private void OnEnable() 
    {
        ShowHideLeaderboard();
    }

    private void OnDisable() 
    {
        HideLeaderboard();    
    }

    public void ShowHideLeaderboard()
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

    public void ChangeLeaderboardDifficulty(string difficulty)
    {
        leaderboardDifficulty = difficulty;
        HideLeaderboard();
        ShowHideLeaderboard(); 
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
        rank = 0;

        if (leaderboard.data.Count == 0)
        {
            return rank = 1;
        }

        if (playerScore <= leaderboard.data[leaderboard.data.Count - 1].score)
        {
            rank = leaderboard.data.Count + 1;
        } else
        {
            
            for (int i = 0; i < leaderboard.data.Count; i++)
            {   
                rank++;
                if (playerScore > leaderboard.data[i].score)
                {   
                    break;
                }
            }
        }
        
        return rank;
    }

    private IEnumerator ShowLeaderboard()
    {
        yield return new WaitUntil(() => gotData);
        gotData = false;

        for (int i = 0; i < leaderboard.data.Count; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);

            entryTransform.Find("RankingText").GetComponent<TextMeshProUGUI>().text = i + 1 + "";
            entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = leaderboard.data[i].user;
            entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = leaderboard.data[i].score + "";

            Button btn = entryTransform.Find("ExtraInfoBtn").GetComponent<Button>();
            btn.onClick.AddListener(() => ShowHideExtraInfo(entryTransform.gameObject));

            if (i % 2 == 0)
            {
                entryTransform.Find("Background").GetComponent<Image>().color = alternateBG;
            }
        }

        if (rank != 0)
        {
            Transform playerRankTransform = entryContainer.GetChild(rank - 1);

            playerRankTransform.Find("Background").GetComponent<Image>().color = backgroundNameHighlight;
        }

        isShowing = true;
    }

    public IEnumerator FetchData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url + "limit=" + entryLimit + "&version=" + version + "&difficulty=" + leaderboardDifficulty))
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

    public IEnumerator SendData(string name, int score, int wave, int damage, string difficulty, int kills)
    {
        Data data = new Data();
        data.user = name;
        data.score = score;
        data.wave = wave;
        data.damage = damage;
        data.difficulty = difficulty;
        data.kills = kills;
        data.createdAt = System.DateTime.Now.ToUniversalTime();
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

    private void ShowHideExtraInfo(GameObject entry)
    {
        if (!isShowingExtraInfo)
        {
            Animator anim = entry.GetComponent<Animator>();
            anim.Play("showExtraInfo");
            
            int index = 0;
            foreach (Transform child in entryContainer) 
            {
                if (entry.transform != child)
                {
                    child.gameObject.SetActive(false);
                } else
                {
                    Transform extraInfoTransform = child.Find("ExtraInfo").GetComponent<Transform>();
                    extraInfoTransform.Find("Kills").GetComponent<TextMeshProUGUI>().text = leaderboard.data[index].kills.ToString();
                    extraInfoTransform.Find("DamageDealt").GetComponent<TextMeshProUGUI>().text = leaderboard.data[index].damage.ToString();

                    System.DateTime convertedDate = System.DateTime.SpecifyKind(System.DateTime.Parse(leaderboard.data[index].createdAt.ToString()), System.DateTimeKind.Utc);
                    extraInfoTransform.Find("Date").GetComponent<TextMeshProUGUI>().text = convertedDate.ToLocalTime().ToShortDateString();
                }
                index++;
            }

            isShowingExtraInfo = true;
        } else
        {
            Animator anim = entry.GetComponent<Animator>();
            anim.Play("normal");

            foreach (Transform child in entryContainer) 
            {
                child.gameObject.SetActive(true);
            }
    
            isShowingExtraInfo = false;
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
    public System.DateTime createdAt;
    public string version;
}