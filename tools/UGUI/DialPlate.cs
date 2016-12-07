using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialPlate : MonoBehaviour {
    /// <summary>
    /// 表针
    /// </summary>
    [SerializeField]
    private RectTransform watchHand;
    /// <summary>
    /// 尺度
    /// </summary>
    [SerializeField]
    private float scale;
    /// <summary>
    /// 表盘阈值，即最大值
    /// </summary>
    [SerializeField]
    private float threshold;
    /// <summary>
    /// 当前值
    /// </summary>
    [SerializeField]
    private float current;

	void Update () {
        UpdateWatchHand();
    }
    
    public void Init(float threshold)
    {
        this.threshold = threshold;
    }

    private void UpdateWatchHand()
    {
        if(threshold <= 0)
        {
            threshold = 1;
        }
        float angle = 90 - 180 * current / threshold;
        watchHand.localEulerAngles = new Vector3(0f, 0f, Mathf.Clamp(angle, -90, 90));
    }
}
