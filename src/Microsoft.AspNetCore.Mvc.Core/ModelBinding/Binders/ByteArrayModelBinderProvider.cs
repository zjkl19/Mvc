// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// An <see cref="IModelBinderProvider"/> for binding base64 encoded byte arrays.
    /// </summary>
    public class ByteArrayModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ByteArrayModelBinderProvider"/>.
        /// </summary>
        public ByteArrayModelBinderProvider()
            : this(new NullLoggerFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ByteArrayModelBinderProvider"/>.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public ByteArrayModelBinderProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(byte[]))
            {
                return new ByteArrayModelBinder(_loggerFactory);
            }
            else
            {
                _logger.LogDebug("Cannot use byte array model binder as it supports binding to only byte[] type");
            }

            return null;
        }
    }
}
