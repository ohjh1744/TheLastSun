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

    // 불러올 에셋들
    //[SerializeField] private AssetReferenceGameObject _playerObject;
    //[SerializeField] private AssetReferenceGameObject[] _monsterObjects;
    //[SerializeField] private AssetReferenceGameObject _coinObject;
    //[SerializeField] private AssetReferenceT<AudioClip> _bgmClip;
    //[SerializeField] private AssetReferenceSprite _imageSprite;


    //실제 Objects들
    //[SerializeField] private GameObject _player;
    //[SerializeField] private List<GameObject> _monsterList = new List<GameObject>();
    //[SerializeField] private GameObject _coin;
    //[SerializeField] private AudioSource _bgm;
    //[SerializeField] private Image _image;

    //현재 다운진행을 보여주기 위한 UI
    [SerializeField] private GameObject _downPanel;
    [SerializeField] private TextMeshProUGUI _downPercentText;
    [SerializeField] private TextMeshProUGUI _downSizeText;
    [SerializeField] private Slider _downPercentSlider;
    [SerializeField] private Button _downButton;

    //어드레서블 라벨
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

    // 어드레서블 초기화 코드
    // 안해줘도 되지만 혹시모를 불상사를 위해 추가
    IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync(); // 어드레서블 초기화 시작

        yield return init; //초기화 완료될떄까지 기다림

        Debug.Log("어드레서블 초기화 완료");

        StartCoroutine(CheckDownLoadFIle()); //다운받을 파일있는지 확인

    }

    //private void GetAssets()
    //{
    //    //InstantiateAsync -> Object 생성함수.

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


    //    //LoadAssetAsync -> 에셋 가져오기

    //    //Sound Image 가지고와서 AudioSource에 넣기
    //    _bgmClip.LoadAssetAsync().Completed += (clip) =>
    //    {
    //        _bgm.clip = clip.Result;
    //        _bgm.loop = true;
    //        _bgm.Play();
    //    };

    //    //Image가지고와서 UI에 추가하기
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

    // 다운받을 파일 여부 확인
    IEnumerator CheckDownLoadFIle()
    {
        List<string> labels = new List<string>() { _defaultLabel.labelString, _imageLabel.labelString};

        _downSize = 0;

        foreach (string label in labels)
        {
            // 라벨별로 다운로드할 사이즈 가져오기
            var handle = Addressables.GetDownloadSizeAsync(label);

            //  작업이 완료될때까지 기다리기
            yield return handle;

            // 정상적으로 size가져오면 down로드해야할 사이즈에 추가해주기.
            _downSize += handle.Result;
        }


        // 0보다 크다면 다운받을 파일이 존재하다는 것
        if (_downSize > decimal.Zero)
        {
            _downSizeText.SetText(GetFileSize(_downSize));
            _downButton.interactable = true;
        }
        // 다운받을 파일이 존재하지 않다면
        else
        {
            _downSizeText.SetText("0 Bytes");
            _downPercentText.SetText("100 %");
            _downPercentSlider.value = 1f;
            _downPanel.SetActive(false);
        }
    }

    //파일 사이즈 사이즈 크기에 맞는 단위로 표현하기 위한 함수
    StringBuilder GetFileSize(long byteCnt)
    {
        StringBuilder sb = new StringBuilder();

        Debug.Log($"총 사이즈: {byteCnt}");

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

    //다운로드 시작
    IEnumerator DownLoad()
    {
        List<string> labels = new List<string>() { _defaultLabel.labelString, _imageLabel.labelString};

        foreach (string label in labels)
        {
            // 라벨별로 다운로드할 사이즈 가져오기
            var handle = Addressables.GetDownloadSizeAsync(label);

            //  작업이 완료될때까지 기다리기
            yield return handle;

            // 패치할 내용이 있다면 다운받기
            if (handle.Result != decimal.Zero)
            {
                StartCoroutine(DownLoadPerLabel(label));
            }
        }

        // 위 포문을 통해 라벨별로 다운을 시작하고
        // 다운 과정을 UI로 표시
        yield return CheckDownLoadStatus();
    }

    // 어드레서블 라벨 별로 다운로드 받기
    IEnumerator DownLoadPerLabel(string label)
    {
        _patchMap.Add(label, 0); // 각레이블에 대한 다운 상태

        var handle = Addressables.DownloadDependenciesAsync(label, false);

        while (!handle.IsDone)
        {
            _patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;

            //한프레임씩 대기하면서 반복
            yield return new WaitForEndOfFrame();
        }

        _patchMap[label] = handle.GetDownloadStatus().TotalBytes;

        Addressables.Release(handle);

        Debug.Log("하나의 Label 다운끝!");
    }

    //현재 다운로드 상황 알려주기
    IEnumerator CheckDownLoadStatus()
    {
        StringBuilder sb = new StringBuilder();
        long total = 0;

        while (true)
        {
            // 다운받은 파일 크기 구하기
            total += _patchMap.Sum(tmp => tmp.Value);

            // 슬라이더에 표시
            _downPercentSlider.value = (float)total / (float)_downSize;

            // 텍스트에 표시
            int curPatchValue = (int)(_downPercentSlider.value * 100);
            sb.Clear();
            sb.Append(curPatchValue);
            sb.Append("%");
            _downPercentText.SetText(sb);

            Debug.Log($"check 중! 현재 {_downPercentSlider.value}%, {total}Size만큼 다운받음");

            //다운로드가 다 완료 됏다면
            if (total == _downSize)
            {
                // 다운로드 패널 꺼주기
                _downPanel.SetActive(false);
                // 다운로드 루틴 초기화
                _routine = null;
                Debug.Log("다운로드 끝!");
                break;
            }

            total = 0;
            yield return new WaitForEndOfFrame();

        }
    }
}
