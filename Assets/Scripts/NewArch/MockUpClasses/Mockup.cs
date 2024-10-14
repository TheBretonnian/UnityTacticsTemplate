namespace UnityEngine
{
    public abstract class MonoBehaviour
    {
        public bool enabled = true;
    }

    public abstract class ScriptableObject{}

    public class GameObject{}

    public class Vector3{
        public float x;
        public float y;
        public float z;
    }

    public class Vector2{
        public float x;
        public float y;
    }

    public class Vector2Int{
        public int x;
        public int y;

        public Vector2Int(int x, int y){
            this.x = x;
            this.y = y;
        }

        public static float Distance(Vector2Int orig, Vector2Int dest){ return 0.0f;}
    }   

    public class Camera{
        public static Camera main;
    }

    public class Ray{}

    public class RaycastHit{}
}

