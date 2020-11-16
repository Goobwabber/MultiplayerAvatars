using CustomAvatar.Tracking;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiplayerAvatars.Avatars
{
    internal class MultiplayerAvatarInput : IAvatarInput
    {
        private readonly AvatarPoseController _poseController;

        private Transform headTransform;
        private Transform rightHandTransform;
        private Transform leftHandTransform;
        private Transform bodyTransform;

        private Pose head = new Pose();
        private Pose rightHand = new Pose();
        private Pose leftHand = new Pose();

        internal MultiplayerAvatarInput(AvatarPoseController poseController, bool handsEnabled = false)
        {
            _poseController = poseController;

            _poseController.didUpdatePoseEvent += OnInputChanged;
            headTransform = _poseController.GetField<Transform, AvatarPoseController>("_headTransform");
            rightHandTransform = _poseController.GetField<Transform, AvatarPoseController>("_rightHandTransform");
            leftHandTransform = _poseController.GetField<Transform, AvatarPoseController>("_leftHandTransform");
            bodyTransform = _poseController.GetField<Transform, AvatarPoseController>("_bodyTransform");

            headTransform.gameObject.SetActive(false);
            bodyTransform.gameObject.SetActive(false);
            rightHandTransform.gameObject.SetActive(handsEnabled);
            leftHandTransform.gameObject.SetActive(handsEnabled);
            rightHandTransform.Find("hand")?.gameObject.SetActive(handsEnabled);
            leftHandTransform.Find("hand")?.gameObject.SetActive(handsEnabled);
        }

        private void OnInputChanged(Vector3 newHeadPosition)
        {
            head.position = newHeadPosition;
            head.rotation = headTransform.localRotation;
            rightHand.position = rightHandTransform.localPosition;
            rightHand.rotation = rightHandTransform.localRotation;
            leftHand.position = leftHandTransform.localPosition;
            leftHand.rotation = leftHandTransform.localRotation;
        }

        public bool allowMaintainPelvisPosition => true;

        public event Action inputChanged;

        public bool TryGetFingerCurl(DeviceUse use, out FingerCurl curl)
        {
            curl = new FingerCurl(0f, 0f, 0f, 0f, 0f);
            return false;
        }

        public bool TryGetPose(DeviceUse use, out Pose pose)
        {
            switch (use)
            {
                case DeviceUse.Head:
                    pose = head;
                    return true;
                case DeviceUse.RightHand:
                    pose = rightHand;
                    return true;
                case DeviceUse.LeftHand:
                    pose = leftHand;
                    return true;
                default:
                    pose = Pose.identity;
                    return false;
            }
        }
    }
}
