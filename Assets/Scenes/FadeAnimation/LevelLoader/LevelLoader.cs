using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public void StartGame()
    {
        StartCoroutine(LoadFirstLevel());
    }
    public void StartMakeGraffiti()
    {
        StartCoroutine(LoadMakeGraffiti());
    }
    public void CloseGame()
    {
        Application.Quit();
    }
    IEnumerator LoadFirstLevel()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }
    IEnumerator LoadMakeGraffiti()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2);
    }
}
