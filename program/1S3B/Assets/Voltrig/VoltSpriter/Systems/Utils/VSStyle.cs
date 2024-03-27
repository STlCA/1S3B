using UnityEngine;

#pragma warning disable 0649
namespace Voltrig.VoltSpriter
{
    [CreateAssetMenu(fileName = nameof(VSStyle), menuName = nameof(Voltrig)+"/"+nameof(VoltSpriter)+"/"+nameof(VSStyle), order = 1)]
    internal partial class VSStyle : ScriptableObject
    {
        [SerializeField] internal VSStyleNormal norm;

        internal event System.Action OnValidateE;

        void OnValidate()
        {
            OnValidateE?.Invoke();
        }
    }

    [System.Serializable]
    internal class VSStyleNormal
    {
        public void ResetToDefault() 
        {
            
        }
    }
}

