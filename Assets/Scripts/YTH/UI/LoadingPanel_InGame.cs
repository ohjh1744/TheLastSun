using System.Collections;
using System.Text;
using TMPro;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel_InGame : UIBInder
{
    [SerializeField] float waitTime = 3f;

    private Slider _loadSlider;
    private TMP_Text _loadText;

    StringBuilder _sb = new StringBuilder();

    private Coroutine _fillRoutine;

    [SerializeField] TutorialManager _tutorialManager;

    private void Awake()
    {
        BindAll();

        _loadSlider = GetUI<Slider>("LoadingSlider");
        _loadText = GetUI<TMP_Text>("LoadingText");

        if (_loadText != null)
            _loadText.text = "0%";

        if (_loadSlider != null)
        {
            _loadSlider.value = 0f;
            _loadSlider.maxValue = 1f;
        }
    }

    private void Start()
    {
        if (_fillRoutine != null) StopCoroutine(_fillRoutine);
        _fillRoutine = StartCoroutine(FillSliderOverTime(waitTime));

        Invoke(nameof(HidePanel), waitTime);

       
    }

    private void OnDisable()
    {
        if (_fillRoutine != null)
        {
            StopCoroutine(_fillRoutine);
            _fillRoutine = null;
        }
    }

    private IEnumerator FillSliderOverTime(float duration)
    {
        if (_loadSlider == null || duration <= 0f) yield break;

        float t = 0f;
        _loadSlider.value = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(t / duration);
            _loadSlider.value = ratio;

            if (_loadText != null)
            {
                int percent = Mathf.RoundToInt(ratio * 100f);
                _sb.Clear();
                _sb.Append(percent).Append('%');
                _loadText.SetText(_sb);
            }

            yield return null;
        }

        _loadSlider.value = 1f;

        if (_loadText != null)
            _loadText.SetText("100%");

        _fillRoutine = null;
    }

   
    public void HidePanel()
    {
        gameObject.SetActive(false);
        _tutorialManager.SetTutorial();
    }
}
