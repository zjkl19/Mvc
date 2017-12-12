// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// An <see cref="IModelBinderProvider"/> for binding header values.
    /// </summary>
    public class HeaderModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public HeaderModelBinderProvider()
            : this(new NullLoggerFactory())
        {
        }

        public HeaderModelBinderProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.BindingInfo.BindingSource != null &&
                context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Header))
            {
                // We only support strings and collections of strings. Some cases can fail
                // at runtime due to collections we can't modify.
                if (context.Metadata.ModelType == typeof(string) ||
                    context.Metadata.ElementType == typeof(string))
                {
                    return new HeaderModelBinder(_loggerFactory);
                }
                else
                {
                    _logger.LogDebug("Headermodelbinder only supports binding data to types of string and collection of strings");
                }
            }
            else
            {
                _logger.LogDebug("Could not user header model binder provider");
            }

            return null;
        }
    }
}
