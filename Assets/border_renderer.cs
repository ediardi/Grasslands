using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class border_renderer : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    public Color color = new Color();
    //public Material lineMaterial;
    
   
    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
            lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
        }
    }
    

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        Vector3 last = points[points.Count - 1];
        for (int i = 0; i < points.Count; ++i)
        {
            // Vertex colors change from red to green
            GL.Color(color);
            // One vertex at transform position
            GL.Vertex3(last.x, last.y + 0.02f, last.z);
            // Another vertex at edge of circle
            last = points[i];
            GL.Vertex3(last.x, last.y + 0.02f, last.z);
        }
        GL.End();
        GL.PopMatrix();
    }
}
