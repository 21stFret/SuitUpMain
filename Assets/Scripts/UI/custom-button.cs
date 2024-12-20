using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomHoverButton : Button
{
    [Tooltip("Event triggered when hovering over the button")]
    public UnityEvent onHoverEnter;

    [Tooltip("Event triggered when stopping hovering over the button")]
    public UnityEvent onHoverExit;

    private bool isHovered = false;

    protected override void Awake()
    {
        base.Awake();
        if (onHoverEnter == null)
            onHoverEnter = new UnityEvent();
        if (onHoverExit == null)
            onHoverExit = new UnityEvent();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (!isHovered)
        {
            isHovered = true;
            onHoverEnter?.Invoke();
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (isHovered)
        {
            isHovered = false;
            onHoverExit?.Invoke();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (!isHovered)
        {
            isHovered = true;
            onHoverEnter?.Invoke();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (isHovered)
        {
            isHovered = false;
            onHoverExit?.Invoke();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CustomHoverButton))]
public class CustomHoverButtonEditor : UnityEditor.UI.ButtonEditor
{
    SerializedProperty onHoverEnterProperty;
    SerializedProperty onHoverExitProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        onHoverEnterProperty = serializedObject.FindProperty("onHoverEnter");
        onHoverExitProperty = serializedObject.FindProperty("onHoverExit");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(onHoverEnterProperty);
        EditorGUILayout.PropertyField(onHoverExitProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif