using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableManager : MonoBehaviour
{

    public static AddressableManager _instance;
    public static AddressableManager Instance {  get { return _instance;} set { _instance = value; } }

    //어드레서블 라벨
    [SerializeField] private List<AssetLabelReference> _label;

    private List<string> _labels;

    private long _downSize;

    //다운받을 레이블의 정보들을 모아놓은 자료
    private Coroutine _downLoadRoutine;

    private Coroutine _checkDownLoadRoutine;

    [SerializeField] private float _delayToStartCheckDownLoad;

    [SerializeField] private float _delayTofinishDownLoad;

    private WaitForSeconds _delayToStartCheckDownLoadWs;

    private WaitForSeconds _delayTofinishDownLoadWs;

    private bool _isInitialized;
    private bool _isInitializeFinished;
    public bool IsInitialized { get { return _isInitialized; } }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
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
        _delayToStartCheckDownLoadWs = new WaitForSeconds(_delayToStartCheckDownLoad);
        _delayTofinishDownLoadWs = new WaitForSeconds(_delayTofinishDownLoad);

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
        var init = Addressables.InitializeAsync(false); // 어드레서블 초기화 시작

        yield return init; //초기화 완료될떄까지 기다림

        _isInitialized = init.Status == AsyncOperationStatus.Succeeded;
        _isInitializeFinished = true;

        if (_isInitialized)
            Debug.Log("어드레서블 초기화 완료");
        else
            Debug.LogError($"어드레서블 초기화 실패: {init.OperationException}");

        if (init.IsValid())
            Addressables.Release(init);
    }

    public IEnumerator WaitUntilInitialized()
    {
        yield return new WaitUntil(() => _isInitializeFinished);
    }

    //단순 Object 생성
    public void GetObject(AssetReferenceGameObject assetObject, Action<GameObject> callBack)
    {
        var handle = assetObject.InstantiateAsync();

        handle.Completed += (obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded && obj.Result != null)
            {
                callBack?.Invoke(obj.Result);
            }
            else
            {
                Debug.LogError($"[AddressableManager] Object instantiate failed. Key: {assetObject.RuntimeKey}, Error: {obj.OperationException}");
                callBack?.Invoke(null);
            }
        };
    }

    // 주소로 단순 Object 생성
    public void GetObject(string address, Transform transform, Action<GameObject> callBack)
    {
        Addressables.InstantiateAsync(address, transform).Completed += (obj) =>
        {
            callBack(obj.Result);
        };
    }

    // 주소 존재 선검사 + 부모 지정 + 월드좌표 유지
    public void GetObject(string address, Transform parent, Action<GameObject> callBack, bool instantiateInWorldSpace = true)
    {
        var locHandle = Addressables.LoadResourceLocationsAsync(address);
        locHandle.Completed += locOp =>
        {
            bool exists = locOp.Status == AsyncOperationStatus.Succeeded
                          && locOp.Result != null
                          && locOp.Result.Count > 0;

            if (!exists)
            {
                Debug.LogError($"[AddressableManager] Address 키를 찾지 못했습니다: {address}");
                if (locHandle.IsValid()) Addressables.Release(locHandle);
                callBack?.Invoke(null);
                return;
            }

            var instHandle = Addressables.InstantiateAsync(address, parent, instantiateInWorldSpace);
            instHandle.Completed += objOp =>
            {
                if (objOp.Status == AsyncOperationStatus.Succeeded)
                {
                    callBack?.Invoke(objOp.Result);
                }
                else
                {
                    Debug.LogError($"Addressables Instantiate 실패 - address: {address}");
                    callBack?.Invoke(null);
                }

                if (locHandle.IsValid()) Addressables.Release(locHandle);
            };
        };
    }

    //단순 Object 생성 후 List에 저장
    public void GetObjectAndSave(AssetReferenceGameObject assetObject, List<GameObject> realObjects,  Action<GameObject> callBack)
    {
        var handle = assetObject.InstantiateAsync();

        handle.Completed += (obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded && obj.Result != null)
            {
                realObjects.Add(obj.Result);
                callBack?.Invoke(obj.Result);
            }
            else
            {
                Debug.LogError($"[AddressableManager] Object instantiate failed. Key: {assetObject.RuntimeKey}, Error: {obj.OperationException}");
                callBack?.Invoke(null);
            }
        };
    }


    //List에 저장된 Object들 생성 후 List에 저장
    // 이건 사용 x 함수  callback 호출시점이 애매
    public void GetObjectsAndSave(List<AssetReferenceGameObject> assetObjects, List<GameObject> realObjects, Action callBack)
    {
        for (int i = 0; i < assetObjects.Count; i++)
        {
            assetObjects[i].InstantiateAsync().Completed += (obj) =>
            {
                realObjects.Add(obj.Result);
                callBack();
            };
        }
    }

    //Sound 가져오기
    public void LoadSound(AssetReferenceT<AudioClip> assetAudioClip, AudioSource audio, Action callBack)
    {
        var handle = assetAudioClip.LoadAssetAsync();

        handle.Completed += (clip) =>
        {
            if (clip.Status == AsyncOperationStatus.Succeeded && clip.Result != null)
            {
                audio.clip = clip.Result;
                callBack?.Invoke();
            }
            else
            {
                Debug.LogError($"[AddressableManager] Sound load failed. Key: {assetAudioClip.RuntimeKey}, Error: {clip.OperationException}");
                callBack?.Invoke();
            }
        };
    }

    // 단순 Sound 가져오기
    public void LoadSound(AssetReferenceT<AudioClip> assetAudioClip, Action<AudioClip> callBack)
    {
        var handle = assetAudioClip.LoadAssetAsync();

        handle.Completed += (clip) =>
        {
            if (clip.Status == AsyncOperationStatus.Succeeded && clip.Result != null)
            {
                callBack?.Invoke(clip.Result);
            }
            else
            {
                Debug.LogError($"[AddressableManager] Sound load failed. Key: {assetAudioClip.RuntimeKey}, Error: {clip.OperationException}");
                callBack?.Invoke(null);
            }
        };
    }

    //Sprite 가져와서 이미지에 참조
    public void LoadSprite(AssetReferenceSprite assetImageSprite, Image image, Action callBack)
    {

        assetImageSprite.LoadAssetAsync().Completed += (img) =>
        {
            if (image == null)
            {
                Debug.LogError("스프라이트가 널임");
            }

            image.sprite = img.Result;
            image.gameObject.SetActive(true);
            callBack();
        };
    }

    //Sprite 가져와서 Sprite에 참조
    public void LoadOnlySprite(AssetReferenceSprite assetImageSprite,  Action<Sprite> callBack)
    {
        var handle = assetImageSprite.LoadAssetAsync();

        handle.Completed += (sprite) =>
        {
            if (sprite.Status == AsyncOperationStatus.Succeeded && sprite.Result != null)
            {
                callBack?.Invoke(sprite.Result);
            }
            else
            {
                Debug.LogError($"[AddressableManager] Sprite load failed. Key: {assetImageSprite.RuntimeKey}, Error: {sprite.OperationException}");
                callBack?.Invoke(null);
            }
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
    private List<object> GetDownloadKeys()
    {
        List<object> keys = new List<object>(_labels.Count);

        for (int i = 0; i < _labels.Count; i++)
            keys.Add(_labels[i]);

        return keys;
    }

    public void DoCheckDownLoadFile( Action<long> callback)
    {
        if (_checkDownLoadRoutine == null)
        {
            _checkDownLoadRoutine = StartCoroutine(CheckDownLoadFileWithCatalogUpdate(callback)); // 다운로드할 파일 있는지 확인
        }
    }

    IEnumerator CheckDownLoadFileWithCatalogUpdate(Action<long> callback)
    {
        Addressables.CheckForCatalogUpdates().Completed += (result) =>
        {
            var catalogToUpdate = result.Result;
            if (catalogToUpdate != null && catalogToUpdate.Count > 0)
            {
                var updateHandle = Addressables.UpdateCatalogs(catalogToUpdate);
                updateHandle.Completed += (updateResult) =>
                {
                    StartCoroutine(CheckDownLoadFIle(callback));
                    Debug.Log("카탈로그 업데이트완료");

                    if (updateHandle.IsValid())
                        updateHandle.Release();

                    if (result.IsValid())
                        result.Release();
                };
            }
            else
            {
                Debug.Log("카탈로그 업데이트할것이 없음");
                StartCoroutine(CheckDownLoadFIle(callback));

                if (result.IsValid())
                    result.Release();
            }
        };

        yield return null;
    }

    // CheckDownLoadFIle 코루틴에서 _downSize를 계산하고 콜백을 호출
    IEnumerator CheckDownLoadFIle( Action<long> callback)
    {
        _downSize = 0;

        yield return _delayToStartCheckDownLoadWs;

        var keys = GetDownloadKeys();
        var handle = Addressables.GetDownloadSizeAsync(keys);

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _downSize = handle.Result;
        }
        else
        {
            Debug.LogError($"다운로드 사이즈 확인 실패: {handle.OperationException}");
        }

        if (handle.IsValid())
            Addressables.Release(handle);

        Debug.Log($"다운로드 총용량: {_downSize}");
        callback(_downSize);

        _checkDownLoadRoutine = null;
    }

    //파일 사이즈 사이즈 크기에 맞는 단위로 표현하기 위한 함수
    public StringBuilder GetFileSize(long byteCnt)
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
    public void DownLoad(Slider downPercentSlider, TextMeshProUGUI downPercentText,  Action<bool> callback)
    {
        if (_downLoadRoutine == null)
        {
            _downLoadRoutine = StartCoroutine(OnDownLoad(downPercentSlider, downPercentText, callback));
        }
        //다운로드 다시 누르면 재수행
        else
        {
            StopCoroutine(_downLoadRoutine);
            _downLoadRoutine = null;
            _downLoadRoutine = StartCoroutine(OnDownLoad(downPercentSlider, downPercentText, callback));
        }
    }

    //다운로드 방식을 라벨별이 아닌 라벨 모두 합쳐서 중복제거하고 순서대로 받도록 수정.
    IEnumerator OnDownLoad(Slider downPercentSlider, TextMeshProUGUI downPercentText, Action<bool> callback)
    {
        StringBuilder sb = new StringBuilder();
        var keys = GetDownloadKeys();

        var sizeHandle = Addressables.GetDownloadSizeAsync(keys);
        yield return sizeHandle;

        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"다운로드 사이즈 확인 실패: {sizeHandle.OperationException}");

            if (sizeHandle.IsValid())
                Addressables.Release(sizeHandle);

            callback(false);
            _downLoadRoutine = null;
            yield break;
        }

        _downSize = sizeHandle.Result;

        if (sizeHandle.IsValid())
            Addressables.Release(sizeHandle);

        if (_downSize <= 0)
        {
            downPercentSlider.value = 1f;
            downPercentText.SetText("100 %");
            callback(true);
            _downLoadRoutine = null;
            yield break;
        }

        var downloadHandle = Addressables.DownloadDependenciesAsync(
            keys,
            Addressables.MergeMode.Union,
            false);

        while (!downloadHandle.IsDone)
        {
            var status = downloadHandle.GetDownloadStatus();
            float progress = status.TotalBytes > 0
                ? (float)status.DownloadedBytes / status.TotalBytes
                : downloadHandle.PercentComplete;

            progress = Mathf.Clamp01(progress);
            downPercentSlider.value = progress;

            sb.Clear();
            sb.Append(Mathf.FloorToInt(progress * 100f));
            sb.Append(" %");
            downPercentText.SetText(sb);

            yield return null;
        }

        bool success = downloadHandle.Status == AsyncOperationStatus.Succeeded;

        if (!success)
            Debug.LogError($"다운로드 실패: {downloadHandle.OperationException}");

        if (downloadHandle.IsValid())
            Addressables.Release(downloadHandle);

        if (success)
        {
            yield return _delayTofinishDownLoadWs;

            downPercentSlider.value = 1f;
            downPercentText.SetText("100 %");
        }

        callback(success);
        _downLoadRoutine = null;
    }
}
