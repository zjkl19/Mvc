// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
{
    public class SaveTempDataPropertyFilterTestBase
    {
        public static IList<TempDataProperty> BuildTempDataProperties<TSubject>()
        {
            var subjectType = typeof(TSubject);

            var properties = subjectType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            var result = new List<TempDataProperty>();

            foreach (var property in properties)
            {
                result.Add(new TempDataProperty(property, property.GetValue, property.SetValue));
            }

            return result;
        }
    }
}
