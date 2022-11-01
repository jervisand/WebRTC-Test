using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace geniikw.DataSheetLab.Example
{
    [Serializable]
    public class CharacterData
    {
        public string name;
        ///// <summary>
        ///// apply color when big or small.
        ///// it only apply float and int.
        ///// </summary>
        [BigCheck(30)]
        public int attack;
        [BigCheck(30)]
        public float factor;

        /// <summary>
        /// known issue : if there are alot of item(more than view count), can't draw in scrollview.
        /// </summary>
        public string describe;

        /// <summary>
        /// You can also declare it as List <SkillSheet>.
        /// However, the ReferSheet class has its own List<int>,
        /// which makes it easy to refer to each item that Sheet has.
        /// </summary>
        public SkillRefer skillSet;


        [BigCheck(30)]
        [SmallCheck(10)]
        /// <summary>
        /// you can see and sort property in sheet
        /// </summary>
        public float AttackResult
        {
            get
            {
                return attack * factor;
            }
        }
        //dont declare a constructor with parameter.
        //if you have to, must declare a contructor with no parameter too.
    }    

    /// <summary>
    /// sheet class name must be same with file name.
    /// </summary>
    [CreateAssetMenu]
    public class CharacterSheet : Sheet<CharacterData> { }
}