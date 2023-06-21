using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
public class LeaderboardDataManager : MonoBehaviour
{
    public string url;
    public GameObject leaderboardPanel;


    public void GetData()
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
                    Debug.Log(webRequest.downloadHandler.text);
                    Leaderboard leaderboard = JsonConvert.DeserializeObject<Leaderboard>(webRequest.downloadHandler.text);  
                    
                    Debug.Log(leaderboard.playerLeaderboardStatsList.Count);
                    Debug.Log(leaderboard.playerLeaderboardStatsList[0].wave);
                    Debug.Log(leaderboard.playerLeaderboardStatsList[1].wave);
                    break;
            }
        }
    }

    public void PostData()
    {
        StartCoroutine(PushData());
    }

    private IEnumerator PushData()
    {
        yield return null;
    }
}