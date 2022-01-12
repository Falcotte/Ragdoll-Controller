using System;
using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class RagdollCreator : MonoBehaviour
    {
        private Rig rig;
        private Ragdoll ragdoll;

        private List<RagdollPart> ragdollParts;
        private RagdollPart rootPart;

        public void CreateRagdoll()
        {
            rig = new Rig();

            rig.DetermineRigType(transform);
            rig.GetParts(transform);

            ragdoll = gameObject.AddComponent<Ragdoll>();

            SetupRagdollParts();

            AddColliders();
            AddRigidbodies();
            AddJoints();

            AddRagdollComponents();
        }

        #region Ragdoll Creation

        private void SetupRagdollParts()
        {
            Quaternion originalRotation = transform.rotation;
            transform.rotation = Quaternion.identity;

            ragdollParts = new List<RagdollPart>();

            rootPart = new RagdollPart();
            rootPart.name = "Pelvis";
            rootPart.anchor = rig.Pelvis;
            rootPart.parent = null;
            rootPart.density = 2.5F;
            ragdollParts.Add(rootPart);

            AddMirroredJoint("Hips", rig.LeftHips, rig.RightHips, "Pelvis", Vector3.right, Vector3.forward, -20,
                70, 30,
                typeof(CapsuleCollider), 0.3F, 1.5F);
            AddMirroredJoint("Knee", rig.LeftKnee, rig.RightKnee, "Hips", Vector3.right, Vector3.forward, -80, 0,
                0,
                typeof(CapsuleCollider), 0.25F, 1.5F);

            AddJoint("Middle Spine", rig.MiddleSpine, "Pelvis", Vector3.right, Vector3.forward, -20, 20, 10, null, 1,
                2.5F);

            AddMirroredJoint("Arm", rig.LeftArm, rig.RightArm, "Middle Spine", Vector3.up, Vector3.forward, -70,
                10, 50,
                typeof(CapsuleCollider), 0.25F, 1.0F);
            AddMirroredJoint("Elbow", rig.LeftElbow, rig.RightElbow, "Arm", Vector3.forward, Vector3.up, -90, 0,
                0,
                typeof(CapsuleCollider), 0.20F, 1.0F);

            AddJoint("Head", rig.Head, "Middle Spine", Vector3.right, Vector3.forward, -40, 25, 25, null, 1, 1.0F);

            transform.rotation = originalRotation;

            if(ragdollParts != null)
            {
                foreach(RagdollPart ragdollPart in ragdollParts)
                {
                    if(ragdollPart.anchor)
                    {
                        Component[] joints = ragdollPart.anchor.GetComponentsInChildren(typeof(Joint));
                        foreach(Joint joint in joints)
                        {
                            DestroyImmediate(joint);
                        }

                        Component[] rigidbodies = ragdollPart.anchor.GetComponentsInChildren(typeof(Rigidbody));
                        foreach(Rigidbody rigidbody in rigidbodies)
                        {
                            DestroyImmediate(rigidbody);
                        }

                        Component[] colliders = ragdollPart.anchor.GetComponentsInChildren(typeof(Collider));
                        foreach(Collider collider in colliders)
                        {
                            DestroyImmediate(collider);
                        }
                    }
                }
            }
        }

        private void AddAppendageColliders()
        {
            foreach(RagdollPart ragdollPart in ragdollParts)
            {
                if(ragdollPart.colliderType == typeof(CapsuleCollider))
                {
                    int direction;
                    float distance;
                    if(ragdollPart.children.Count == 1)
                    {
                        RagdollPart childPart = (RagdollPart)ragdollPart.children[0];
                        Vector3 endPoint = childPart.anchor.position;
                        CalculateDirection(ragdollPart.anchor.InverseTransformPoint(endPoint), out direction, out distance);
                    }
                    else
                    {
                        Vector3 endPoint = (ragdollPart.anchor.position - ragdollPart.parent.anchor.position) + ragdollPart.anchor.position;
                        CalculateDirection(ragdollPart.anchor.InverseTransformPoint(endPoint), out direction, out distance);

                        if(ragdollPart.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
                        {
                            Bounds bounds = new Bounds();
                            foreach(Transform child in ragdollPart.anchor.GetComponentsInChildren(typeof(Transform)))
                            {
                                bounds.Encapsulate(ragdollPart.anchor.InverseTransformPoint(child.position));
                            }

                            if(distance > 0)
                                distance = bounds.max[direction];
                            else
                                distance = bounds.min[direction];
                        }
                    }

                    CapsuleCollider collider = ragdollPart.anchor.gameObject.AddComponent<CapsuleCollider>();
                    collider.direction = direction;

                    Vector3 center = Vector3.zero;
                    center[direction] = distance * 0.5F;
                    collider.center = center;
                    collider.height = Mathf.Abs(distance);
                    collider.radius = Mathf.Abs(distance * ragdollPart.radiusScale);
                }
            }
        }

        private void AddBodyColliders()
        {
            if(rig.MiddleSpine != null && rig.Pelvis != null)
            {
                Bounds bounds;
                BoxCollider box;

                bounds = Clip(GetBodyBounds(rig.Pelvis), rig.Pelvis, rig.MiddleSpine, false);
                box = rig.Pelvis.gameObject.AddComponent<BoxCollider>();
                box.center = bounds.center;
                box.size = bounds.size;

                bounds = Clip(GetBodyBounds(rig.MiddleSpine), rig.MiddleSpine, rig.MiddleSpine,
                    true);
                box = rig.MiddleSpine.gameObject.AddComponent<BoxCollider>();
                box.center = bounds.center;
                box.size = bounds.size;
            }
            else
            {
                Bounds bounds = new Bounds();
                bounds.Encapsulate(rig.Pelvis.InverseTransformPoint(rig.LeftHips.position));
                bounds.Encapsulate(rig.Pelvis.InverseTransformPoint(rig.RightHips.position));
                bounds.Encapsulate(rig.Pelvis.InverseTransformPoint(rig.LeftArm.position));
                bounds.Encapsulate(rig.Pelvis.InverseTransformPoint(rig.RightArm.position));

                Vector3 size = bounds.size;
                size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2.0F;

                BoxCollider box = rig.Pelvis.gameObject.AddComponent<BoxCollider>();
                box.center = bounds.center;
                box.size = size;
            }
        }

        private void AddHeadCollider()
        {
            if(rig.Head.GetComponent<Collider>())
            {
                DestroyImmediate(rig.Head.GetComponent<Collider>());
            }

            float radius =
                Vector3.Distance(rig.LeftArm.transform.position, rig.RightArm.transform.position);
            radius /= 4;

            SphereCollider sphere = rig.Head.gameObject.AddComponent<SphereCollider>();
            sphere.radius = radius;
            Vector3 center = Vector3.zero;

            int direction;
            float distance;

            CalculateDirection(rig.Head.InverseTransformPoint(rig.Pelvis.position), out direction,
                out distance);
            if(distance > 0)
            {
                center[direction] = -radius;
            }
            else
            {
                center[direction] = radius;
            }

            sphere.center = center;
        }

        private void AddColliders()
        {
            AddAppendageColliders();
            AddBodyColliders();
            AddHeadCollider();
        }

        private void AddRigidbodies()
        {
            foreach(RagdollPart ragdollPart in ragdollParts)
            {
                ragdollPart.anchor.gameObject.AddComponent<Rigidbody>();
                ragdollPart.anchor.GetComponent<Rigidbody>().mass = ragdollPart.density;
            }
        }

        private void AddJoints()
        {
            foreach(RagdollPart ragdollPart in ragdollParts)
            {
                if(ragdollPart.parent == null)
                    continue;

                CharacterJoint joint = ragdollPart.anchor.gameObject.AddComponent<CharacterJoint>();
                ragdollPart.joint = joint;

                joint.axis = CalculateDirectionAxis(ragdollPart.anchor.InverseTransformDirection(ragdollPart.axis));
                joint.swingAxis = CalculateDirectionAxis(ragdollPart.anchor.InverseTransformDirection(ragdollPart.normalAxis));
                joint.anchor = Vector3.zero;
                joint.connectedBody = ragdollPart.parent.anchor.GetComponent<Rigidbody>();
                joint.enablePreprocessing = false;

                SoftJointLimit limit = new SoftJointLimit();
                limit.contactDistance = 0;

                limit.limit = ragdollPart.minLimit;
                joint.lowTwistLimit = limit;

                limit.limit = ragdollPart.maxLimit;
                joint.highTwistLimit = limit;

                limit.limit = ragdollPart.swingLimit;
                joint.swing1Limit = limit;

                limit.limit = 0;
                joint.swing2Limit = limit;
            }
        }

        private RagdollComponent AddRagdollComponent(Transform transform)
        {
            RagdollComponent ragdollComponent = transform.gameObject.AddComponent<RagdollComponent>();
            ragdoll.RagdollComponents.Add(ragdollComponent);

            ragdollComponent.SetLayerMask(LayerMask.GetMask("Default"));

            return ragdollComponent;
        }

        private void AddRagdollComponents()
        {
            AddRagdollComponent(rig.Pelvis).SetBodyPart(RagdollComponent.BodyParts.CenterMass);
            AddRagdollComponent(rig.LeftHips).SetBodyPart(RagdollComponent.BodyParts.Leg);
            AddRagdollComponent(rig.LeftKnee).SetBodyPart(RagdollComponent.BodyParts.Leg);
            AddRagdollComponent(rig.RightHips).SetBodyPart(RagdollComponent.BodyParts.Leg);
            AddRagdollComponent(rig.RightKnee).SetBodyPart(RagdollComponent.BodyParts.Leg);
            AddRagdollComponent(rig.LeftArm).SetBodyPart(RagdollComponent.BodyParts.Arm);
            AddRagdollComponent(rig.LeftElbow).SetBodyPart(RagdollComponent.BodyParts.Arm);
            AddRagdollComponent(rig.RightArm).SetBodyPart(RagdollComponent.BodyParts.Arm);
            AddRagdollComponent(rig.RightElbow).SetBodyPart(RagdollComponent.BodyParts.Arm);
            AddRagdollComponent(rig.MiddleSpine).SetBodyPart(RagdollComponent.BodyParts.CenterMass);
            AddRagdollComponent(rig.Head).SetBodyPart(RagdollComponent.BodyParts.Head);
        }

        #endregion

        #region Utility

        private class RagdollPart
        {
            public string name;

            public Transform anchor;
            public CharacterJoint joint;
            public RagdollPart parent;

            public float minLimit;
            public float maxLimit;
            public float swingLimit;

            public Vector3 axis;
            public Vector3 normalAxis;

            public float radiusScale;
            public Type colliderType;

            public List<RagdollPart> children = new List<RagdollPart>();
            public float density;
            public float summedMass;
        }

        private RagdollPart FindRagdollPart(string name)
        {
            foreach(RagdollPart ragdollPart in ragdollParts)
            {
                if(ragdollPart.name == name)
                    return ragdollPart;
            }
            return null;
        }

        private void AddJoint(string name, Transform anchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis,
            float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
        {
            RagdollPart ragdollPart = new RagdollPart();
            ragdollPart.name = name;
            ragdollPart.anchor = anchor;
            ragdollPart.axis = worldTwistAxis;
            ragdollPart.normalAxis = worldSwingAxis;
            ragdollPart.minLimit = minLimit;
            ragdollPart.maxLimit = maxLimit;
            ragdollPart.swingLimit = swingLimit;
            ragdollPart.density = density;
            ragdollPart.colliderType = colliderType;
            ragdollPart.radiusScale = radiusScale;

            if(FindRagdollPart(parent) != null)
                ragdollPart.parent = FindRagdollPart(parent);
            else if(name.StartsWith("Left"))
                ragdollPart.parent = FindRagdollPart("Left " + parent);
            else if(name.StartsWith("Right"))
                ragdollPart.parent = FindRagdollPart("Right " + parent);

            ragdollPart.parent.children.Add(ragdollPart);
            ragdollParts.Add(ragdollPart);
        }

        private void AddMirroredJoint(string name, Transform leftAnchor, Transform rightAnchor, string parent,
            Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit,
            Type colliderType, float radiusScale, float density)
        {
            AddJoint("Left " + name, leftAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit,
                colliderType, radiusScale, density);
            AddJoint("Right " + name, rightAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit,
                swingLimit,
                colliderType, radiusScale, density);
        }

        private static void CalculateDirection(Vector3 point, out int direction, out float distance)
        {
            direction = 0;

            if(Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
                direction = 1;
            if(Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
                direction = 2;

            distance = point[direction];
        }

        private static Vector3 CalculateDirectionAxis(Vector3 point)
        {
            int direction = 0;
            float distance;

            CalculateDirection(point, out direction, out distance);
            Vector3 axis = Vector3.zero;

            if(distance > 0)
                axis[direction] = 1.0F;
            else
                axis[direction] = -1.0F;

            return axis;
        }

        private Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
        {
            int axis = LargestComponent(bounds.size);

            if(Vector3.Dot(Vector3.up, relativeTo.TransformPoint(bounds.max)) >
                Vector3.Dot(Vector3.up, relativeTo.TransformPoint(bounds.min)) == below)
            {
                Vector3 min = bounds.min;
                min[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
                bounds.min = min;
            }
            else
            {
                Vector3 max = bounds.max;
                max[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
                bounds.max = max;
            }
            return bounds;
        }

        private Bounds GetBodyBounds(Transform relativeTo)
        {
            Bounds bounds = new Bounds();

            bounds.Encapsulate(relativeTo.InverseTransformPoint(rig.LeftHips.position));
            bounds.Encapsulate(relativeTo.InverseTransformPoint(rig.RightHips.position));
            bounds.Encapsulate(relativeTo.InverseTransformPoint(rig.LeftArm.position));
            bounds.Encapsulate(relativeTo.InverseTransformPoint(rig.RightArm.position));

            Vector3 size = bounds.size;
            size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2.0F;
            bounds.size = size;
            return bounds;
        }

        private static int SmallestComponent(Vector3 point)
        {
            int direction = 0;
            if(Mathf.Abs(point[1]) < Mathf.Abs(point[0]))
                direction = 1;
            if(Mathf.Abs(point[2]) < Mathf.Abs(point[direction]))
                direction = 2;

            return direction;
        }

        private static int LargestComponent(Vector3 point)
        {
            int direction = 0;
            if(Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
                direction = 1;
            if(Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
                direction = 2;

            return direction;
        }

        #endregion
    }
}
