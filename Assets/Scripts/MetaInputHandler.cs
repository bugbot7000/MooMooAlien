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
#if UNITY_EDITOR
        CheckForEditorInput();
#endif
        CheckQuestInput();
    }

    void CheckForEditorInput()
    {
        if (Input.GetKey(KeyCode.F))
        {
            _onLeftIndexPressed.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            _onLeftMidPressed.Invoke();
        }
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
    private void FixedUpdate()
    {
        OVRInput.FixedUpdate();
    }
}
