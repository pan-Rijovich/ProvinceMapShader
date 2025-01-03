namespace UnityEngine
{
    public static class VectorExtension
    {
        #region GetDirectionTo
        public static Vector2 GetDirectionTo(this Vector2 from, Vector2 to)
        {
            return (to - from).normalized;
        }
        public static Vector3 GetDirectionTo(this Vector2 from, Vector3 to)
        {
            return (to - (Vector3)from).normalized;
        }
        public static Vector4 GetDirectionTo(this Vector2 from, Vector4 to)
        {
            return (to - (Vector4)from).normalized;
        }




        public static Vector3 GetDirectionTo(this Vector3 from, Vector2 to)
        {
            return ((Vector3)to - from).normalized;
        }

        public static Vector3 GetDirectionTo(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }
        public static Vector4 GetDirectionTo(this Vector3 from, Vector4 to)
        {
            return (to - (Vector4)from).normalized;
        }




        public static Vector4 GetDirectionTo(this Vector4 from, Vector2 to)
        {
            return ((Vector4)to - from).normalized;
        }

        public static Vector4 GetDirectionTo(this Vector4 from, Vector3 to)
        {
            return ((Vector4)to - from).normalized;
        }

        public static Vector4 GetDirectionTo(this Vector4 from, Vector4 to)
        {
            return (to - from).normalized;
        }
        #endregion

        #region GetOffsetTo
        public static Vector2 GetOffsetTo(this Vector2 from, Vector2 to)
        {
            return to - from;
        }
        public static Vector3 GetOffsetTo(this Vector2 from, Vector3 to)
        {
            return to - (Vector3)from;
        }
        public static Vector4 GetOffsetTo(this Vector2 from, Vector4 to)
        {
            return to - (Vector4)from;
        }




        public static Vector3 GetOffsetTo(this Vector3 from, Vector2 to)
        {
            return (Vector3)to - from;
        }

        public static Vector3 GetOffsetTo(this Vector3 from, Vector3 to)
        {
            return to - from;
        }
        public static Vector4 GetOffsetTo(this Vector3 from, Vector4 to)
        {
            return to - (Vector4)from;
        }




        public static Vector4 GetOffsetTo(this Vector4 from, Vector2 to)
        {
            return (Vector4)to - from;
        }

        public static Vector4 GetOffsetTo(this Vector4 from, Vector3 to)
        {
            return (Vector4)to - from;
        }

        public static Vector4 GetOffsetTo(this Vector4 from, Vector4 to)
        {
            return to - from;
        }
        #endregion

        #region AngleBetween

        public static float AngleBetween(this Vector2 from, Vector2 to)
        {
            float dotProduct = Vector2.Dot(from.normalized, to.normalized);
            return Mathf.Acos(Mathf.Clamp(dotProduct, -1f, 1f)) * Mathf.Rad2Deg;
        }

        public static float AngleBetween(this Vector3 from, Vector3 to)
        {
            float dotProduct = Vector3.Dot(from.normalized, to.normalized);
            return Mathf.Acos(Mathf.Clamp(dotProduct, -1f, 1f)) * Mathf.Rad2Deg;
        }

        #endregion
        
        
        
        
        
        public static Vector2 GetPerpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.y, vector.x);
        }

        /// <summary>
        /// Повертає один із можливих перпендикулярних векторів для Vector3.
        /// </summary>
        public static Vector3 GetPerpendicular(this Vector3 vector)
        {
            // Перпендикулярний до Vector3. Вибираємо напрямок, наприклад, у площині XZ.
            if (vector != Vector3.up)
            {
                return Vector3.Cross(vector, Vector3.up).normalized;
            }
            else
            {
                return Vector3.Cross(vector, Vector3.forward).normalized;
            }
        }


    }
}
