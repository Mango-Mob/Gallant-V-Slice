using UnityEngine;

namespace GEN.Nodes
{
    /****************
     * PreviewNode : A component used to manage the preview texture for the levelStart component.
     * @author : Michael Jordan
     * @file : PreviewNode.cs
     * @year : 2021
     */
    public class PreviewNode : MonoBehaviour
    {
        //Camera components:
        private Camera m_myCamera;
        private LayerMask m_layers;

        //Camera's render target
        public RenderTexture m_texture { get; private set; }

        //Position manipulation
        public Vector3 m_positionInitial { get; set; }
        public Vector3 m_positionOffset { get; private set; }
        public float m_sizeOffset { get; private set; }

        //Called when the component is loaded into the scene (Immediately).
        private void Awake()
        {
            //Initialise the fundamental variables.
            transform.position = m_positionInitial + m_positionOffset;
            m_positionInitial = transform.position;
            m_positionOffset = Vector3.zero;
            m_sizeOffset = 150;

            //Create a render texture at run time
            m_texture = new RenderTexture((int)960, (int)540, 24, RenderTextureFormat.ARGB32);

            //Create a camera if it doesn't exist
            if (GetComponent<Camera>() == null)
                gameObject.AddComponent<Camera>();

            //Camera Settings
            m_myCamera = GetComponent<Camera>();
            m_myCamera.hideFlags = HideFlags.HideAndDontSave;
            m_myCamera.orthographic = true;
            m_myCamera.orthographicSize = m_sizeOffset;
            m_myCamera.cullingMask = m_layers;
            m_myCamera.targetDisplay = 2;
            m_myCamera.targetTexture = m_texture;
        }

        /*******************
         * SetOffset : Sets the camera's offset position and orthographic Size.
         * @author : Michael Jordan
         * @param : (Vector3) Offset from initial position.
         * @param : (float) New Size for the orthographic camera.
         */
        public void SetOffset(Vector3 _position, float size)
        {
            //Set new offset variables
            m_positionOffset = _position;
            m_sizeOffset = size;

            //Update the status of this gameObject
            transform.position = m_positionInitial + m_positionOffset;
            m_myCamera.orthographicSize = m_sizeOffset;
        }

        /*******************
         * Render : Forward's the call to the preview camera to do a render update.
         * @author : Michael Jordan
         */
        public void Render()
        {
            m_myCamera.Render();
        }

        /*******************
         * Instantiate : A static function that creates a peview camera in the game world.
         * @author : Michael Jordan
         * @param : (Transform) Transform of the target.
         * @param : (Vector3) Forward of the camera.
         * @param : (LayerMask) LayerMask to cull for the texture.
         */
        public static PreviewNode Instantiate(Transform _target, Vector3 _forward, LayerMask _layers)
        {
            //Create gameObject
            GameObject temp = new GameObject();
            temp.name = "PreviewNode";

            //Set position and orientation
            temp.transform.position = _target.position - _forward.normalized;
            temp.transform.forward = _forward.normalized;

            //Create and return a new PreviewNode
            PreviewNode node = temp.AddComponent<PreviewNode>();
            node.m_layers = _layers;
            node.Awake();
            return node;
        }

        /*******************
         * CleanAll : Removes all instances of a preview node from the game world.
         * @author : Michael Jordan
         */
        public static void CleanAll()
        {
            PreviewNode[] node = FindObjectsOfType<PreviewNode>();
            for (int i = 0; i < node.Length; i++)
            {
                DestroyImmediate(node[i].gameObject);
            }
        }
    }
}

