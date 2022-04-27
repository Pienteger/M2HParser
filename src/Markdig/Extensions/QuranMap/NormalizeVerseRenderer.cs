using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.QuranMap;

public class NormalizeVerseRenderer : NormalizeObjectRenderer<QuranVerse>
{
    protected override void Write(NormalizeRenderer renderer, QuranVerse obj)
    {
        renderer.Write(obj.Verse);
        renderer.Write("-");
        renderer.Write(obj.VerseInfo);
    }
}