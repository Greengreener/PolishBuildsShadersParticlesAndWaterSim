using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangingBetweenThings : MonoBehaviour
{
    [SerializeField] int sceneNumber;
    [SerializeField] int activeShader;
    [SerializeField] GameObject[] Shaders ;
    [SerializeField] int activeEffect;
    [SerializeField] GameObject[] Effects;

    [SerializeField] GameObject hologram;
    public bool IsEffects;

    private void Start() {
        switch(IsEffects)
        {
            case true: 
                Effects[0].SetActive(true);
                activeEffect = 0;
            break;
            case false:
                Shaders[0].SetActive(true);
                activeShader = 0;
            break;
        }
    }
    private void Update() {
        if(!IsEffects){
            hologram.transform.RotateAround(hologram.transform.position,new Vector3(0,1,0), 0.1f);
        }
    }
    public void ChangeEffect(){
        Effects[activeEffect].SetActive(false);
        activeEffect++;
        if(activeEffect >= Effects.Length) activeEffect = 0;
        Effects[activeEffect].SetActive(true);
    }
    public void ChangeShader(){
        Shaders[activeShader].SetActive(false);
        activeShader++;
        if(activeShader >= Shaders.Length) activeShader = 0;
        Shaders[activeShader].SetActive(true);
    }
    
    public void SceneChange(int _newSceneInt){
        SceneManager.LoadScene(_newSceneInt);
    }
}
