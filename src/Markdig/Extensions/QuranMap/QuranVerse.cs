using Markdig.Helpers;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace Markdig.Extensions.QuranMap;

[DebuggerDisplay("{VerseInfo}")]
public class QuranVerse : LinkInline
{
    public QuranVerse()
    {
        IsClosed = true;
    }

    public StringSlice Verse { get; set; }

    /// <summary>
    /// (Chapter Name - Starting Verse : Ending Verse)
    /// </summary>
    public StringSlice VerseInfo { get; set; }
}