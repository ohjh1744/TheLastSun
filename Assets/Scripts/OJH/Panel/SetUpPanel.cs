using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetUpPanel : UIBInder
{
    //���� �ٿ������� �����ֱ� ���� UI
    private TextMeshProUGUI _downPercentText;
    private TextMeshProUGUI _downSizeText;
    private Slider _downPercentSlider;
    private Button _downLoadButton;

    [SerializeField] private GameObject _updatePanel;
    [SerializeField] private GameObject _checkDownLoadPanel;
    [SerializeField] private GameObject _doDownLoadPanel;
    [SerializeField] private GameObject _mainPanel;

    [SerializeField] private float _delayToStartUpdate;
    [SerializeField] private float _delayToFinishUpdate;
    [SerializeField] private float _delayToStartDownLoad;
    [SerializeField] private float _delayToFinishDownLoad;

    private Coroutine _routine;
    private WaitForSeconds _checkCanUpdateRateWs;
    [SerializeField] private float _checkCanUpdateRate;

    private bool _isCheckDownLoad;


    private void Awake()
    {
        BindAll();
        Init();
    }
    private void Start()
    {
        DoLogin();
        DoCheckUpdate();
    }
    private void Update()
    {
        // ��Ʈ��ũ����Ǿ� �ִ� ��츸 �Ʒ� ���� �����ϵ��� 
        if(NetworkCheckManager.Instance.IsConnected == false)
        {
            // true�� �س� ������ 
            // ��Ʈ��ũ �˾�â�� �߰� �Ǹ� �ٿ��ư�� �ڷ� �������µ�
            // �ٿ�ε� ��Ȳ���� ��Ʈ��ũ�� �����Ǿ��ٰ� �ٽ� ����Ǹ�, �ٿ�ε带 �ٽ� �Ҽ��ֵ��� �ϱ����ؼ�
            _downLoadButton.interactable = true;
            return;
        }

        //Updateüũ ������ DownLoadPanel�� ������ �׶� DownLoadüũ
        // �ٿ�ε� �ѹ������ϸ� ��ư �����
        if (_updatePanel.activeSelf == false && _isCheckDownLoad == false)
        {
            DoCheckDownLoad();
        }

    }

    private void Init()
    {
        //Ws �ʱ�ȭ
        _checkCanUpdateRateWs = new WaitForSeconds(_checkCanUpdateRate);

        //�̸� ���� �Ҵ�
        _downPercentText = GetUI<TextMeshProUGUI>("DownPercentText");
        _downSizeText = GetUI<TextMeshProUGUI>("DownSizeText");
        _downPercentSlider = GetUI<Slider>("DownPercentSlider");
        _downLoadButton = GetUI<Button>("DownLoadButton");

        //��ư �Լ� ����
        _downLoadButton.onClick.AddListener(() => DoDownLoad());

    }

    private void DoLogin()
    {
        GpgsManager.Instance.Login();
    }

    //�ٿ�ε� �������� Ȯ�ν���
    private void DoCheckUpdate()
    {
        if(_routine == null)
        {
            _routine = StartCoroutine(CheckUpdate());
        }
    }

    // ��Ʈ��ũ�� ����Ǿ��ִ��� Ȯ�� �� ����Ǹ� Update�����ϰ� ����
    IEnumerator CheckUpdate()
    {
        while (true)
        {
            // ��Ʈ��ũ ����Ǿ��ִ� ���¿�����
            if (NetworkCheckManager.Instance.IsConnected == true)
            {
                GpgsManager.Instance.DoCheckForUpdate(_updatePanel, _checkDownLoadPanel, _delayToStartUpdate, _delayToFinishUpdate);
                break;
            }
            yield return _checkCanUpdateRate;
        }

        _routine = null;
    }
    private void DoCheckDownLoad()
    {
        _isCheckDownLoad = true;
        AddressableManager.Instance.DoCheckDownLoadFile(_downSizeText, _checkDownLoadPanel, _doDownLoadPanel, _mainPanel, _delayToStartDownLoad);
    }

    private void DoDownLoad()
    {
        // �ٿ�ε� ���� �ϸ� ��ư ��ȣ�ۿ� ����
        _downLoadButton.interactable = false;
        AddressableManager._instance.DoDownLoad(_downPercentSlider, _doDownLoadPanel, _mainPanel, _downPercentText, _delayToFinishDownLoad);
    }





}
