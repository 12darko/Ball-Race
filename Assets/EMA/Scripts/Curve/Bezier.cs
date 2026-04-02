using System.Collections.Generic;
using UnityEngine;

namespace EMA.Scripts.Curve
{
    public static class Bezier
    {
        public static Vector3[] Quadratic(Vector3 point1, Vector3 point2, Vector3 point3, int smoothness)
        {
            var _pointList = new List<Vector3>();

            for (var _ratio = 0f; _ratio <= 1; _ratio += 1.0f / smoothness) {
                var _tangentLineVertex1 = Vector3.Lerp(point1, point2, _ratio);
                var _tangentLineVertex2 = Vector3.Lerp(point2, point3, _ratio);
                var _bezierPoint = Vector3.Lerp(_tangentLineVertex1, _tangentLineVertex2, _ratio);

                _pointList.Add(_bezierPoint);
            }

            return _pointList.ToArray();
        }

        public static Vector3[] Cubic(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, int smoothness)
        {
            var _points = new List<Vector3>();

            for (float _ratio = 0f; _ratio <= 1f; _ratio += 1.0f / smoothness) {
                var _tangentLineVertex1 = Vector3.Lerp(point1, point2, _ratio);
                var _tangentLineVertex2 = Vector3.Lerp(point2, point3, _ratio);
                var _tangentLineVertex3 = Vector3.Lerp(point3, point4, _ratio);

                var _quadraticPoint1 = Vector3.Lerp(_tangentLineVertex1, _tangentLineVertex2, _ratio);
                var _quadraticPoint2 = Vector3.Lerp(_tangentLineVertex3, _tangentLineVertex3, _ratio);

                var _cubicBezier = Vector3.Lerp(_quadraticPoint1, _quadraticPoint2, _ratio);

                _points.Add(_cubicBezier);
            }
            
            return _points.ToArray();
        }

        public static void GizmoHelper(Vector3[] points, Color gizmoColor)
        {
            for (int i = 0; i < points.Length; i++) {
                if(i == points.Length - 1)
                    return;

                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
    }
}
