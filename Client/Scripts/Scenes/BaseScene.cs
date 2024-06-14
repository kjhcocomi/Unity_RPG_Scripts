using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

	void Awake()
	{
		Init();
	}

	protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        UI_Loading ui_Lodding = Managers.UI.ShowLoadingUI<UI_Loading>();

        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                ui_Lodding.SetProgressBarValue(Mathf.Lerp(ui_Lodding.GetProgressBarValue(), op.progress, timer));
                if (ui_Lodding.GetProgressBarValue() >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                ui_Lodding.SetProgressBarValue(Mathf.Lerp(ui_Lodding.GetProgressBarValue(), 1f, timer));
                if (ui_Lodding.GetProgressBarValue() == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
