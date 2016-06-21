using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RayTracer
{

    /// <summary>
    /// A simple scene for a simple raytracer.
    /// A scene consists of a collection of shapes and lights.
    /// </summary>
    public class Scene
    {
        private Camera m_Camera;
        private List<Geometry> m_Geoms = new List<Geometry>();
        private List<Light> m_Lights = new List<Light>();

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
            using (StreamReader sr = new StreamReader(filePath))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.NullValueHandling = NullValueHandling.Ignore;
                ser.TypeNameHandling = TypeNameHandling.Auto;
                ser.ObjectCreationHandling = ObjectCreationHandling.Replace;
                return ser.Deserialize<Scene>(reader);
            }
        }

        /// <summary>
        /// Serializes this scene to a file named scene.json
        /// TODO: allow naming of file
        /// </summary>
        public void Serialize()
        {
            using (StreamWriter sw = new StreamWriter("scene.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.NullValueHandling = NullValueHandling.Ignore;
                ser.TypeNameHandling = TypeNameHandling.Auto;
                ser.Serialize(writer, this);
            }
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
}