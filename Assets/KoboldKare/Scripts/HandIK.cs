using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIK : MonoBehaviour {
    private Animator animator;
    private Transform head;
    private List<Hand> hands = new List<Hand>();
    private Transform[] controllers = new Transform[2];
    public bool controlledByPlayer;
    [SerializeField] Vector3[] bends = new Vector3[3] { new Vector3(0, 0, 0), new Vector3(0, 0, 45), new Vector3(0, 0, -45), };
    private class Hand {
        public float positionWeight = 0f;
        public float rotationWeight = 0f;
        public Coroutine routine;
        public Vector3 position;
        public Quaternion goalRotation;
        public bool transitioning = false;
    }
    public IEnumerator WeightUp(int hand) {
        while (hands[hand].positionWeight != 1f) {
            hands[hand].positionWeight = Mathf.MoveTowards(hands[hand].positionWeight, 1f, Time.deltaTime);
            hands[hand].rotationWeight = Mathf.MoveTowards(hands[hand].rotationWeight, 1f, Time.deltaTime);
            yield return new WaitForEndOfFrame(); 
        }
        hands[hand].transitioning = false;
    }
    public IEnumerator WeightDown(int hand) {
        while (hands[hand].positionWeight != 0f) {
            hands[hand].positionWeight = Mathf.MoveTowards(hands[hand].positionWeight, 0f, Time.deltaTime);
            hands[hand].rotationWeight = Mathf.MoveTowards(hands[hand].rotationWeight, 0f, Time.deltaTime);
            yield return new WaitForEndOfFrame(); 
        }
        hands[hand].transitioning = false;
    }
    private void Awake() {
        animator = GetComponent<Animator>();
        hands.Add(new Hand());
        hands.Add(new Hand());

        head = animator.GetBoneTransform(HumanBodyBones.Head);

        if (FoxVRLoader.GetSeatedMode()) return;

        controllers[0] = FoxVRLoader.GetTrackedDeviceTRANSFORM(FoxVRLoader.XRDevice.RightHand);//GameObject.Find("VRHandsAttachPoseLEFT").transform;
        controllers[1] = FoxVRLoader.GetTrackedDeviceTRANSFORM(FoxVRLoader.XRDevice.LeftHand);
    }
    public void SetIKTarget(int hand, Vector3 position, Quaternion rotation) {
        hands[hand].position = position;
        hands[hand].goalRotation = rotation;
        if (!hands[hand].transitioning && hands[hand].positionWeight != 1f) {
            hands[hand].routine = StartCoroutine(WeightUp(hand));
            hands[hand].transitioning = true;
        }
    }
    public void UnsetIKTarget(int hand) {
        if (hands[hand].transitioning && hands[hand].routine != null) {
            StopCoroutine(hands[hand].routine);
        }
        StartCoroutine(WeightDown(hand));
    }
    private void OnAnimatorIK(int layerIndex)
    {

        if (!isActiveAndEnabled) 
            return; 

        head.localScale = Vector3.one;

        if (controlledByPlayer)                 //Moove all the stuff into PlayerPossetion script for synching.
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, controllers[1].position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, controllers[1].rotation); 

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, controllers[0].position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, controllers[0].rotation);  

            animator.bodyPosition = (FoxVRLoader.GetActiveCameraTransform().position - new Vector3(0, .6688192f,0) * FoxVRLoader.koboldScale) - (FoxVRLoader.GetActiveCameraTransform().forward * .2f) - (FoxVRLoader.GetActiveCameraTransform().up * .1f);

            //animator.bodyPosition = (FoxVRLoader.activeCameraOBJ.transform.position - (new Vector3(0, .6688192f,0) - (FoxVRLoader.activeCameraOBJ.transform.forward * .2f) - (FoxVRLoader.activeCameraOBJ.transform.up * .1f) * FoxVRLoader.koboldScale));

            //transform.GetComponentInParent<CapsuleCollider>().center = new Vector3(animator.bodyPosition.x, 0, animator.bodyPosition.z);

            animator.GetBoneTransform(HumanBodyBones.RightHand).localScale = Vector3.one * FoxVRLoader.GetHandsSize();
            animator.GetBoneTransform(HumanBodyBones.LeftHand).localScale = Vector3.one * FoxVRLoader.GetHandsSize();

            head.localScale = Vector3.one * .001f;








            animator.SetBoneLocalRotation(HumanBodyBones.RightIndexProximal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.triggerSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightIndexIntermediate, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.triggerSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightIndexDistal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.triggerSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleProximal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.gripSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightRingProximal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.gripSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightLittleIntermediate, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.gripSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleIntermediate, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.gripSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightRingIntermediate, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.gripSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightLittleDistal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[1], FoxVRLoader.gripSqueezeR)));
            animator.SetBoneLocalRotation(HumanBodyBones.RightThumbDistal, Quaternion.Euler(FoxVRLoader.thumbMoveR));


            animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexProximal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.triggerSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexIntermediate, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.triggerSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexDistal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.triggerSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleProximal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.gripSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftRingProximal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.gripSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleIntermediate, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.gripSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleIntermediate, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.gripSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftRingIntermediate, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.gripSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleDistal, Quaternion.Euler(Vector3.Lerp(bends[0], bends[2], FoxVRLoader.gripSqueezeL)));
            animator.SetBoneLocalRotation(HumanBodyBones.LeftThumbDistal, Quaternion.Euler(FoxVRLoader.thumbMoveR));

        }
        else
        {  
            animator.SetIKPosition(AvatarIKGoal.LeftHand, hands[0].position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, hands[0].goalRotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, hands[0].positionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, hands[0].rotationWeight);

            animator.SetIKPosition(AvatarIKGoal.RightHand, hands[1].position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, hands[1].goalRotation);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, hands[1].positionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, hands[1].rotationWeight);

        } 
    }
}
