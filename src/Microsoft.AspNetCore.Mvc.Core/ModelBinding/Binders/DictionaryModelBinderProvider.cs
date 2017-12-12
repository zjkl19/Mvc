// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// An <see cref="IModelBinderProvider"/> for binding <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public class DictionaryModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public DictionaryModelBinderProvider()
            : this(new NullLoggerFactory())
        {
        }

        public DictionaryModelBinderProvider(ILoggerFactory loggerFactory)
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

            var modelType = context.Metadata.ModelType;
            var dictionaryType = ClosedGenericMatcher.ExtractGenericInterface(modelType, typeof(IDictionary<,>));
            if (dictionaryType != null)
            {
                var keyType = dictionaryType.GenericTypeArguments[0];
                var keyBinder = context.CreateBinder(context.MetadataProvider.GetMetadataForType(keyType));

                var valueType = dictionaryType.GenericTypeArguments[1];
                var valueBinder = context.CreateBinder(context.MetadataProvider.GetMetadataForType(valueType));

                var binderType = typeof(DictionaryModelBinder<,>).MakeGenericType(dictionaryType.GenericTypeArguments);
                return (IModelBinder)Activator.CreateInstance(binderType, keyBinder, valueBinder);
            }
            else
            {
                _logger.LogDebug($"Could not create a dicitionary binder for model type {modelType}.");
            }

            return null;
        }
    }
}
