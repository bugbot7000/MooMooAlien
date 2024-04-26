using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR.Input;
using UnityEngine.Events;

public class MetaInputHandler : MonoBehaviour
{
    #region META QUEST CONTROLLER EVENTS
    //[SerializeField]
    UnityEvent _onAPressed;
    //[SerializeField]
    UnityEvent _onBPressed;
    //[SerializeField]
    UnityEvent _onXPressed;
    //[SerializeField]
    UnityEvent _onYPressed;
    //[SerializeField]
    UnityEvent _onMenuPressed;
    [SerializeField]
    UnityEvent _onLeftIndexPressed;
    [SerializeField]
    UnityEvent _onRightIndexPressed;
    [SerializeField]
    UnityEvent _onLeftMidPressed;
    [SerializeField]
    UnityEvent _onRightMidPressed;
    #endregion
    
    public OVRInput.Controller lController, rController;
    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        CheckForEditorInput();
        CheckQuestInput();
    }

    void CheckForMetaInput()
    {
        
    }

    public void TestInvoker()
    {
        Debug.Log("Hello");
    }

    void CheckQuestInput()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            _onAPressed.Invoke();
        }       
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger,lController) >0.5f)
        {
            _onLeftIndexPressed.Invoke();
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger,lController) >0.5f)
        {
            _onLeftMidPressed.Invoke();
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger,rController) >0.5f)
        {
            _onRightIndexPressed.Invoke();
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger,rController) >0.5f)
        {
            _onRightMidPressed.Invoke();
        }
    }
    void CheckForEditorInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _onAPressed.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Plus))
        {
            
        }
    }
    private void FixedUpdate()
    {
        OVRInput.FixedUpdate();
    }
}
