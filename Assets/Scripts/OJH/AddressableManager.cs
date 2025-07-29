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

    // �ҷ��� ���µ�
    //[SerializeField] private AssetReferenceGameObject _playerObject;
    //[SerializeField] private AssetReferenceGameObject[] _monsterObjects;
    //[SerializeField] private AssetReferenceGameObject _coinObject;
    //[SerializeField] private AssetReferenceT<AudioClip> _bgmClip;
    //[SerializeField] private AssetReferenceSprite _imageSprite;


    //���� Objects��
    //[SerializeField] private GameObject _player;
    //[SerializeField] private List<GameObject> _monsterList = new List<GameObject>();
    //[SerializeField] private GameObject _coin;
    //[SerializeField] private AudioSource _bgm;
    //[SerializeField] private Image _image;

    //���� �ٿ������� �����ֱ� ���� UI
    [SerializeField] private GameObject _downPanel;
    [SerializeField] private TextMeshProUGUI _downPercentText;
    [SerializeField] private TextMeshProUGUI _downSizeText;
    [SerializeField] private Slider _downPercentSlider;
    [SerializeField] private Button _downButton;

    //��巹���� ��
    [SerializeField] private AssetLabelReference _defaultLabel;
    [SerializeField] private AssetLabelReference _imageLabel;
    //[SerializeField] private AssetLabelReference _soundLabel;


    private long _downSize;
    private Dictionary<string, long> _patchMap = new Dictionary<string, long>();

    Coroutine _routine;


    void Start()
    {
        StartCoroutine(InitAddressable());
    }

    // ��巹���� �ʱ�ȭ �ڵ�
    // �����൵ ������ Ȥ�ø� �һ�縦 ���� �߰�
    IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync(); // ��巹���� �ʱ�ȭ ����

        yield return init; //�ʱ�ȭ �Ϸ�ɋ����� ��ٸ�

        Debug.Log("��巹���� �ʱ�ȭ �Ϸ�");

        StartCoroutine(CheckDownLoadFIle()); //�ٿ���� �����ִ��� Ȯ��

    }

    //private void GetAssets()
    //{
    //    //InstantiateAsync -> Object �����Լ�.

    //    // Player
    //    _playerObject.InstantiateAsync().Completed += (obj) =>
    //    {
    //        _player = obj.Result;
    //    };

    //    //Monster
    //    for (int i = 0; i < _monsterObjects.Length; i++)
    //    {
    //        _monsterObjects[i].InstantiateAsync().Completed += (obj) =>
    //        {
    //            _monsterList.Add(obj.Result);
    //        };
    //    }

    //    //Coin
    //    _coinObject.InstantiateAsync().Completed += (obj) =>
    //    {
    //        _coin = obj.Result;
    //    };


    //    //LoadAssetAsync -> ���� ��������

    //    //Sound Image ������ͼ� AudioSource�� �ֱ�
    //    _bgmClip.LoadAssetAsync().Completed += (clip) =>
    //    {
    //        _bgm.clip = clip.Result;
    //        _bgm.loop = true;
    //        _bgm.Play();
    //    };

    //    //Image������ͼ� UI�� �߰��ϱ�
    //    _imageSprite.LoadAssetAsync().Completed += (img) =>
    //    {
    //        _image.sprite = img.Result;
    //    };

    //}

    //private void ReleaseAssets()
    //{
    //    // LoadAssetAsync <-> ReleaseAsset
    //    _bgmClip.ReleaseAsset();
    //    _imageSprite.ReleaseAsset();


    //    // InstantiateAsync <-> ReleaseInstance
    //    Addressables.ReleaseInstance(_player);
    //    for (int i = _monsterObjects.Length; i > 0; i--)
    //    {
    //        Addressables.ReleaseInstance(_monsterList[i - 1]);
    //        _monsterList.RemoveAt(i - 1);
    //    }
    //    Addressables.ReleaseInstance(_coin);
    //}

    // �ٿ���� ���� ���� Ȯ��
    IEnumerator CheckDownLoadFIle()
    {
        List<string> labels = new List<string>() { _defaultLabel.labelString, _imageLabel.labelString};

        _downSize = 0;

        foreach (string label in labels)
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
            _downSizeText.SetText(GetFileSize(_downSize));
            _downButton.interactable = true;
        }
        // �ٿ���� ������ �������� �ʴٸ�
        else
        {
            _downSizeText.SetText("0 Bytes");
            _downPercentText.SetText("100 %");
            _downPercentSlider.value = 1f;
            _downPanel.SetActive(false);
        }
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

    public void DoDownLoad()
    {
        if (_routine == null)
        {
            _routine = StartCoroutine(DownLoad());
        }
    }

    //�ٿ�ε� ����
    IEnumerator DownLoad()
    {
        List<string> labels = new List<string>() { _defaultLabel.labelString, _imageLabel.labelString};

        foreach (string label in labels)
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
        yield return CheckDownLoadStatus();
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
    IEnumerator CheckDownLoadStatus()
    {
        StringBuilder sb = new StringBuilder();
        long total = 0;

        while (true)
        {
            // �ٿ���� ���� ũ�� ���ϱ�
            total += _patchMap.Sum(tmp => tmp.Value);

            // �����̴��� ǥ��
            _downPercentSlider.value = (float)total / (float)_downSize;

            // �ؽ�Ʈ�� ǥ��
            int curPatchValue = (int)(_downPercentSlider.value * 100);
            sb.Clear();
            sb.Append(curPatchValue);
            sb.Append("%");
            _downPercentText.SetText(sb);

            Debug.Log($"check ��! ���� {_downPercentSlider.value}%, {total}Size��ŭ �ٿ����");

            //�ٿ�ε尡 �� �Ϸ� �Ѵٸ�
            if (total == _downSize)
            {
                // �ٿ�ε� �г� ���ֱ�
                _downPanel.SetActive(false);
                // �ٿ�ε� ��ƾ �ʱ�ȭ
                _routine = null;
                Debug.Log("�ٿ�ε� ��!");
                break;
            }

            total = 0;
            yield return new WaitForEndOfFrame();

        }
    }
}
