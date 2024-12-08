using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public static class VisualElementReadOnly
{
    public static readonly Type[] ValidTypes =
    {
        #region Numerics
        typeof(IntegerField),
        typeof(UnsignedIntegerField),
        typeof(LongField),
        typeof(FloatField),
        typeof(DoubleField),
        #endregion

        typeof(TextField),
        typeof(Hash128Field),
        
    };
}
