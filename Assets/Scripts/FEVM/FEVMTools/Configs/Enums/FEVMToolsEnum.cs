using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FEVMToolsEnum
{
   /// <summary>
   /// 场景物体层级 sorttingorder
   /// </summary>
   public enum EntityLayer
   {
      UI,
      TopUI,
   }
   
   /// <summary>
   /// 输出颜色
   /// </summary>
   public enum DebugColor
   {
      Normal,
      Red,
      Yellow,
      Green,
   }
   /// <summary>
   /// Canvas
   /// </summary>
   public enum CanvasType
   {
      UI2D,
      DisUI,
      TopUI
   }
}
