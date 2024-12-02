﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;          //JSON 라이브러리 추가
using System;
using UnityEngine.Rendering;
using Unity.VisualScripting;       //Action<> 사용을 위한 네임스페이스 추가

public class GameAPI : MonoBehaviour
{
    private string baseUrl = "http://localhost:4000/api";   //Node.js 서버의 URL
    //�÷��̾� ��� �޼���
    public IEnumerator RegisterPlayer(string playerName, string password)
    {
        var requestData = new { name = playerName, password = password };
        string jsonData = JsonConvert.SerializeObject(requestData);
        Debug.Log($"Registering Player: {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/register", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error registering player: {request.result}");
            }
            else
            {
                Debug.Log("Player registered successfully");
            }
        }
    }

    //플레이어 로그인 메서드
    public IEnumerator LoginPlayer(string playerName, string password, Action<PlayerModel> onSuccess)
    {
        var requestData = new { name = playerName, password = password };
        string jsonData = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)       //실패 예러
            {
                Debug.LogError($"Error loging in : {request.error}");   //에러 로그
            }
            else
            {
                //응답을 처리하여 PlayerModel 생성   
                string responseBody = request.downloadHandler.text;

                try
                {
                    var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                    //서버 응답에서 PlayerModel 생성
                    PlayerModel playerModel = new PlayerModel(responseData["playerName"].ToString())
                    {
                        metal = Convert.ToInt32(responseData["metal"]),
                        crystal = Convert.ToInt32(responseData["crystal"]),
                        deuterium = Convert.ToInt32(responseData["deuterium"]),
                        Planets = new List<PlanetModel>()
                    };

                    onSuccess?.Invoke(playerModel); //PlayerModel 반환
                    Debug.Log("Login successful");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error processing login response: {ex.Message}");
                }
            }
        }
    }

    //플레이어 등록 메서드
    public IEnumerator CollectResources(string playerName, Action<PlayerModel> onSuccess)
    {
        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/collect/{playerName}", "POST"))
        {
            string jsonData = JsonConvert.SerializeObject(new { });         //빈 JSON 객체
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)        //실패 예러
            {
                Debug.LogError($"Error loging in : {request.error}");   //에러 로그
            }
            else
            {
                //응답을 처리하여 PlayerModel 생성
                string responseBody = request.downloadHandler.text;

                try
                {
                    var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                    //서버 응답에서 PlayerModel 생성
                    PlayerModel playerModel = new PlayerModel("")
                    {
                        metal = Convert.ToInt32(responseData["metal"]),
                        crystal = Convert.ToInt32(responseData["crystal"]),
                        deuterium = Convert.ToInt32(responseData["deuterium"])
                    };

                    onSuccess?.Invoke(playerModel); //PlayerModel 반환
                    Debug.Log("Login successful");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error processing login response: {ex.Message}");
                }
            }
        }
    }
}