using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;
using QuranLib;
using System.Text;

namespace Markdig.Extensions.QuranMap;

public class QuranMapInlineParser : InlineParser
{

    public QuranMapInlineParser()
    {
        OpeningCharacters = new[] { 'Q' };
    }
    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // Allow preceding whitespace or `(`
        char pc = slice.PeekCharExtra(-1);
        if (!pc.IsWhiteSpaceOrZero() && pc != '(')
        {
            return false;
        }

        char current = slice.CurrentChar;
        var index = 0;

        // Base pattern is 'Quran(000:000)' or 'Quran(000:000-000)'
        var startingChars = new[] { 'Q', 'u', 'r', 'a', 'n', '(' };
        while (index < startingChars.Length && current == startingChars[index])
        {
            index++;
            current = slice.NextChar();
        }
        if (index != startingChars.Length | !current.IsDigit())
        {
            return false;
        }
        index = 0;
        var chapterNumber = 0;
        while (index < 3 && current.IsDigit())
        {
            chapterNumber = chapterNumber * 10 + current - '0';
            current = slice.NextChar();
            index++;
        }
        // Require a ':' to separate the sura and ayah
        if (current != ':' | chapterNumber < 1 | chapterNumber > 114)
        {
            return false;
        }
        current = slice.NextChar(); // skip :
        var startVerse = 0;
        var endVerse = 0;
        index = 0;
        while (index < 3 && current.IsDigit())
        {
            startVerse = startVerse * 10 + current - '0';
            current = slice.NextChar();
            index++;
        }

        if (current == ')')
        {
            processor.Inline = GetVerse(processor, ref slice, chapterNumber, startVerse, startVerse);
            return true;
        }
        if (current != '-' | startVerse < 1 | startVerse > 286)
        {
            return false;
        }

        current = slice.NextChar(); // skip :
        while (index < 3 && current.IsDigit())
        {
            endVerse = endVerse * 10 + current - '0';
            current = slice.NextChar();
            index++;
        }
        if (current != ')' | startVerse > endVerse | endVerse > 286)
        {
            return false;
        }
        
        processor.Inline = GetVerse(processor, ref slice, chapterNumber, startVerse, endVerse);
        return true;
    }


    private static QuranVerse GetVerse(InlineProcessor processor, ref StringSlice slice,
        int chapterNumber, int startVerse, int endVerse)
    {
        slice.NextChar(); // skip )
        var quran = new QuranInstance(ScriptType.WithTashkil);
        quran.LoadChapter((byte)chapterNumber);
        var verse = new QuranVerse()
        {
            Span =
            {
                Start = processor.GetSourcePosition(slice.Start, out int line, out int column)
            },
            Line = line,
            Column = column,
        };
        string verseText;
        string verseInfo;
        if (startVerse == endVerse)
        {
            verseText = quran.GetVerse((byte)chapterNumber, (ushort)startVerse).Text;
            verseInfo = $"{quran.GetChapterNames()[(byte)chapterNumber - 1]} - {startVerse}";
        }
        else
        {
            var sb = new StringBuilder();
            foreach (Verse v in quran.GetVerses((byte)chapterNumber, (ushort)startVerse, (ushort)endVerse))
            {
                sb.Append(v.Text);
            }
            verseText = sb.ToString();
            verseInfo = $"{quran.GetChapterNames()[(byte)chapterNumber - 1]} - {startVerse}:{endVerse}";
        }
        verse.Verse = new StringSlice(verseText);
        verse.VerseInfo = new StringSlice(verseInfo);

        var builder = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        builder.Append(verse.Verse.AsSpan());
        builder.Append(verse.VerseInfo.AsSpan());

        verse.AppendChild(new LiteralInline(builder.ToString()));
        return verse;
    }
}

