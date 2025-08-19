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

    //어드레서블 라벨
    [SerializeField] private List<AssetLabelReference> _label;

    private List<string> _labels;

    //다운로드 및 체크 관련 변수들
    private Coroutine _downRoutine;

    private Coroutine _checkFileRoutine;

    private long _downSize;

    private Dictionary<string, long> _patchMap = new Dictionary<string, long>();
    

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            Init();
            DontDestroyOnLoad(this);
            Debug.Log("어드레서블 초기화!");
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
        //라벨 설정
        for (int i = 0; i < _label.Count; i++)
        {
            _labels.Add(_label[i].labelString);
        }

    }

    // 어드레서블 초기화 코드
    // 안해줘도 되지만 혹시모를 불상사를 위해 추가
    IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync(); // 어드레서블 초기화 시작

        yield return init; //초기화 완료될떄까지 기다림

        Debug.Log("어드레서블 초기화 완료");

    }

    //단순 Object 생성
    public void GetObject(AssetReferenceGameObject assetObject, GameObject realObject)
    {
        assetObject.InstantiateAsync().Completed += (obj) =>
        {
            realObject = obj.Result;
        };
    }


    //단순 Object 생성 후 List에 저장
    public void GetObjectAndSave(AssetReferenceGameObject assetObject, List<GameObject> realObjects)
    {
        assetObject.InstantiateAsync().Completed += (obj) =>
        {
            realObjects.Add(obj.Result);
        };
    }


    //List에 저장된 Object들 생성 후 List에 저장
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

    //Sound 가져오기
    public void LoadSound(AssetReferenceT<AudioClip> assetAudioClip, AudioSource audio)
    {
        assetAudioClip.LoadAssetAsync().Completed += (clip) =>
        {
            audio.clip = clip.Result;
        };
    }

    //Sprite 가져오기
    public void LoadSprite(AssetReferenceSprite assetImageSprite, Image _image)
    {
        assetImageSprite.LoadAssetAsync().Completed += (img) =>
        {
            _image.sprite = img.Result;
        };
    }

    // 가져온 에셋 해제
    public void ReleaseObject(AssetReference asset)
    {
        asset.ReleaseAsset();
    }

    //생성한 에셋 해제
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


    // 다운받을 파일 여부 확인
    // 다운받을 파일이 없다면 MainPanel 열어주기
    // MainPanel이 nextPanel
    public void DoCheckDownLoadFile(TextMeshProUGUI downSizeText, GameObject checkDownLoadPanel, GameObject doDownLoadPanel, GameObject mainPanel, float _delayToCheckDownLoad)
    {
        if(_checkFileRoutine == null)
        {
            _checkFileRoutine = StartCoroutine(CheckDownLoadFIle(downSizeText, checkDownLoadPanel, doDownLoadPanel,  mainPanel, _delayToCheckDownLoad)); //다운받을 파일있는지 확인
        }
    }
    IEnumerator CheckDownLoadFIle(TextMeshProUGUI downSizeText, GameObject checkDownLoadPanel, GameObject doDownLoadPanel, GameObject mainPanel, float _delayToCheckDownLoad)
    {
        _downSize = 0;

        yield return new WaitForSeconds(_delayToCheckDownLoad);

        foreach (string label in _labels)
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
            //CheckDownLoad패널 닫아주고, 다운로드패널 열어주기
            checkDownLoadPanel.SetActive(false);
            doDownLoadPanel.SetActive(true);
            downSizeText.SetText(GetFileSize(_downSize));
        }
        // 다운받을 파일이 존재하지 않다면
        else
        {
            //CheckDownLoad패널 닫아주고, 바로 메인패널 열어주기
            checkDownLoadPanel.SetActive(false);
            mainPanel.SetActive(true);
            Debug.Log("다운받을 파일이 없음!!!");
        }

        _checkFileRoutine = null;
    }

    //파일 사이즈 사이즈 크기에 맞는 단위로 표현하기 위한 함수
    StringBuilder GetFileSize(long byteCnt)
    {
        StringBuilder sb = new StringBuilder();

        Debug.Log($"총 사이즈: {byteCnt}");

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

    //다운로드 시작
    //다운로드 끝나면 MainPanel로 이동.
    //여기서 nextPanel은 MainPanel
    public void DoDownLoad(Slider downPercentSlider, GameObject doDownLoadPanel, GameObject mainPanel, TextMeshProUGUI downPercentText, float _delayToFinishDownLoad)
    {
        if (_downRoutine == null)
        {
            _downRoutine = StartCoroutine(DownLoad(downPercentSlider, doDownLoadPanel, mainPanel, downPercentText, _delayToFinishDownLoad));
        }
        //다운로드 다시 누르면 재수행
        else
        {
            StopCoroutine(_downRoutine);
            _downRoutine = null;
            _downRoutine = StartCoroutine(DownLoad(downPercentSlider, doDownLoadPanel, mainPanel, downPercentText, _delayToFinishDownLoad));
        }
    }

    IEnumerator DownLoad(Slider downPercentSlider, GameObject doDownLoadPanel,GameObject mainPanel, TextMeshProUGUI downPercentText, float _delayToFinishDownLoad)
    {
        foreach (string label in _labels)
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
        yield return CheckDownLoadStatus(downPercentSlider, doDownLoadPanel, mainPanel, downPercentText, _delayToFinishDownLoad);
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
    IEnumerator CheckDownLoadStatus(Slider downPercentSlider, GameObject doDownLoadPanel, GameObject mainPanel, TextMeshProUGUI downPercentText, float _delayToFinishDownLoad)
    {
        StringBuilder sb = new StringBuilder();
        long total = 0;

        while (true)
        {
            // 다운받은 파일 크기 구하기
            total += _patchMap.Sum(tmp => tmp.Value);

            // 슬라이더에 표시
            downPercentSlider.value = (float)total / (float)_downSize;

            // 텍스트에 표시
            int curPatchValue = (int)(downPercentSlider.value * 100);
            sb.Clear();
            sb.Append(curPatchValue);
            sb.Append(" %");
            downPercentText.SetText(sb);

            Debug.Log($"check 중! 현재 {downPercentSlider.value}%, {total}Size만큼 다운받음");

            //다운로드가 다 완료 됏다면
            if (total == _downSize)
            {

                yield return new WaitForSeconds(_delayToFinishDownLoad);

                //DoDownLoadPanel 켜주기
                doDownLoadPanel.SetActive(false);
                mainPanel.SetActive(true);
                Debug.Log("다운로드 끝!");
                break;
            }

            total = 0;
            yield return new WaitForEndOfFrame();

        }

        // 다운로드 코루틴 초기화
        _downRoutine = null;
    }
}
