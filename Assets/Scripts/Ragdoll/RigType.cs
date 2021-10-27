using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Ragdoll
{
    [CreateAssetMenu(fileName = "RigType", menuName = "Angry Koala/Ragdoll/New Rig Type", order = 2)]
    public class RigType : ScriptableObject
    {
        [SerializeField] private string pelvis;
        public string Pelvis => pelvis;

        [SerializeField] private string leftHips;
        public string LeftHips => leftHips;
        [SerializeField] private string leftKnee;
        public string LeftKnee => leftKnee;

        [SerializeField] private string rightHips;
        public string RightHips => rightHips;
        [SerializeField] private string rightKnee;
        public string RightKnee => rightKnee;

        [SerializeField] private string leftArm;
        public string LeftArm => leftArm;
        [SerializeField] private string leftElbow;
        public string LeftElbow => leftElbow;

        [SerializeField] private string rightArm;
        public string RightArm => rightArm;
        [SerializeField] private string rightElbow;
        public string RightElbow => rightElbow;

        [SerializeField] private string middleSpine;
        public string MiddleSpine => middleSpine;
        [SerializeField] private string head;
        public string Head => head;

        public List<string> PartNames
        {
            get
            {
                return new List<string> { pelvis, leftHips, leftKnee, rightHips, rightKnee, leftArm, leftElbow, rightArm, rightElbow, middleSpine, head };
            }
        }
    }
}
