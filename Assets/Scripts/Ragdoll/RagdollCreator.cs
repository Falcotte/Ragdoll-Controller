using System;
using System.Collections;
using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class RagdollCreator : MonoBehaviour
    {
        private float totalMass = 20;

        private Rig rig;
        private Ragdoll ragdoll;

        private ArrayList bones;
        private BoneInfo rootBone;

        Vector3 right = Vector3.right;
        Vector3 up = Vector3.up;
        Vector3 forward = Vector3.forward;

        Vector3 worldRight = Vector3.right;
        Vector3 worldUp = Vector3.up;
        Vector3 worldForward = Vector3.forward;

        public void CreateRagdoll()
        {
            rig = new Rig();

            rig.DetermineRigType(transform);
            rig.GetParts(transform);

            ragdoll = gameObject.AddComponent<Ragdoll>();

            PrepareBones();
            CalculateAxes();

            Cleanup();

            BuildCapsules();
            AddBreastColliders();
            AddHeadCollider();

            BuildBodies();
            BuildJoints();
            CalculateMass();

            AddRagdollComponents();
        }

        private void Cleanup()
        {
            if(bones == null)
                return;

            foreach(BoneInfo bone in bones)
            {
                if(!bone.anchor)
                    continue;

                Component[] joints = bone.anchor.GetComponentsInChildren(typeof(Joint));
                foreach(Joint joint in joints)
                {
                    DestroyImmediate(joint);
                }

                Component[] bodies = bone.anchor.GetComponentsInChildren(typeof(Rigidbody));
                foreach(Rigidbody body in bodies)
                {
                    DestroyImmediate(body);
                }

                Component[] colliders = bone.anchor.GetComponentsInChildren(typeof(Collider));
                foreach(Collider collider in colliders)
                {
                    DestroyImmediate(collider);
                }
            }
        }

        #region Ragdoll Creation

        void PrepareBones()
        {
            if(rig.Pelvis)
            {
                worldRight = rig.Pelvis.TransformDirection(right);
                worldUp = rig.Pelvis.TransformDirection(up);
                worldForward = rig.Pelvis.TransformDirection(forward);
            }

            bones = new ArrayList();

            rootBone = new BoneInfo();
            rootBone.name = "Pelvis";
            rootBone.anchor = rig.Pelvis;
            rootBone.parent = null;
            rootBone.density = 2.5F;
            bones.Add(rootBone);

            AddMirroredJoint("Hips", rig.LeftHips, rig.RightHips, "Pelvis", worldRight, worldForward, -20,
                70, 30,
                typeof(CapsuleCollider), 0.3F, 1.5F);
            AddMirroredJoint("Knee", rig.LeftKnee, rig.RightKnee, "Hips", worldRight, worldForward, -80, 0,
                0,
                typeof(CapsuleCollider), 0.25F, 1.5F);

            AddJoint("Middle Spine", rig.MiddleSpine, "Pelvis", worldRight, worldForward, -20, 20, 10, null, 1,
                2.5F);

            AddMirroredJoint("Arm", rig.LeftArm, rig.RightArm, "Middle Spine", worldUp, worldForward, -70,
                10, 50,
                typeof(CapsuleCollider), 0.25F, 1.0F);
            AddMirroredJoint("Elbow", rig.LeftElbow, rig.RightElbow, "Arm", worldForward, worldUp, -90, 0,
                0,
                typeof(CapsuleCollider), 0.20F, 1.0F);

            AddJoint("Head", rig.Head, "Middle Spine", worldRight, worldForward, -40, 25, 25, null, 1, 1.0F);
        }

        void CalculateAxes()
        {
            if(rig.Head != null && rig.Pelvis != null)
                up = CalculateDirectionAxis(rig.Pelvis.InverseTransformPoint(rig.Head.position));
            if(rig.RightElbow != null && rig.Pelvis != null)
            {
                Vector3 removed, temp;
                DecomposeVector(out temp, out removed,
                    rig.Pelvis.InverseTransformPoint(rig.RightElbow.position), up);
                right = CalculateDirectionAxis(removed);
            }

            forward = Vector3.Cross(right, up);
        }

        void BuildCapsules()
        {
            foreach(BoneInfo bone in bones)
            {
                if(bone.colliderType != typeof(CapsuleCollider))
                    continue;

                int direction;
                float distance;
                if(bone.children.Count == 1)
                {
                    BoneInfo childBone = (BoneInfo)bone.children[0];
                    Vector3 endPoint = childBone.anchor.position;
                    CalculateDirection(bone.anchor.InverseTransformPoint(endPoint), out direction, out distance);
                }
                else
                {
                    Vector3 endPoint = (bone.anchor.position - bone.parent.anchor.position) + bone.anchor.position;
                    CalculateDirection(bone.anchor.InverseTransformPoint(endPoint), out direction, out distance);

                    if(bone.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
                    {
                        Bounds bounds = new Bounds();
                        foreach(Transform child in bone.anchor.GetComponentsInChildren(typeof(Transform)))
                        {
                            bounds.Encapsulate(bone.anchor.InverseTransformPoint(child.position));
                        }

                        if(distance > 0)
                            distance = bounds.max[direction];
                        else
                            distance = bounds.min[direction];
                    }
                }

                CapsuleCollider collider = bone.anchor.gameObject.AddComponent<CapsuleCollider>();
                collider.direction = direction;

                Vector3 center = Vector3.zero;
                center[direction] = distance * 0.5F;
                collider.center = center;
                collider.height = Mathf.Abs(distance);
                collider.radius = Mathf.Abs(distance * bone.radiusScale);
            }
        }

        void AddBreastColliders()
        {
            if(rig.MiddleSpine != null && rig.Pelvis != null)
            {
                Bounds bounds;
                BoxCollider box;

                bounds = Clip(GetBreastBounds(rig.Pelvis), rig.Pelvis, rig.MiddleSpine, false);
                box = rig.Pelvis.gameObject.AddComponent<BoxCollider>();
                box.center = bounds.center;
                box.size = bounds.size;

                bounds = Clip(GetBreastBounds(rig.MiddleSpine), rig.MiddleSpine, rig.MiddleSpine,
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

        void AddHeadCollider()
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

        void BuildBodies()
        {
            foreach(BoneInfo bone in bones)
            {
                bone.anchor.gameObject.AddComponent<Rigidbody>();
                bone.anchor.GetComponent<Rigidbody>().mass = bone.density;
            }
        }

        void BuildJoints()
        {
            foreach(BoneInfo bone in bones)
            {
                if(bone.parent == null)
                    continue;

                CharacterJoint joint = bone.anchor.gameObject.AddComponent<CharacterJoint>();
                bone.joint = joint;

                joint.axis = CalculateDirectionAxis(bone.anchor.InverseTransformDirection(bone.axis));
                joint.swingAxis = CalculateDirectionAxis(bone.anchor.InverseTransformDirection(bone.normalAxis));
                joint.anchor = Vector3.zero;
                joint.connectedBody = bone.parent.anchor.GetComponent<Rigidbody>();
                joint.enablePreprocessing = false;

                SoftJointLimit limit = new SoftJointLimit();
                limit.contactDistance = 0;

                limit.limit = bone.minLimit;
                joint.lowTwistLimit = limit;

                limit.limit = bone.maxLimit;
                joint.highTwistLimit = limit;

                limit.limit = bone.swingLimit;
                joint.swing1Limit = limit;

                limit.limit = 0;
                joint.swing2Limit = limit;
            }
        }

        private void CalculateMass()
        {
            CalculateMassRecurse(rootBone);

            float massScale = totalMass / rootBone.summedMass;

            foreach(BoneInfo bone in bones)
            {
                bone.anchor.GetComponent<Rigidbody>().mass *= massScale;
            }

            CalculateMassRecurse(rootBone);
        }

        private void AddRagdollComponent(Transform transform)
        {
            RagdollComponent ragdollComponent = transform.gameObject.AddComponent<RagdollComponent>();
            ragdoll.RagdollComponents.Add(ragdollComponent);
        }

        private void AddRagdollComponents()
        {
            AddRagdollComponent(rig.Pelvis);
            AddRagdollComponent(rig.LeftHips);
            AddRagdollComponent(rig.LeftKnee);
            AddRagdollComponent(rig.RightHips);
            AddRagdollComponent(rig.RightKnee);
            AddRagdollComponent(rig.LeftArm);
            AddRagdollComponent(rig.LeftElbow);
            AddRagdollComponent(rig.RightArm);
            AddRagdollComponent(rig.RightElbow);
            AddRagdollComponent(rig.MiddleSpine);
            AddRagdollComponent(rig.Head);
        }

        #endregion

        #region Utility

        private class BoneInfo
        {
            public string name;

            public Transform anchor;
            public CharacterJoint joint;
            public BoneInfo parent;

            public float minLimit;
            public float maxLimit;
            public float swingLimit;

            public Vector3 axis;
            public Vector3 normalAxis;

            public float radiusScale;
            public Type colliderType;

            public ArrayList children = new ArrayList();
            public float density;
            public float summedMass; // The mass of this and all children bodies
        }

        private BoneInfo FindBone(string name)
        {
            foreach(BoneInfo bone in bones)
            {
                if(bone.name == name)
                    return bone;
            }
            return null;
        }

        private void AddJoint(string name, Transform anchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis,
            float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
        {
            BoneInfo bone = new BoneInfo();
            bone.name = name;
            bone.anchor = anchor;
            bone.axis = worldTwistAxis;
            bone.normalAxis = worldSwingAxis;
            bone.minLimit = minLimit;
            bone.maxLimit = maxLimit;
            bone.swingLimit = swingLimit;
            bone.density = density;
            bone.colliderType = colliderType;
            bone.radiusScale = radiusScale;

            if(FindBone(parent) != null)
                bone.parent = FindBone(parent);
            else if(name.StartsWith("Left"))
                bone.parent = FindBone("Left " + parent);
            else if(name.StartsWith("Right"))
                bone.parent = FindBone("Right " + parent);

            bone.parent.children.Add(bone);
            bones.Add(bone);
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

        private void DecomposeVector(out Vector3 normalCompo, out Vector3 tangentCompo, Vector3 outwardDir,
            Vector3 outwardNormal)
        {
            outwardNormal = outwardNormal.normalized;
            normalCompo = outwardNormal * Vector3.Dot(outwardDir, outwardNormal);
            tangentCompo = outwardDir - normalCompo;
        }

        private void CalculateMassRecurse(BoneInfo bone)
        {
            float mass = bone.anchor.GetComponent<Rigidbody>().mass;

            foreach(BoneInfo child in bone.children)
            {
                CalculateMassRecurse(child);
                mass += child.summedMass;
            }

            bone.summedMass = mass;
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

        private Bounds GetBreastBounds(Transform relativeTo)
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