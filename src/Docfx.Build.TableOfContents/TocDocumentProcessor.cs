﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Composition;
using System.Web;

using Docfx.Common;
using Docfx.DataContracts.Common;
using Docfx.Plugins;

namespace Docfx.Build.TableOfContents;

[Export(typeof(IDocumentProcessor))]
public class TocDocumentProcessor : TocDocumentProcessorBase
{
    public override string Name => nameof(TocDocumentProcessor);

    [ImportMany(nameof(TocDocumentProcessor))]
    public override IEnumerable<IDocumentBuildStep> BuildSteps { get; set; }

    public override ProcessingPriority GetProcessingPriority(FileAndType file)
    {
        if (file.Type == DocumentType.Article && Utility.IsSupportedFile(file.File))
        {
            return ProcessingPriority.High;
        }

        return ProcessingPriority.NotSupported;
    }

    protected override void RegisterTocToContext(TocItemViewModel toc, FileModel model, IDocumentBuildContext context)
    {
        var key = model.Key;

        // Add current folder to the toc mapping, e.g. `a/` maps to `a/toc`
        var directory = ((RelativePath)key).GetPathFromWorkingFolder().GetDirectoryPath();
        context.RegisterToc(key, directory);

        var tocInfo = new TocInfo(key);
        if (toc.Homepage != null)
        {
            if (PathUtility.IsRelativePath(toc.Homepage))
            {
                var pathToRoot = ((RelativePath)model.File + (RelativePath)HttpUtility.UrlDecode(toc.Homepage)).GetPathFromWorkingFolder();
                tocInfo.Homepage = pathToRoot;
            }
        }

        context.RegisterTocInfo(tocInfo);
    }

    protected override void RegisterTocMapToContext(TocItemViewModel item, FileModel model, IDocumentBuildContext context)
    {
        var key = model.Key;
        // If tocHref is set, href is originally RelativeFolder type, and href is set to the homepage of TocHref,
        // So in this case, TocHref should be used to in TocMap
        // TODO: what if user wants to set TocHref?
        var tocHref = item.TocHref;
        var tocHrefType = Utility.GetHrefType(tocHref);
        if (tocHrefType == HrefType.MarkdownTocFile || tocHrefType == HrefType.YamlTocFile)
        {
            context.RegisterToc(key, HttpUtility.UrlDecode(UriUtility.GetPath(tocHref)));
        }
        else
        {
            var href = item.Href; // Should be original href from working folder starting with ~
            if (Utility.IsSupportedRelativeHref(href))
            {
                context.RegisterToc(key, HttpUtility.UrlDecode(UriUtility.GetPath(href)));
            }
        }
    }
}
