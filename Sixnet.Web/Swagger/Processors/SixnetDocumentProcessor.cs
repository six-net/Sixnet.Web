// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

using Sixnet.Web.Extensions;

namespace Sixnet.Web.Swagger.Processors
{
    internal class SixnetDocumentProcessor : IDocumentProcessor
    {
        ApiDocGroupItem _groupItem;

        internal SixnetDocumentProcessor(ApiDocGroupItem groupItem)
        {
            _groupItem = groupItem;
        }

        public void Process(DocumentProcessorContext context)
        {
            if (!context.Document.Tags.IsNullOrEmpty())
            {
                var operationTags = context.Document.Operations?.SelectMany(c => c.Operation.Tags).Distinct();
                context.Document.Tags = context.Document.Tags.Where(c => operationTags.Contains(c.Name)).ToList(); ;

            }
        }
    }
}
