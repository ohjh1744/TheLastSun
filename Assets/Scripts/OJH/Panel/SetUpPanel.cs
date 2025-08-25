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
    private TextMeshProUGUI _updateDetailText;
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


    private Coroutine _routine2;

    private bool _isUpdating;

    private void Awake()
    {
        BindAll();
        Init();
    }
    private void Start()
    {
        DoLogin();
    }

    private void OnApplicationFocus(bool focus)
    {
        //������ũ üũ�� �Ϸ�����ʾҵ��� ������ũ Ȯ�� ����
        if(focus == true && _isUpdating == false)
        {
            DoCheckUpdate();
        }

        //������Ʈ �߿� �ڷΰ��⸦ �ߴٸ�
        if (focus == true && _isUpdating == true)
        {
            DoQuitWhenBackInUpdate();
        }
    }
    private void Update()
    {
        // ��Ʈ��ũ����Ǿ� ���� �ʴ� ���
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
        _updateDetailText = GetUI<TextMeshProUGUI>("UpdateDetailText");

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
        if(_routine != null)
        {
            StopCoroutine(_routine);
        }
        _routine = StartCoroutine(CheckUpdate());
    }

    // ��Ʈ��ũ�� ����Ǿ��ִ��� Ȯ�� �� ����Ǹ� Update�����ϰ� ����
    IEnumerator CheckUpdate()
    {
        Debug.Log("������Ʈ üũ���ϴ�");
        while (true)
        {
            // ��Ʈ��ũ ����Ǿ��ִ� ���¿�����
            if (NetworkCheckManager.Instance.IsConnected == true)
            {
                GpgsManager.Instance.DoCheckForUpdate((status =>
                {
                    //������Ʈ �Ұ��� �־�, â�� �ߴ� ������ ���ϵ�.
                    if (status == UpdateAvailability.UpdateAvailable)
                    {
                        Debug.Log("������Ʈ �ؽ�Ʈ ��ȯ!");
                        _isUpdating = true;
                        Debug.Log($"isUpdating: {_isUpdating}");
                    }
                    //������Ʈ �Ұ��� ���ٸ� ���� ��
                    else if (status == UpdateAvailability.UpdateNotAvailable)
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
            yield return _checkCanUpdateRateWs;
        }

        _routine = null;
    }

    //������Ʈ �ϴ� ���߿� �ڷΰ����� �����ϱ�
    private void DoQuitWhenBackInUpdate()
    {
        if(_routine2 == null)
        {
            _routine2 = StartCoroutine(QuitWhenBackInUpdate());
        }
    }

    IEnumerator QuitWhenBackInUpdate()
    {
        _updateDetailText.text = "������Ʈ�� �����Ͽ� ���� ���� �˴ϴ�.";
        yield return new WaitForSeconds(2f);

        Application.Quit();
        _routine2 = null;
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
