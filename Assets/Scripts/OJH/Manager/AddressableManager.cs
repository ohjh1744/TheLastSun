using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AddressableManager : MonoBehaviour
{

    public static AddressableManager _instance;
    public static AddressableManager Instance {  get { return _instance;} set { _instance = value; } }

    //��巹���� ��
    [SerializeField] private List<AssetLabelReference> _label;

    private List<string> _labels;

    //�ٿ�ε� �� üũ ���� ������
    private Coroutine _downRoutine;

    private Coroutine _checkFileRoutine;

    [SerializeField] private float _delayToFinish;

    private WaitForSeconds _delayToFinishWs;

    private long _downSize;

    private Dictionary<string, long> _patchMap = new Dictionary<string, long>();
    


    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            Init();
            DontDestroyOnLoad(this);
            Debug.Log("��巹���� �ʱ�ȭ!");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        StartCoroutine(InitAddressable());
    }

    private void Init()
    {
        _labels = new List<string>();
        //�� ����
        for (int i = 0; i < _label.Count; i++)
        {
            _labels.Add(_label[i].labelString);
        }

        _delayToFinishWs = new WaitForSeconds(_delayToFinish);
    }

    // ��巹���� �ʱ�ȭ �ڵ�
    // �����൵ ������ Ȥ�ø� �һ�縦 ���� �߰�
    IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync(); // ��巹���� �ʱ�ȭ ����

        yield return init; //�ʱ�ȭ �Ϸ�ɋ����� ��ٸ�

        Debug.Log("��巹���� �ʱ�ȭ �Ϸ�");

    }

    //�ܼ� Object ����
    public void GetObject(AssetReferenceGameObject assetObject, GameObject realObject)
    {
        assetObject.InstantiateAsync().Completed += (obj) =>
        {
            realObject = obj.Result;
        };
    }


    //�ܼ� Object ���� �� List�� ����
    public void GetObjectAndSave(AssetReferenceGameObject assetObject, List<GameObject> realObjects)
    {
        assetObject.InstantiateAsync().Completed += (obj) =>
        {
            realObjects.Add(obj.Result);
        };
    }


    //List�� ����� Object�� ���� �� List�� ����
    public void GetObjectsAndSave(List<AssetReferenceGameObject> assetObjects, List<GameObject> realObjects)
    {
        for (int i = 0; i < assetObjects.Count; i++)
        {
            assetObjects[i].InstantiateAsync().Completed += (obj) =>
            {
                realObjects.Add(obj.Result);
            };
        }
    }

    //Sound ��������
    public void LoadSound(AssetReferenceT<AudioClip> assetAudioClip, AudioSource audio)
    {
        assetAudioClip.LoadAssetAsync().Completed += (clip) =>
        {
            audio.clip = clip.Result;
        };
    }

    //Sprite ��������
    public void LoadSprite(AssetReferenceSprite assetImageSprite, Image _image)
    {
        assetImageSprite.LoadAssetAsync().Completed += (img) =>
        {
            _image.sprite = img.Result;
        };
    }

    // ������ ���� ����
    public void ReleaseObject(AssetReference asset)
    {
        asset.ReleaseAsset();
    }

    //������ ���� ����
    public void ReleaseInstance(GameObject assetObjects)
    {
        Addressables.ReleaseInstance(assetObjects);
    }

    public void ReleaseInstances(List<GameObject> assetObjects)
    {
        for (int i = assetObjects.Count; i > 0; i--)
        {
            Addressables.ReleaseInstance(assetObjects[i - 1]);
            assetObjects.RemoveAt(i - 1);
        }
    }


    // �ٿ���� ���� ���� Ȯ��
    public void DoCheckDownLoadFile(TextMeshProUGUI downSizeText, TextMeshProUGUI downPercentText, Slider downPercentSlider, Button downButton, GameObject nextPanel)
    {
        if(_checkFileRoutine == null)
        {
            _checkFileRoutine = StartCoroutine(CheckDownLoadFIle(downSizeText, downPercentText, downPercentSlider, downButton, nextPanel)); //�ٿ���� �����ִ��� Ȯ��
        }
    }
    IEnumerator CheckDownLoadFIle(TextMeshProUGUI downSizeText, TextMeshProUGUI downPercentText, Slider downPercentSlider, Button downButton, GameObject nextPanel)
    {
        _downSize = 0;

        foreach (string label in _labels)
        {
            // �󺧺��� �ٿ�ε��� ������ ��������
            var handle = Addressables.GetDownloadSizeAsync(label);

            //  �۾��� �Ϸ�ɶ����� ��ٸ���
            yield return handle;

            // ���������� size�������� down�ε��ؾ��� ����� �߰����ֱ�.
            _downSize += handle.Result;
        }


        // 0���� ũ�ٸ� �ٿ���� ������ �����ϴٴ� ��
        if (_downSize > decimal.Zero)
        {
            downSizeText.SetText(GetFileSize(_downSize));
            downButton.interactable = true;
        }
        // �ٿ���� ������ �������� �ʴٸ�
        else
        {
            downSizeText.SetText("0 Bytes");
            downPercentText.SetText("100 %");
            downPercentSlider.value = 1f;

            yield return _delayToFinishWs;
            nextPanel.SetActive(true);
            Debug.Log("�ٿ���� ������ ����!!!");
        }

        _checkFileRoutine = null;
    }

    //���� ������ ������ ũ�⿡ �´� ������ ǥ���ϱ� ���� �Լ�
    StringBuilder GetFileSize(long byteCnt)
    {
        StringBuilder sb = new StringBuilder();

        Debug.Log($"�� ������: {byteCnt}");

        if ((byteCnt >= 1073741824.0))
        {
            sb.Append(string.Format("{0: ##.##}", byteCnt / 1073741824.0));
            sb.Append("GB");
        }
        else if ((byteCnt >= 1048576.0))
        {
            sb.Append(string.Format("{0: ##.##}", byteCnt / 1048576.0));
            sb.Append("MB");
        }
        else if ((byteCnt >= 1024.0))
        {
            sb.Append(string.Format("{0: ##.##}", byteCnt / 1024.0));
            sb.Append("KB");
        }
        else if ((byteCnt > 0 && byteCnt < 1024.0))
        {
            sb.Append(byteCnt.ToString());
            sb.Append("Bytes");
        }

        return sb;
    }

    public void DoDownLoad(Slider downPercentSlider, GameObject nextPanel, TextMeshProUGUI downPercentText)
    {
        if (_downRoutine == null)
        {
            _downRoutine = StartCoroutine(DownLoad(downPercentSlider, nextPanel, downPercentText));
        }
    }

    //�ٿ�ε� ����
    IEnumerator DownLoad(Slider downPercentSlider, GameObject nextPanel, TextMeshProUGUI downPercentText)
    {
        foreach (string label in _labels)
        {
            // �󺧺��� �ٿ�ε��� ������ ��������
            var handle = Addressables.GetDownloadSizeAsync(label);

            //  �۾��� �Ϸ�ɶ����� ��ٸ���
            yield return handle;

            // ��ġ�� ������ �ִٸ� �ٿ�ޱ�
            if (handle.Result != decimal.Zero)
            {
                StartCoroutine(DownLoadPerLabel(label));
            }
        }

        // �� ������ ���� �󺧺��� �ٿ��� �����ϰ�
        // �ٿ� ������ UI�� ǥ��
        yield return CheckDownLoadStatus(downPercentSlider, nextPanel, downPercentText);
    }

    // ��巹���� �� ���� �ٿ�ε� �ޱ�
    IEnumerator DownLoadPerLabel(string label)
    {
        _patchMap.Add(label, 0); // �����̺� ���� �ٿ� ����

        var handle = Addressables.DownloadDependenciesAsync(label, false);

        while (!handle.IsDone)
        {
            _patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;

            //�������Ӿ� ����ϸ鼭 �ݺ�
            yield return new WaitForEndOfFrame();
        }

        _patchMap[label] = handle.GetDownloadStatus().TotalBytes;

        Addressables.Release(handle);

        Debug.Log("�ϳ��� Label �ٿ!");
    }

    //���� �ٿ�ε� ��Ȳ �˷��ֱ�
    IEnumerator CheckDownLoadStatus(Slider downPercentSlider, GameObject nextPanel, TextMeshProUGUI downPercentText)
    {
        StringBuilder sb = new StringBuilder();
        long total = 0;

        while (true)
        {
            // �ٿ���� ���� ũ�� ���ϱ�
            total += _patchMap.Sum(tmp => tmp.Value);

            // �����̴��� ǥ��
            downPercentSlider.value = (float)total / (float)_downSize;

            // �ؽ�Ʈ�� ǥ��
            int curPatchValue = (int)(downPercentSlider.value * 100);
            sb.Clear();
            sb.Append(curPatchValue);
            sb.Append("%");
            downPercentText.SetText(sb);

            Debug.Log($"check ��! ���� {downPercentSlider.value}%, {total}Size��ŭ �ٿ����");

            //�ٿ�ε尡 �� �Ϸ� �Ѵٸ�
            if (total == _downSize)
            {
   
                yield return _delayToFinishWs;

                //���� Panel ���ֱ�
                nextPanel.SetActive(true);
                Debug.Log("�ٿ�ε� ��!");
                // �ٿ�ε� �ڷ�ƾ �ʱ�ȭ
                _downRoutine = null;
                break;
            }

            total = 0;
            yield return new WaitForEndOfFrame();

        }
    }
}
