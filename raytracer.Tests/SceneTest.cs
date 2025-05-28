namespace raytracer.Tests;

using Xunit;
using raytracer;

public class SceneSerializationTests
{
    [Fact]
    public void SceneLoads()
    {
        Scene s = Scene.Load("scene.json");
        Assert.NotNull(s);
        Assert.Equal(5, s.Geoms.Count);
    }
}
