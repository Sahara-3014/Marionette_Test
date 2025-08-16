using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HSJ_Loading : MonoBehaviour
{
    public static HSJ_Loading instance;

    public bool isGaugeVisible = true;
    [SerializeField] RectTransform gauge;
    [SerializeField] TextMeshProUGUI desriptionLabel;
    public List<string> descriptions = new();
    public float descriptionTime = 2f;

    [SerializeField] TextMeshProUGUI centerLabel;
    [SerializeField] string centerText = "Loading...";
    public float centerLabelAnimTime = 0.5f;
    GoogleSheetLoader sheetLoader;
    HSJ_LoopSpriteAnimation loopAnim;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        sheetLoader = GoogleSheetLoader.Instance;
        if (isGaugeVisible)
        {
            loopAnim = HSJ_LoopSpriteAnimation.instance;
            loopAnim.gameObject.SetActive(true);
            loopAnim.PlayAnim();

            LoopDescription();
        }
        else
        {
            var anim = HSJ_LoopSpriteAnimation.instance;
            anim.StopAnim();
            anim.gameObject.SetActive(false);

            desriptionLabel.gameObject.SetActive(false);
        }

        CenterLabelAnim();

        
        // intro start chapter1
        for(int i=0;i<sheetLoader.fixedSheetSequence.Count;i++)
        {
            sheetLoader.LoadDialoguesFromSheet(sheetLoader.fixedSheetSequence[i]);
        }
        // 탐색
        sheetLoader.LoadInvestigate();
        // 논쟁1.2
        sheetLoader.LoadInteractiveDebate();
        // 논쟁3
        sheetLoader.LoadConfrontationDebate();
    }

    private void Update()
    {
        if (sheetLoader != null && sheetLoader.progress != null)
        {
            float value = 0;
            foreach(var item in sheetLoader.progress)
                value += item.Value;
            value /= sheetLoader.progress.Count;

            GaugeSet(value);

            if(value >= 1f)
            {
                SaveDatabase.Instance.ChangeScene("HSJ_Lobby");
            }
        }
    }

    private void OnDisable()
    {
        //if (isGaugeVisible)
        //{
        //    loopAnim.StopAnim();
        //    GaugeSet(1f);
        //}
        //CancelInvoke(nameof(CenterLabelAnim));
        //CancelInvoke(nameof(LoopDescription));
        
    }

    public void CenterLabelAnim()
    {
        string[] split = centerText.Split("");

        int index = centerLabel.text.IndexOf("<size=150>");
        index = index < 0 || index >= centerText.Length -1 ? -1 : index;

        index += 1;
        string targetText = centerText.Substring(index, 1);

        centerLabel.text = centerText.Substring(0, index);

        centerLabel.text += $"<size=150>{targetText}</size>";

        centerLabel.text += centerText.Substring(index + 1, centerText.Length - index - 1);


        Invoke(nameof(CenterLabelAnim), centerLabelAnimTime);
    }


    public void GaugeSet(float value)
    {
        if(gauge.parent.gameObject.activeSelf != isGaugeVisible)
            gauge.parent.gameObject.SetActive(isGaugeVisible);
        if(!isGaugeVisible)
            return;

        float max = gauge.parent.GetComponent<RectTransform>().rect.width;
        max -= (gauge.localPosition.x * 2f);

        gauge.sizeDelta = new Vector2(max * value, gauge.sizeDelta.y);
    }

    public void LoopDescription()
    {
        if (desriptionLabel.gameObject.activeSelf != isGaugeVisible)
            desriptionLabel.gameObject.SetActive(isGaugeVisible);
        if (!isGaugeVisible)
            return;

        desriptionLabel.text = descriptions[Random.Range(0, descriptions.Count)];
        Invoke(nameof(LoopDescription), descriptionTime);
    }
}
