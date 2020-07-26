﻿using System.Collections.Generic;

using UnityEngine;

namespace Danbi {
  [System.Serializable, RequireComponent(typeof(DanbiScreen), typeof(DanbiShaderControl), typeof(DanbiUIControl))]
  public class DanbiControl_Internal : MonoBehaviour {
    /// <summary>
    /// When this is true, then current renderTexture is transferred into the frame buffer.  
    /// </summary>
    [Readonly, SerializeField, Header("It toggled off to false after the image is saved.")]
    bool bStopRender = false;
    /// <summary>
    /// 
    /// </summary>
    [Readonly, SerializeField, Header("When this is true, then the current RenderTexture is used for render.")]
    bool bDistortionReady = false;

    DanbiScreen Screen;

    [SerializeField, Header("It affects to the Scene at editor-time and at run-time")]
    Texture2D TargetPanoramaTex;

    public Texture2D targetPanoramaTex { get => TargetPanoramaTex; set => TargetPanoramaTex = value; }

    List<PanoramaScreenObject> CurrentPanoramaList = new List<PanoramaScreenObject>();

    /// <summary>
    /// used to raytracing to obtain  distorted image and to project the distorted image onto the scene
    /// </summary>
    Camera MainCameraCache;

    DanbiShaderControl ShaderControl;


    [Readonly, SerializeField, Space(20)]
    EDanbiSimulatorMode SimulatorMode = EDanbiSimulatorMode.CAPTURE;

    DanbiUIControl UIControl;
    bool bCaptureFinished = false;

    public delegate void OnRenderFinished();
    public static OnRenderFinished Call_RenderFinished;

    public delegate void OnSaveImage();
    public static OnSaveImage Call_SaveImage;


    void Start() {
      // 1. Initialise the resources.
      Screen = GetComponent<DanbiScreen>();
      MainCameraCache = Camera.main;
      ShaderControl = GetComponent<DanbiShaderControl>();
      UIControl = GetComponent<DanbiUIControl>();

      DanbiImage.ScreenResolutions = Screen.screenResolution;
      DanbiDisableMeshFilterProps.DisableAllUnnecessaryMeshRendererProps();

      // 2. bind the call backs.
      Call_RenderFinished += Caller_RenderFinished;
      Call_SaveImage += Caller_SaveImage;

    }

    void OnValidate() {

    }

    void OnDisable() {
      // Dispose buffer resources.
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
      switch (SimulatorMode) {
        case EDanbiSimulatorMode.PREPARE:
          // Nothing to do on the prepare stage.
        return;

        case EDanbiSimulatorMode.CAPTURE: {
          // bStopRender is already true, but the result isn't saved yet (by button).
          // 
          // so we stop updating rendering but keep the screen with the result for preventing performance issue.          
          if (bDistortionReady) {
            Graphics.Blit(ShaderControl.resultRT, destination);
          } else {
            // 1. Calculate the resolution-wise thread size from the current screen resolution.
            // 2. and Dispatch.
            ShaderControl.Dispatch((Mathf.CeilToInt(Screen.screenResolution.x * 0.125f),
                                    Mathf.CeilToInt(Screen.screenResolution.y * 0.125f)),
                                      destination);
          }
        }
        break;
      }
    }


    void Caller_RenderFinished() {
      bDistortionReady = true;
    }

    void Caller_SaveImage() {

    }

  };
};
