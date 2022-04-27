using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Normalize;
using Markdig.Renderers.Normalize.Inlines;
namespace Markdig.Extensions.QuranMap
{
    public class QuranMapExtension : IMarkdownExtension
    {
        //private readonly QuranMapOptions options;

        //public QuranMapExtension(QuranMapOptions options)
        //{
        //    this.options = options;
        //}
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<QuranMapInlineParser>())
            {
                // Insert the parser before the link inline parser
                // pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new QuranMapInlineParser(options));
                pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new QuranMapInlineParser());

            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is NormalizeRenderer normalizeRenderer && !normalizeRenderer.ObjectRenderers.Contains<NormalizeVerseRenderer>())
            {
                normalizeRenderer.ObjectRenderers.InsertBefore<LinkInlineRenderer>(new NormalizeVerseRenderer());
            }
        }
    }
}
