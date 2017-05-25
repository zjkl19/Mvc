// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests
{
    public class FormFileUploadTest : IClassFixture<MvcTestFixture<FilesWebSite.Startup>>
    {
        public FormFileUploadTest(MvcTestFixture<FilesWebSite.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task CanUploadFileInFrom()
        {
            // Arrange
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("John"), "Name");
            content.Add(new StringContent("23"), "Age");
            content.Add(new StringContent("John's biography content"), "Biography", "Bio.txt");

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/UploadFiles");
            request.Content = content;

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var user = await response.Content.ReadAsStringAsync();

            Assert.Contains("John", user);
            Assert.Contains("23", user);
            Assert.Contains("John's biography content", user);
        }

        [Fact]
        public async Task UploadMultipleFiles()
        {
            // Arrange
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Phone"), "Name");
            content.Add(new StringContent("camera"), "Specs[0].Key");
            content.Add(new StringContent("camera spec1 file contents"), "Specs[0].Value", "camera_spec1.txt");
            content.Add(new StringContent("camera spec2 file contents"), "Specs[0].Value", "camera_spec2.txt");
            content.Add(new StringContent("battery"), "Specs[1].Key");
            content.Add(new StringContent("battery spec1 file contents"), "Specs[1].Value", "battery_spec1.txt");
            content.Add(new StringContent("battery spec2 file contents"), "Specs[1].Value", "battery_spec2.txt");

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/UploadProductSpecs");
            request.Content = content;

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var product = await response.Content.ReadAsStringAsync();
            Assert.NotNull(product);
            Assert.Contains("Phone", product);
            Assert.Contains("camera", product);
            Assert.Contains("camera_spec1.txt", product);
            Assert.Contains("battery_spec1.txt", product);
        }

        private class User
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Biography { get; set; }
        }

        private class Product
        {
            public string Name { get; set; }

            public Dictionary<string, List<string>> Specs { get; set; }
        }
    }
}
