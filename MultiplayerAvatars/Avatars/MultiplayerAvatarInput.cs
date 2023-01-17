using CustomAvatar.Tracking;
using IPA.Utilities;
using System;
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

        internal MultiplayerAvatarInput(AvatarPoseController poseController)
        {
            _poseController = poseController;

            _poseController.didUpdatePoseEvent += OnInputChanged;
            headTransform = _poseController.GetField<Transform, AvatarPoseController>("_headTransform");
            rightHandTransform = _poseController.GetField<Transform, AvatarPoseController>("_rightHandTransform");
            leftHandTransform = _poseController.GetField<Transform, AvatarPoseController>("_leftHandTransform");
            bodyTransform = _poseController.GetField<Transform, AvatarPoseController>("_bodyTransform");

            SetEnabled(true);
        }

        public void SetEnabled(bool enabled)
        {
            headTransform.gameObject.SetActive(!enabled);
            bodyTransform.gameObject.SetActive(!enabled);
            rightHandTransform.Find("Hand").gameObject.SetActive(!enabled);
            leftHandTransform.Find("Hand").gameObject.SetActive(!enabled);
        }

        private void OnInputChanged(Vector3 newHeadPosition)
        {
            head.position = newHeadPosition;
            head.rotation = headTransform.localRotation;
            rightHand.position = rightHandTransform.localPosition;
            rightHand.rotation = rightHandTransform.localRotation;
            leftHand.position = leftHandTransform.localPosition;
            leftHand.rotation = leftHandTransform.localRotation;

            if (rightHand.position == head.position)
                rightHand.position += Vector3.one * 0.1f;
            if (rightHand.rotation == head.rotation)
                rightHand.rotation *= Quaternion.identity;
            if (leftHand.position == head.position)
                leftHand.position += Vector3.one * -0.1f;
            if (leftHand.rotation == head.rotation)
                leftHand.rotation *= Quaternion.identity;
        }

        public bool allowMaintainPelvisPosition => true;

        public event Action? inputChanged;

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
