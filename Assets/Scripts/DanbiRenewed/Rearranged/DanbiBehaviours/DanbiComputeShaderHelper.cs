﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

using UnityEngine;

namespace Danbi {
  public static class DanbiComputeShaderHelper {
    public static void CreateComputeBuffer<T>(ComputeBuffer buffer, List<T> data, int stride)
      where T : struct {

      if (!buffer.Null()) {
        // if there's no data or buffer which doesn't match the given criteria, release it.
        if (data.Count == 0 || buffer.count != data.Count || buffer.stride != stride) {
          buffer.Release();
          buffer = null;
        }

        if (data.Count != 0) {
          // If the buffer has been released or wasn't there to begin with, create it!
          if (buffer.Null()) {
            buffer = new ComputeBuffer(data.Count, stride);
          }
          buffer.SetData(data);
        }
      }
    }

    public static void CreateComputeBuffer<T>(ComputeBuffer buffer, T data, int stride) 
      where T : struct {
      if (!buffer.Null()) {
        // if there's no data or buffer which doesn't match the given criteria, release it.
        if (buffer.stride != stride) {
          buffer.Release();
          buffer = null;
        }

        // If the buffer has been released or wasn't there to begin with, create it!
        if (buffer.Null()) {
          buffer = new ComputeBuffer(1, stride);
        }

        var list = new List<T>();
        list.Add(data);
        buffer.SetData(list);
      }
    }

    public static ComputeBuffer CreateComputeBuffer_Ret<T>(List<T> data, int stride)
      where T : struct {
      var res = new ComputeBuffer(data.Count, stride);
      res.SetData(data);
      return res;
    }

    public static ComputeBuffer CreateComputeBuffer_Ret<T>(T data, int stride)
      where T : struct {
      var res = new ComputeBuffer(1, stride);
      var list = new List<T>();
      list.Add(data);
      res.SetData(list);
      return res;
    }

    public static Matrix4x4 GetOpenCV_KMatrix(float alpha, float beta, float x0, float y0, float near, float far) {
      var perspec = new Matrix4x4();
      float A = near + far;
      float B = near * far;

      perspec[0, 0] = alpha;
      perspec[1, 1] = beta;
      perspec[0, 2] = -x0;
      perspec[1, 2] = -y0;
      perspec[2, 2] = A;
      perspec[2, 3] = B;
      perspec[3, 2] = -1.0f;
      return perspec;
    }

    public static Matrix4x4 GetOpenGL_KMatrix(float left, float right, float bottom, float top, float near, float far) {
      float m00 = 2.0f / (right - left);
      float m11 = 2.0f / (top - bottom);
      float m22 = 2.0f / (far - near);
      float tx = -(left + right) / (right - left);
      float ty = -(bottom + top) / (top - bottom);
      float tz = -(near + far) / (far - near);

      var ortho = new Matrix4x4();
      ortho[0, 0] = m00;
      ortho[1, 1] = m11;
      ortho[2, 2] = m22;
      ortho[0, 3] = tx;
      ortho[1, 3] = ty;
      ortho[2, 3] = tz;
      ortho[3, 3] = 1.0f;
      return ortho;
    }
  };
};

