using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // この行を追加

[System.Serializable]
public class SPARQLQueryResult
{
    public Head head;
    public Results results;

    [System.Serializable]
    public class Head
    {
        public string[] vars;
    }

    [System.Serializable]
    public class Results
    {
        public Binding[] bindings;

        [System.Serializable]
        public class Binding
        {
            public Value s;
            public Value p;
            public Value o;

            [System.Serializable]
            public class Value
            {
                public string type;
                public string value;
                public string datatype;
            }
        }
    }
}

public class GraphDBQuery : MonoBehaviour
{
    //public GameObject prefab;
    
    //public List<AudioClip> clips;

    void Start()
    {
        StartCoroutine(SendSPARQLQuery());
    }

    IEnumerator SendSPARQLQuery()
    {
        string repositoryID = "web360square-vue";
        string sparqlEndpoint = $"http://sdm.hongo.wide.ad.jp:7200/repositories/{repositoryID}";

        string sparqlQuery = @"
            PREFIX ign: <http://data.ign.fr/def/geometrie#>

            SELECT ?s ?p ?o
            WHERE {
                ?s ?p ?o .
                FILTER (
                    REGEX(STR(?s), '^http://sdm.hongo.wide.ad.jp/resource/Billboard170126TargetGeometry[123]$') &&
                    REGEX(STR(?p), '^http://data.ign.fr/def/geometrie#coord[XYZ]$')
                )
            }
        ";

        string url = $"{sparqlEndpoint}?query={UnityWebRequest.EscapeURL(sparqlQuery)}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Accept", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string result = www.downloadHandler.text;
                Debug.Log("SPARQL Query Result: " + result);

                // JSONデータをクラスにデシリアライズ
                SPARQLQueryResult queryResult = JsonUtility.FromJson<SPARQLQueryResult>(result);

                // SubjectごとにX、Y、Z座標をまとめるためのデータ構造
                Dictionary<string, Dictionary<string, string>> subjectData = new Dictionary<string, Dictionary<string, string>>();

                // 結果を解析して利用する処理を追加
                foreach (var binding in queryResult.results.bindings)
                {
                    string subjectLabel = GetAgentTargetLabel(binding.s.value);
                    string predicateLabel = GetCoordinateLabel(binding.p.value);

                    // Subjectごとのデータを取得または新規作成
                    if (!subjectData.ContainsKey(subjectLabel))
                    {
                        subjectData[subjectLabel] = new Dictionary<string, string>();
                    }

                    // X、Y、Z座標を追加
                    subjectData[subjectLabel][predicateLabel] = binding.o.value;
                }

                // 整理したデータをログに出力
                /*foreach (var subjectPair in subjectData)
                {
                    string subjectLabel = subjectPair.Key;
                    Dictionary<string, string> coordinates = subjectPair.Value;

                    // LINQを使用して文字列を生成
                    string coordinatesString = string.Join(", ", coordinates.Select(kv => $"{kv.Key}: {kv.Value}"));

                    Debug.Log($"Subject: {subjectLabel}, Coordinates: {coordinatesString}");

                    GameObject childObject = Instantiate(prefab, this.transform);

                    childObject.transform.localPosition = new Vector3(float.Parse(coordinates["X座標"]), float.Parse(coordinates["Y座標"]), float.Parse(coordinates["Z座標"]));
                    childObject.transform.localRotation = Quaternion.identity;
                    childObject.transform.localScale = new Vector3(1, 1, 1);
                    childObject.transform.name = subjectLabel;

                    AudioSource audio = childObject.GetComponent<AudioSource>();

                    audio.clip = clips[(int)Char.GetNumericValue(subjectLabel[subjectLabel.Length - 1])-1];
                    audio.Play();
                }*/

                Transform myTransform = this.transform;

                for (int i = 0; i < myTransform.childCount; i++)
                {
                    Transform childTransform = myTransform.GetChild(i);
                    string childName = childTransform.name;
                    Dictionary<string, string> coordinates = subjectData[childName];
                    Vector3 pos = childTransform.localPosition;
                    pos.x = float.Parse(coordinates["X座標"]);
                    pos.y = float.Parse(coordinates["Y座標"]);
                    pos.z = float.Parse(coordinates["Z座標"]);

                    childTransform.localPosition = pos;

                    /*AudioSource audio = childTransform.GetComponent<AudioSource>();

                    //audio.clip = clips[(int)Char.GetNumericValue(childName[childName.Length - 1])-1];
                    if (audio.playOnAwake)
                    {
                        audio.Play();
                    }*/
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    // AgentTargetのラベルを取得するメソッド
    string GetAgentTargetLabel(string subject)
    {
        string[] tokens = subject.Split('/');
        string lastToken = tokens[tokens.Length - 1];

        if (lastToken.EndsWith("1"))
        {
            return "AgentTarget1";
        }
        else if (lastToken.EndsWith("2"))
        {
            return "AgentTarget2";
        }
        else if (lastToken.EndsWith("3"))
        {
            return "AgentTarget3";
        }

        return "UnknownAgentTarget";
    }

    // Coordinateのラベルを取得するメソッド
    string GetCoordinateLabel(string predicate)
    {
        string[] tokens = predicate.Split('#');
        string lastToken = tokens[tokens.Length - 1];

        if (lastToken.EndsWith("X"))
        {
            return "X座標";
        }
        else if (lastToken.EndsWith("Y"))
        {
            return "Y座標";
        }
        else if (lastToken.EndsWith("Z"))
        {
            return "Z座標";
        }

        return "UnknownCoordinate";
    }
}