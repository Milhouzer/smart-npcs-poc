using System;
using UnityEngine;

namespace Milhouzer.Core.BuildSystem
{
    public enum RotateAxis {
        xAxis,
        yAxis,
        zAxis 
    }

    [Flags]
    public enum ScaleAxis {
        xAxis = 1 << 0,
        yAxis = 1 << 1,
        zAxis = 1 << 2
    }

    [CreateAssetMenu(fileName = "PreviewConstraints", menuName = "Builder/PreviewConstraints", order = 0)]
    public class PreviewConstraints : ScriptableObject {
        public RotateAxis RotateAxis = RotateAxis.yAxis;
        public ScaleAxis ScaleAxis = ScaleAxis.xAxis | ScaleAxis.yAxis |  ScaleAxis.zAxis;
        public Vector3 MinScale;
        public Vector3 MaxScale;
        public float RotateSpeed = 0.03f;
        public float ScaleFactor = 0.03f;

        public Quaternion Rotate(Quaternion from, float amount) {
            Quaternion smallRotation = Quaternion.AngleAxis(amount * RotateSpeed, GetRotateAxis());

            if (from == Quaternion.identity) {
                return smallRotation; 
            } else {
                return smallRotation * from;
            }
        }

        public Vector3 Scale(Vector3 from, float amount) {
            Vector3 scale = GetScaleAxes();
            Vector3 to = from + new Vector3(
                amount * ScaleFactor * scale.x,
                amount * ScaleFactor * scale.y,
                amount * ScaleFactor * scale.z
            );

            return new Vector3(Mathf.Clamp(to.x, MinScale.x, MaxScale.x), Mathf.Clamp(to.y, MinScale.y, MaxScale.y), Mathf.Clamp(to.z, MinScale.z, MaxScale.z));
        }

        private Vector3 GetRotateAxis() {
            switch (RotateAxis)
            {
                case RotateAxis.xAxis:
                    return new Vector3(1f,0,0);
                case RotateAxis.yAxis:
                    return new Vector3(0,1f,0);
                case RotateAxis.zAxis:
                    return new Vector3(0,0,1f);
            }

            return new Vector3(1f,0,0);
        }

        private Vector3 GetScaleAxes() {
            return new Vector3(
                (ScaleAxis & ScaleAxis.xAxis) == ScaleAxis.xAxis ? 1 : 0,
                (ScaleAxis & ScaleAxis.yAxis) == ScaleAxis.yAxis ? 1 : 0,
                (ScaleAxis & ScaleAxis.zAxis) == ScaleAxis.zAxis ? 1 : 0
            );
        }
    }
}