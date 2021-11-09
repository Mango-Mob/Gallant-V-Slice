using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "characterData", menuName = "Game Data/Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    public string m_name;
    public Texture2D[] m_characterBody;
    public Texture2D[] m_characterFace;
    public Vector2[] m_faceAnchor;


    public void DrawToGUI(Vector2 position, int bodyID, int faceID, float scale)
    {
        if (bodyID < m_characterBody.Length && faceID < m_characterFace.Length)
        {
            Texture body = m_characterBody[bodyID];
            Texture face = m_characterFace[faceID];

            Vector2 anchor = Vector2.zero;
            if (bodyID < m_faceAnchor.Length)
                anchor = new Vector2(m_faceAnchor[bodyID].x, m_faceAnchor[bodyID].y);

            GUILayout.Space(body.height);
            GUILayout.BeginArea(new Rect(position.x, position.y, body.width, body.height));
            GUI.DrawTexture(new Rect(0, 0, body.width * scale, body.height * scale), body);
            GUI.DrawTexture(new Rect(anchor.x, anchor.y, face.width * scale, face.height * scale), face);
            GUILayout.EndArea();
        }
    }
    public void DrawToGUI(Rect rect, int bodyID, int faceID)
    {
        if (bodyID < m_characterBody.Length && faceID < m_characterFace.Length)
        {
            Texture body = m_characterBody[bodyID];
            Texture face = m_characterFace[faceID];

            Vector2 anchor = Vector2.zero;
            if (bodyID < m_faceAnchor.Length)
                anchor = new Vector2(m_faceAnchor[bodyID].x, m_faceAnchor[bodyID].y);

            float scale = Mathf.Min(rect.width / body.width, rect.height / body.height);
            GUILayout.Space(rect.height * scale);
            GUILayout.BeginArea(rect);
            GUI.DrawTexture(new Rect(0, 0, body.width * scale, body.height * scale), body);
            GUI.DrawTexture(new Rect(anchor.x, anchor.y, face.width * scale, face.height * scale), face);
            GUILayout.EndArea();
        }
    }

    public void DrawSliders(ref int bodyIndex, ref int faceIndex)
    {
        bodyIndex = EditorGUILayout.IntSlider("Body:", bodyIndex, 0, m_characterBody.Length - 1);
        faceIndex = EditorGUILayout.IntSlider("Face:", faceIndex, 0, m_characterFace.Length - 1);
    }
}