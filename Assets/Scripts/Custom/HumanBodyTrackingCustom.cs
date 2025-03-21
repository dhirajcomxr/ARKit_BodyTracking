using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation.Samples;


public class HumanBodyTrackingCustom : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce frame events.")]
    private ARHumanBodyManager humanBodyManager;

    [SerializeField] private GameObject jointPrefab;

    [SerializeField] private GameObject lineRendererPrefab;

    public GameObject bodyRig;

    public Transform[] bones;
    public Transform[] newBones;
    private Dictionary<JointIndices3D, Transform> bodyJoints;

    private LineRenderer[] lineRenderers;
    private Transform[][] lineRendererTransforms;

    private const float jointScaleModifier = .4f;

    void OnEnable()
    {
        Debug.Assert(humanBodyManager != null, "Human body manager is required");
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        if (humanBodyManager != null)
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    private void InitialiseObjects(Transform arBodyT)
    {
        
        if (bodyJoints == null)
        {
            bodyJoints = new Dictionary<JointIndices3D, Transform>
            {
                {JointIndices3D.Root,GetNewPos(arBodyT,0)},
                {JointIndices3D.Hips,GetNewPos(arBodyT,1)},
                {JointIndices3D.LeftUpLeg,GetNewPos(arBodyT,2)},
                {JointIndices3D.LeftLeg,GetNewPos(arBodyT,3)},
                {JointIndices3D.LeftFoot,GetNewPos(arBodyT,4)},
                {JointIndices3D.LeftToes,GetNewPos(arBodyT,5)},
                {JointIndices3D.LeftToesEnd,GetNewPos(arBodyT,6)},
                {JointIndices3D.RightUpLeg,GetNewPos(arBodyT,7)},
                {JointIndices3D.RightLeg,GetNewPos(arBodyT,8)},
                {JointIndices3D.RightFoot,GetNewPos(arBodyT,9)},
                {JointIndices3D.RightToes,GetNewPos(arBodyT,10)},
                {JointIndices3D.RightToesEnd,GetNewPos(arBodyT,11)},
                {JointIndices3D.Spine1,GetNewPos(arBodyT,12)},
                {JointIndices3D.Spine2,GetNewPos(arBodyT,13)},
                {JointIndices3D.Spine3,GetNewPos(arBodyT,14)},
                {JointIndices3D.Spine4,GetNewPos(arBodyT,15)},
                {JointIndices3D.Spine5,GetNewPos(arBodyT,16)},
                {JointIndices3D.Spine6,GetNewPos(arBodyT,17)},
                {JointIndices3D.Spine7,GetNewPos(arBodyT,18)},
                {JointIndices3D.LeftShoulder1,GetNewPos(arBodyT,19)},
                {JointIndices3D.LeftArm,GetNewPos(arBodyT,20)},
                {JointIndices3D.LeftForearm,GetNewPos(arBodyT,21)},
                {JointIndices3D.LeftHand,GetNewPos(arBodyT,22)},
                {JointIndices3D.LeftHandIndexStart,GetNewPos(arBodyT,23)},
                {JointIndices3D.LeftHandIndex1,GetNewPos(arBodyT,24)},
                {JointIndices3D.LeftHandIndex2,GetNewPos(arBodyT,25)},
                {JointIndices3D.LeftHandIndex3,GetNewPos(arBodyT,26)},
                {JointIndices3D.LeftHandIndexEnd,GetNewPos(arBodyT,27)},
                {JointIndices3D.LeftHandMidStart,GetNewPos(arBodyT,28)},
                {JointIndices3D.LeftHandMid1,GetNewPos(arBodyT,29)},
                {JointIndices3D.LeftHandMid2,GetNewPos(arBodyT,30)},
                {JointIndices3D.LeftHandMid3,GetNewPos(arBodyT,31)},
                {JointIndices3D.LeftHandMidEnd,GetNewPos(arBodyT,32)},
                {JointIndices3D.LeftHandPinkyStart,GetNewPos(arBodyT,33)},
                {JointIndices3D.LeftHandPinky1,GetNewPos(arBodyT,34)},
                {JointIndices3D.LeftHandPinky2,GetNewPos(arBodyT,35)},
                {JointIndices3D.LeftHandPinky3,GetNewPos(arBodyT,36)},
                {JointIndices3D.LeftHandPinkyEnd,GetNewPos(arBodyT,37)},
                {JointIndices3D.LeftHandRingStart,GetNewPos(arBodyT,38)},
                {JointIndices3D.LeftHandRing1,GetNewPos(arBodyT,39)},
                {JointIndices3D.LeftHandRing2,GetNewPos(arBodyT,40)},
                {JointIndices3D.LeftHandRing3,GetNewPos(arBodyT,41)},
                {JointIndices3D.LeftHandRingEnd,GetNewPos(arBodyT,42)},
                {JointIndices3D.LeftHandThumbStart,GetNewPos(arBodyT,43)},
                {JointIndices3D.LeftHandThumb1,GetNewPos(arBodyT,44)},
                {JointIndices3D.LeftHandThumb2,GetNewPos(arBodyT,45)},
                {JointIndices3D.LeftHandThumbEnd,GetNewPos(arBodyT,46)},
                {JointIndices3D.Neck1,GetNewPos(arBodyT,47)},
                {JointIndices3D.Neck2,GetNewPos(arBodyT,48)},
                {JointIndices3D.Neck3,GetNewPos(arBodyT,49)},
                {JointIndices3D.Neck4,GetNewPos(arBodyT,50)},
                {JointIndices3D.Head,GetNewPos(arBodyT,51)},
                {JointIndices3D.Jaw,GetNewPos(arBodyT,52)},
                {JointIndices3D.Chin,GetNewPos(arBodyT,53)},
                {JointIndices3D.LeftEye,GetNewPos(arBodyT,54)},
                {JointIndices3D.LeftEyeLowerLid,GetNewPos(arBodyT,55)},
                {JointIndices3D.LeftEyeUpperLid,GetNewPos(arBodyT,56)},
                {JointIndices3D.LeftEyeball,GetNewPos(arBodyT,57)},
                {JointIndices3D.Nose,GetNewPos(arBodyT,58)},
                {JointIndices3D.RightEye,GetNewPos(arBodyT,59)},
                {JointIndices3D.RightEyeLowerLid,GetNewPos(arBodyT,60)},
                {JointIndices3D.RightEyeUpperLid,GetNewPos(arBodyT,61)},
                {JointIndices3D.RightEyeball,GetNewPos(arBodyT,62)},
                {JointIndices3D.RightShoulder1,GetNewPos(arBodyT,63)},
                {JointIndices3D.RightArm,GetNewPos(arBodyT,64)},
                {JointIndices3D.RightForearm,GetNewPos(arBodyT,65)},
                {JointIndices3D.RightHand,GetNewPos(arBodyT,66)},
                {JointIndices3D.RightHandIndexStart,GetNewPos(arBodyT,67)},
                {JointIndices3D.RightHandIndex1,GetNewPos(arBodyT,68)},
                {JointIndices3D.RightHandIndex2,GetNewPos(arBodyT,69)},
                {JointIndices3D.RightHandIndex3,GetNewPos(arBodyT,70)},
                {JointIndices3D.RightHandIndexEnd,GetNewPos(arBodyT,71)},
                {JointIndices3D.RightHandMidStart,GetNewPos(arBodyT,72)},
                {JointIndices3D.RightHandMid1,GetNewPos(arBodyT,73)},
                {JointIndices3D.RightHandMid2,GetNewPos(arBodyT,74)},
                {JointIndices3D.RightHandMid3,GetNewPos(arBodyT,75)},
                {JointIndices3D.RightHandMidEnd,GetNewPos(arBodyT,76)},
                {JointIndices3D.RightHandPinkyStart,GetNewPos(arBodyT,77)},
                {JointIndices3D.RightHandPinky1,GetNewPos(arBodyT,78)},
                {JointIndices3D.RightHandPinky2,GetNewPos(arBodyT,79)},
                {JointIndices3D.RightHandPinky3,GetNewPos(arBodyT,80)},
                {JointIndices3D.RightHandPinkyEnd,GetNewPos(arBodyT,81)},
                {JointIndices3D.RightHandRingStart,GetNewPos(arBodyT,82)},
                {JointIndices3D.RightHandRing1,GetNewPos(arBodyT,83)},
                {JointIndices3D.RightHandRing2,GetNewPos(arBodyT,84)},
                {JointIndices3D.RightHandRing3,GetNewPos(arBodyT,85)},
                {JointIndices3D.RightHandRingEnd,GetNewPos(arBodyT,86)},
                {JointIndices3D.RightHandThumbStart,GetNewPos(arBodyT,87)},
                {JointIndices3D.RightHandThumb1,GetNewPos(arBodyT,88)},
                {JointIndices3D.RightHandThumb2,GetNewPos(arBodyT,89)},
                {JointIndices3D.RightHandThumbEnd,GetNewPos(arBodyT,90)}


                // { JointIndices3D.Head, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.Neck1, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.LeftArm, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.RightArm, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.LeftForearm, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.RightForearm, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.LeftHand, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.RightHand, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.LeftUpLeg, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.RightUpLeg, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.LeftLeg, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.RightLeg, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.LeftFoot, GetNewJointPrefab(arBodyT) },
                // { JointIndices3D.RightFoot, GetNewJointPrefab(arBodyT) }
            };

            // Create line renderers
            // lineRenderers = new LineRenderer[]
            // {
            //     Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // head neck
            //     Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // upper
            //     Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // lower
            //     Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // right
            //     Instantiate(lineRendererPrefab).GetComponent<LineRenderer>() // left
            // };

            // lineRendererTransforms = new Transform[][]
            // {
            //     new Transform[] { bodyJoints[JointIndices3D.Head], bodyJoints[JointIndices3D.Neck1] },
            //     new Transform[] { bodyJoints[JointIndices3D.RightHand], bodyJoints[JointIndices3D.RightForearm], bodyJoints[JointIndices3D.RightArm], bodyJoints[JointIndices3D.Neck1], bodyJoints[JointIndices3D.LeftArm], bodyJoints[JointIndices3D.LeftForearm], bodyJoints[JointIndices3D.LeftHand]},
            //     new Transform[] { bodyJoints[JointIndices3D.RightFoot], bodyJoints[JointIndices3D.RightLeg], bodyJoints[JointIndices3D.RightUpLeg], bodyJoints[JointIndices3D.LeftUpLeg], bodyJoints[JointIndices3D.LeftLeg], bodyJoints[JointIndices3D.LeftFoot] },
            //     new Transform[] { bodyJoints[JointIndices3D.RightArm], bodyJoints[JointIndices3D.RightUpLeg] },
            //     new Transform[] { bodyJoints[JointIndices3D.LeftArm], bodyJoints[JointIndices3D.LeftUpLeg] }
            // };

            // for (int i = 0; i < lineRenderers.Length; i++)
            // {
            //     lineRenderers[i].positionCount = lineRendererTransforms[i].Length;
            // }
        }
    }

    private Transform GetNewJointPrefab(Transform arBodyT)
    {
        return Instantiate(jointPrefab, arBodyT).transform;
    }
    private Transform GetNewPos(Transform arBodyT, int i){
        Transform T = bones[i];
        //T.SetParent(arBodyT);
        return T;
    }
    void Update(){
        //UpdateNewRig();
    }
    private void UpdateNewRig(){
        for (int i = 0; i < newBones.Length-1; i++)
        {
            newBones[i].position = bones[i].position;
            newBones[i].rotation = bones[i].rotation;
            newBones[i].localScale = bones[i].localScale;          
        }
    }
    void UpdateBody(ARHumanBody arBody)
    {
        Transform arBodyT = arBody.transform;
        // bodyRig.SetActive(true);
        if (arBodyT == null)
        {
            Debug.Log("No root transform found for ARHumanBody");
            return;
        }

        InitialiseObjects(arBodyT);

        /// Update joint placement
        NativeArray<XRHumanBodyJoint> joints = arBody.joints;
        if(!joints.IsCreated) return;

        /// Update placement of all joints
        foreach (KeyValuePair<JointIndices3D, Transform> item in bodyJoints)
        {
            UpdateJointTransform(item.Value, joints[(int)item.Key]);
        }
        // for (int i = 0; i < bones.Length - 1; i++)
        // {
        //     UpdateNewPositions(bones[i],joints[i]);
        // }

        /// Update all line renderers.
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            //lineRenderers[i].SetPositions(lineRendererTransforms[i].position);            
        }
    }

    private void UpdateJointTransform(Transform jointT, XRHumanBodyJoint bodyJoint)
    {
        jointT.localPosition = bodyJoint.anchorPose.position;
        jointT.localRotation = bodyJoint.anchorPose.rotation;
        jointT.localScale = bodyJoint.anchorScale * jointScaleModifier;
    }

    private void UpdateNewPositions(Transform jointT,XRHumanBodyJoint bodyJoint){
        jointT.localPosition = bodyJoint.anchorPose.position;
        jointT.localRotation = bodyJoint.anchorPose.rotation;
        jointT.localScale = bodyJoint.anchorScale * jointScaleModifier;
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        foreach (ARHumanBody humanBody in eventArgs.added)
        {
            UpdateBody(humanBody);
        }

        foreach (ARHumanBody humanBody in eventArgs.updated)
        {
            UpdateBody(humanBody);
        }

        //Debug.Log($"Created {eventArgs.added.Count}, updated {eventArgs.updated.Count}, removed {eventArgs.removed.Count}");
    }
}