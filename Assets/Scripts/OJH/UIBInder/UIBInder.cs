using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//UIBinding
//실제 게임에선 UI개수가 무지하게 많기 때문에, 
// 인스펙터상에서 참조하여 관리하기가 쉽지 않음.
// 때문에 스크립트상에서 바인딩하도록함.
public class UIBInder : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjectDic;
    private Dictionary<(string, System.Type), Component> componentDic;

    // 빠른 시간에 게임오브젝트만 바인딩
    protected void Bind()
    {
        //false로 하면 비활성화 컴포넌트는 안찾음, true는 비활성화 활성화 모두 찾음.
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        // 2* 2 크기를 4배로 일단.
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length << 2);
        foreach (Transform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        componentDic = new Dictionary<(string, System.Type), Component>();
    }

    // 비교적 오랜 시간에 게임오브젝트와 모든 컴포넌트 바인딩
    protected void BindAll()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length << 2);

        foreach (Transform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        Component[] components = GetComponentsInChildren<Component>(true);
        componentDic = new Dictionary<(string, System.Type), Component>(components.Length << 4);
        foreach (Component child in components)
        {
            componentDic.TryAdd((child.gameObject.name, components.GetType()), child);
        }
    }

    public void BindButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach(Button button in buttons)
        {
            button.onClick.AddListener(() => SoundManager.Instance.PlayUI("mousedown2"));
        }
    }

    public void BindButtonTexts()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                ChangeTextColor changeTextColor = button.gameObject.AddComponent<ChangeTextColor>();
                changeTextColor.buttonText = text;
            }
        }
    }

    // 이름이 name인 UI 게임오브젝트 가져오기
    // GetUI("Key") : Key 이름의 게임오브젝트 가져오기
    public GameObject GetUI(in string name)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        return gameObject;
    }

    // 이름이 name인 UI에서 컴포넌트 T 가져오기
    // GetUI<Image>("Key") : Key 이름의 게임오브젝트에서 Image 컴포넌트 가져오기
    public T GetUI<T>(in string name) where T : Component
    {
        (string, System.Type) key = (name, typeof(T));

        //컴포넌트 딕셔너리에 이미 있을때(찾아본 적 있는 경우): 이미 찾은걸 줌.
        componentDic.TryGetValue(key, out Component component);
        if (component != null)
            return component as T;
        // 컴포넌트 딕셔너리에 아직 없다면 찾은 후 딕셔너리에 추가하고 줌.
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return null;

        component = gameObject.GetComponent<T>();
        if (component == null)
            return null;

        componentDic.TryAdd(key, component);
        return component as T;
    }

    public enum EventType { Click, Enter, Exit, Up, Down, Move, BeginDrag, EndDrag, Drag, Drop }
    public void AddEvent(in string name, EventType eventType, UnityAction<PointerEventData> callback)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return;

        EventReceiver receiver = gameObject.GetOrAddComponent<EventReceiver>();
        switch (eventType)
        {
            case EventType.Click:
                receiver.OnClicked += callback;
                break;
            case EventType.Enter:
                receiver.OnEntered += callback;
                break;
            case EventType.Exit:
                receiver.OnExited += callback;
                break;
            case EventType.Up:
                receiver.OnUped += callback;
                break;
            case EventType.Down:
                receiver.OnDowned += callback;
                break;
            case EventType.Move:
                receiver.OnMoved += callback;
                break;
            case EventType.BeginDrag:
                receiver.OnBeginDraged += callback;
                break;
            case EventType.EndDrag:
                receiver.OnEndDraged += callback;
                break;
            case EventType.Drag:
                receiver.OnDraged += callback;
                break;
            case EventType.Drop:
                receiver.OnDroped += callback;
                break;

        }
    }

    public void RemoveEvent(in string name, EventType eventType, UnityAction<PointerEventData> callback)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return;

        EventReceiver receiver = gameObject.GetOrAddComponent<EventReceiver>();
        switch (eventType)
        {
            case EventType.Click:
                receiver.OnClicked -= callback;
                break;
            case EventType.Enter:
                receiver.OnEntered -= callback;
                break;
            case EventType.Exit:
                receiver.OnExited -= callback;
                break;
            case EventType.Up:
                receiver.OnUped -= callback;
                break;
            case EventType.Down:
                receiver.OnDowned -= callback;
                break;
            case EventType.Move:
                receiver.OnMoved -= callback;
                break;
            case EventType.BeginDrag:
                receiver.OnBeginDraged -= callback;
                break;
            case EventType.EndDrag:
                receiver.OnEndDraged -= callback;
                break;
            case EventType.Drag:
                receiver.OnDraged -= callback;
                break;
            case EventType.Drop:
                receiver.OnDroped -= callback;
                break;

        }
    }
}