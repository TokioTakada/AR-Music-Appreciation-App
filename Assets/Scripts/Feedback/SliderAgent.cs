using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SliderAgent : Agent
{
    public TMP_Dropdown dropdown;
    public Slider slider;
    public Button decisionButton;
    public AudioMixer audioMixer;

    // Audio Mixer のパラメータ名を修正（Inspectorの表記に合わせる）
    private const string wetnessParam = "Wetness Adjust";
    private const string rT60Param = "RT60 Scale";
    private const string spatializerParam = "Spatializer";

    private float wetnessAdjust;
    private float rT60Scale;
    private float spatializer;

    private bool isTraining = false;

    public override void Initialize()
    {
        UpdateParametersFromAudioMixer();

        // ドロップダウンの選択肢変更時の処理
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // ボタンを押すと学習（決定をリクエスト）
        decisionButton.onClick.AddListener(OnDecisionButtonClicked);
    }

    private void OnDropdownValueChanged(int value)
    {
        if (value == 2) // 3つ目の選択肢が選ばれたとき
        {
            isTraining = true;
            Debug.Log("学習開始");
            EndEpisode(); // 新しいエピソードを開始（学習開始）
        }
        else if (isTraining) // 学習中に他の選択肢が選ばれたとき
        {
            isTraining = false;
            Debug.Log("学習停止と最適パラメータ適用");
            RequestDecision(); // 学習結果から最適なパラメータを適用
        }
    }

    private void OnDecisionButtonClicked()
    {
        Debug.Log("Decision button clicked"); // ボタンが押されたことを確認
        if (isTraining)
        {
            Debug.Log("Requesting decision"); // `RequestDecision`が呼ばれるか確認
            RequestDecision(); // ボタンが押されるたびにエージェントが行動
        }
    }

    public override void OnEpisodeBegin()
    {
        if (!isTraining) // 学習していないときのみAudio Mixerのパラメータを取得
        {
            UpdateParametersFromAudioMixer();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(slider.value);
        sensor.AddObservation(NormalizeWetness(wetnessAdjust));
        sensor.AddObservation(NormalizeRT60(rT60Scale));
        sensor.AddObservation(spatializer);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (!isTraining) return;

        Debug.Log("OnActionReceived called"); // `OnActionReceived()` が呼ばれているか確認

        float actionWetness = actionBuffers.ContinuousActions[0];
        float actionRT60 = actionBuffers.ContinuousActions[1];
        float actionSpatializer = actionBuffers.ContinuousActions[2];

        wetnessAdjust = DenormalizeWetness(actionWetness);
        rT60Scale = DenormalizeRT60(actionRT60);
        spatializer = Mathf.Clamp(actionSpatializer, 0.0f, 1.0f);

        // Audio Mixer に新しい値を適用
        audioMixer.SetFloat(wetnessParam, wetnessAdjust);
        audioMixer.SetFloat(rT60Param, rT60Scale);
        audioMixer.SetFloat(spatializerParam, spatializer);

        Debug.Log($"Updated Parameters: Wetness={wetnessAdjust}, RT60={rT60Scale}, Spatializer={spatializer}");

        // 報酬を設定（Sliderの値が0で1、1で-1）
        float reward = 1.5f - 3.0f * slider.value; // スケールを拡大
        SetReward(reward);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        /*
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Random.Range(-1.0f, 1.0f);
        continuousActions[1] = Random.Range(0.0f, 1.0f);
        continuousActions[2] = Random.Range(0.0f, 1.0f);
        */
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = 0.0f; // 固定値
        continuousActions[1] = 1.0f;
        continuousActions[2] = 0.5f;
    }

    private void UpdateParametersFromAudioMixer()
    {
        audioMixer.GetFloat(wetnessParam, out wetnessAdjust);
        audioMixer.GetFloat(rT60Param, out rT60Scale);
        audioMixer.GetFloat(spatializerParam, out spatializer);
        Debug.Log($"Audio Mixer Updated: Wetness={wetnessAdjust}dB, RT60={rT60Scale}, Spatializer={spatializer}");
    }

    private float NormalizeWetness(float value) => value / 20.0f;
    private float DenormalizeWetness(float value) => Mathf.Clamp(value * 20.0f, -20.0f, 20.0f);
    private float NormalizeRT60(float value) => value / 2.0f;
    private float DenormalizeRT60(float value) => Mathf.Clamp(value * 2.0f, 0.0f, 2.0f);
}
