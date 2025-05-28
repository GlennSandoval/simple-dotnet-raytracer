using System.Collections.Generic;
using System.IO;
using raytracer.geometry;
using Newtonsoft.Json;

namespace raytracer;

/// <summary>
/// A simple scene for a simple raytracer.
/// A scene consists of a collection of shapes and lights.
/// </summary>
public class Scene
{
    private Camera m_Camera;
    private List<Geometry> m_Geoms = [];
    private List<Light> m_Lights = [];

    public Scene(Camera? camera)
    {
        if (camera != null)
        {
            m_Camera = camera;
        }
        else
        {
            m_Camera = new Camera();
        }
    }

    public Camera Camera
    {
        get
        {
            return m_Camera;
        }
        set
        {
            m_Camera = value;
        }
    }

    /// <summary>
    /// Loads a scene file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>The scene to be rendered</returns>
    public static Scene Load(string filePath)
    {
        using JsonReader reader = new JsonTextReader(new StreamReader(filePath));
        JsonSerializer ser = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
        return ser.Deserialize<Scene>(reader)!;
    }

    /// <summary>
    /// Serializes this scene to a file named scene.json
    /// TODO: allow naming of file
    /// </summary>
    public void Serialize()
    {
        using JsonWriter writer = new JsonTextWriter(new StreamWriter("scene.json"));
        JsonSerializer ser = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };
        ser.Serialize(writer, this);
    }

    public List<Geometry> Geoms
    {
        get
        {
            return m_Geoms;
        }
        set
        {
            m_Geoms = value;
        }
    }

    public List<Light> Lights
    {
        get
        {
            return m_Lights;
        }
        set
        {
            m_Lights = value;
        }
    }
}
