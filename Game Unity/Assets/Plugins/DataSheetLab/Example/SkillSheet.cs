using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace geniikw.DataSheetLab.Example
{

    public enum EABC
    {
        A=1,
        B=2,
        C=4
    }

    [Serializable]
    public class SkillData
    {
        public string Name;
        public float Factor;
        public AnimationCurve curve;
        public Color color;
        public EABC abc;
    }

    /// <summary>
    /// sheet class name must be same with file name.
    /// </summary>
    [CreateAssetMenu]
    public class SkillSheet : Sheet<SkillData> { }

    /// <summary>
    /// you have to declare class for serialize.
    /// if use like ReferSheet<SkillSheet, SkillData> skillSet, it will not be save.
    /// [Serializable] is also required.
    /// </summary>
    [Serializable]
    public class SkillRefer : ReferSheet<SkillSheet, SkillData> { }
}