using System;
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

    private long _downSize;

    //�ٿ���� ���̺��� �������� ��Ƴ��� �ڷ�
    private Dictionary<string, long> _patchMap = new Dictionary<string, long>();

    private Coroutine _downLoadRoutine;

    private Coroutine _checkDownLoadRoutine;

    [SerializeField] private float _delayToStartCheckDownLoad;

    [SerializeField] private float _delayTofinishDownLoad;

    private WaitForSeconds _delayToStartCheckDownLoadWs;

    private WaitForSeconds _delayTofinishDownLoadWs;



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
        _delayToStartCheckDownLoadWs = new WaitForSeconds(_delayToStartCheckDownLoad);
        _delayTofinishDownLoadWs = new WaitForSeconds(_delayTofinishDownLoad);

        //�� ����
        for (int i = 0; i < _label.Count; i++)
        {
            _labels.Add(_label[i].labelString);
        }

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
    public void GetObject(AssetReferenceGameObject assetObject, Action<GameObject> callBack)
    {
        assetObject.InstantiateAsync().Completed += (obj) =>
        {
            callBack(obj.Result);
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
    // �ٿ���� ������ ���ٸ� MainPanel �����ֱ�
    // MainPanel�� nextPanel
    public void DoCheckDownLoadFile( Action<long> callback)
    {
        if (_checkDownLoadRoutine == null)
        {
            _checkDownLoadRoutine = StartCoroutine(CheckDownLoadFIle(callback)); // �ٿ�ε��� ���� �ִ��� Ȯ��
        }
    }

    // CheckDownLoadFIle �ڷ�ƾ���� _downSize�� ����ϰ� �ݹ��� ȣ��
    IEnumerator CheckDownLoadFIle( Action<long> callback)
    {
        _downSize = 0;

        yield return _delayToStartCheckDownLoadWs;

        foreach (string label in _labels)
        {
            // �󺧺��� �ٿ�ε��� ������ ��������
            var handle = Addressables.GetDownloadSizeAsync(label);

            // �۾��� �Ϸ�� ������ ��ٸ���
            yield return handle;

            // ���������� size�� �������� �ٿ�ε��� ����� �߰����ֱ�
            _downSize += handle.Result;
        }

        // _downSize ���� �ݹ��� ���� ��ȯ
        callback(_downSize);

        _checkDownLoadRoutine = null;
    }

    //���� ������ ������ ũ�⿡ �´� ������ ǥ���ϱ� ���� �Լ�
    public StringBuilder GetFileSize(long byteCnt)
    {
        StringBuilder sb = new StringBuilder();

        Debug.Log($"�� ������: {byteCnt}");

        if ((byteCnt >= 1073741824.0))
        {
            sb.Append(string.Format("{0: ##.##}", byteCnt / 1073741824.0));
            sb.Append("Gb");
        }
        else if ((byteCnt >= 1048576.0))
        {
            sb.Append(string.Format("{0: ##.##}", byteCnt / 1048576.0));
            sb.Append("Mb");
        }
        else if ((byteCnt >= 1024.0))
        {
            sb.Append(string.Format("{0: ##.##}", byteCnt / 1024.0));
            sb.Append("Kb");
        }
        else if ((byteCnt > 0 && byteCnt < 1024.0))
        {
            sb.Append(byteCnt.ToString());
            sb.Append("Bytes");
        }

        return sb;
    }

    //�ٿ�ε� ����
    //�ٿ�ε� ������ MainPanel�� �̵�.
    //���⼭ nextPanel�� MainPanel
    public void DownLoad(Slider downPercentSlider, TextMeshProUGUI downPercentText,  Action<bool> callback)
    {
        if (_downLoadRoutine == null)
        {
            _downLoadRoutine = StartCoroutine(OnDownLoad(downPercentSlider, downPercentText, callback));
        }
        //�ٿ�ε� �ٽ� ������ �����
        else
        {
            StopCoroutine(_downLoadRoutine);
            _downLoadRoutine = null;
            _downLoadRoutine = StartCoroutine(OnDownLoad(downPercentSlider, downPercentText, callback));
        }
    }

    IEnumerator OnDownLoad(Slider downPercentSlider, TextMeshProUGUI downPercentText, Action<bool> callback)
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
                StartCoroutine(OnDownLoadPerLabel(label));
            }
        }

        // �� ������ ���� �󺧺��� �ٿ��� �����ϰ�
        // �ٿ� ������ UI�� ǥ��
        yield return OnCheckDownLoadStatus(downPercentSlider, downPercentText, callback);
    }

    // ��巹���� �� ���� �ٿ�ε� �ޱ�
    IEnumerator OnDownLoadPerLabel(string label)
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
    // 
    IEnumerator OnCheckDownLoadStatus(Slider downPercentSlider, TextMeshProUGUI downPercentText, Action<bool> finishDownLoadCallback)
    {
        StringBuilder sb = new StringBuilder();
        long total = 0;
        bool isFinishDownLad;

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
            sb.Append(" %");
            downPercentText.SetText(sb);

            Debug.Log($"check ��! ���� {downPercentSlider.value}%, {total}Size��ŭ �ٿ����");

            //�ٿ�ε尡 �� �Ϸ� �Ѵٸ�
            if (total == _downSize)
            {

                yield return _delayTofinishDownLoadWs;

                isFinishDownLad = true;
                finishDownLoadCallback(isFinishDownLad);
                Debug.Log("�ٿ�ε� ��!");
                break;
            }

            total = 0;
            yield return new WaitForEndOfFrame();

        }

        // �ٿ�ε� �ڷ�ƾ �ʱ�ȭ
        _downLoadRoutine = null;
    }
}
