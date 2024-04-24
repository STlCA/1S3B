/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public string UniqueSceneName;

    private void OnEnable()
    {
        GameManager.Instance.LoadedSceneData = this;
    }

    private void OnDisable()
    {
        if (GameManager.Instance?.LoadedSceneData == this)
            GameManager.Instance.LoadedSceneData = null;
    }
}
*/