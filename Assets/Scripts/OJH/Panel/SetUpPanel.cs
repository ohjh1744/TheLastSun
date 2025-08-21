using Google.Play.AppUpdate;
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

    private Coroutine _routine;
    //UpdateȮ�� ���� Network������ Ȯ���ϱ� ���� �ֱ�
    private WaitForSeconds _checkCanUpdateRateWs;
    [SerializeField] private float _checkCanUpdateRate;

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
                GpgsManager.Instance.DoCheckForUpdate((status =>
                {
                    if (status == UpdateAvailability.UpdateNotAvailable)
                    {
                        // ���� Panel�� UpdatePanel �ݰ�, ���� Panel�� DownPanel ����
                        _updatePanel?.SetActive(false);
                        _checkDownLoadPanel?.SetActive(true);
                        //Update Ȯ�� ������ DownLoad Ȯ�� ����
                        DoCheckDownLoad();
                    }
                }));
                break;
            }
            yield return _checkCanUpdateRate;
        }

        _routine = null;
    }
    private void DoCheckDownLoad()
    {
        AddressableManager.Instance.DoCheckDownLoadFile((downSIze) =>{
            // �ٿ�ε��� ������ �����ϸ� �ٿ�ε� �г��� ����
            if (downSIze > decimal.Zero)
            {
                // CheckDownLoad �г��� �ݰ�, �ٿ�ε� �г��� ����, �ٿ���� �뷮 Text ���� Update.
                _checkDownLoadPanel.SetActive(false);
                _doDownLoadPanel.SetActive(true);
                _downSizeText.SetText(AddressableManager.Instance.GetFileSize(downSIze));
            }
            // �ٿ���� ������ �������� ������ ���� �г��� ����
            else
            {
                // CheckDownLoad �г��� �ݰ�, �ٷ� ���� �г��� ����
                _checkDownLoadPanel.SetActive(false);
                _mainPanel.SetActive(true);
                Debug.Log("�ٿ���� ������ ����!!!");
            }
        });
    }

    private void DoDownLoad()
    {
        // �ٿ�ε� ���� �ϸ� ��ư ��ȣ�ۿ� ����
        _downLoadButton.interactable = false;

        //�ٿ�ε尡 ������ ������ 
        AddressableManager._instance.DoDownLoad(_downPercentSlider, _downPercentText, (isDownFinish) =>
        {
            if (isDownFinish == true)
            {
                //DoDownLoadPanel ���ֱ�
                _doDownLoadPanel.SetActive(false);
                _mainPanel.SetActive(true);
            }
        });
    }





}
