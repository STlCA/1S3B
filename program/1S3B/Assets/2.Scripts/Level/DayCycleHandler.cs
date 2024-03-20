using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class DayCycleHandler : MonoBehaviour
{

    [Header("Day Light")]
    public Light2D DayLight;
    public Gradient DayLightGradient;

    [Header("Night Light")]
    public Light2D NightLight;
    public Gradient NightLightGradient;

    [Header("Ambient Light")]
    public Light2D AmbientLight;
    public Gradient AmbientLightGradient;

    private void Awake()
    {
        GameManager.Instance.DayCycleHandler = this;
    }

    public void Tick()
    {
        UpdateLight(GameManager.Instance.CurrentDayRatio);
    }

    public void UpdateLight(float ratio)
    {
        DayLight.color = DayLightGradient.Evaluate(ratio);
        NightLight.color = NightLightGradient.Evaluate(ratio);


#if UNITY_EDITOR
        //the test between the define will only happen in editor and not in build, as it is assumed those will be set
        //in build. But in editor we may want to test without those set. (those were added later in development so
        //some test scene didn't have those set and we wanted to be able to still test those)
        if (AmbientLight != null)
#endif
            AmbientLight.color = AmbientLightGradient.Evaluate(ratio);

    }

  



#if UNITY_EDITOR
    [CustomEditor(typeof(DayCycleHandler))]
    class DayCycleEditor : Editor
    {
        private DayCycleHandler m_Target;

        public override VisualElement CreateInspectorGUI()
        {
            m_Target = target as DayCycleHandler;

            var root = new VisualElement();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            var slider = new Slider(0.0f, 1.0f);
            slider.label = "Test time 0:00";
            slider.RegisterValueChangedCallback(evt =>
            {
                m_Target.UpdateLight(evt.newValue);

                slider.label = $"Test Time {GameManager.GetTimeAsString(evt.newValue)} ({evt.newValue:F2})";
                SceneView.RepaintAll();
            });

            //registering click event, it's very catch all but not way to do a change check for control change
            root.RegisterCallback<ClickEvent>(evt =>
            {
                m_Target.UpdateLight(slider.value);
                SceneView.RepaintAll();
            });

            root.Add(slider);

            return root;
        }

    }
#endif
}
