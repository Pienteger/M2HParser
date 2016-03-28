﻿// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.SmartyPants
{
    /// <summary>
    /// A HTML renderer for a <see cref="SmartyPant"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{SmartyPant}" />
    public class HtmlSmartyPantRenderer : HtmlObjectRenderer<SmartyPant>
    {
        private static readonly SmartyPantOptions DefaultOptions = new SmartyPantOptions();

        private readonly SmartyPantOptions options;

        public HtmlSmartyPantRenderer(SmartyPantOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            this.options = options;
        }

        protected override void Write(HtmlRenderer renderer, SmartyPant obj)
        {
            string text;
            if (!options.Mapping.TryGetValue(obj.Type, out text))
            {
                DefaultOptions.Mapping.TryGetValue(obj.Type, out text);
            }
            renderer.Write(text);
        }
    }
}