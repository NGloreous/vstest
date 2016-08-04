// Copyright (c) Microsoft. All rights reserved.

namespace TestPlatform.Common.UnitTests.ExtensionFramework
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestDiscoveryExtensionManagerTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            TestDiscoveryExtensionManager.Destroy();
        }

        [TestMethod]
        public void CreateShouldDiscoverDiscovererExtensions()
        {
            TestPluginCacheTests.SetupMockExtensions();

            var extensionManager = TestDiscoveryExtensionManager.Create();

            Assert.IsNotNull(extensionManager.Discoverers);
            Assert.IsTrue(extensionManager.Discoverers.Count() > 0);
        }

        [TestMethod]
        public void CreateShouldCacheDiscoveredExtensions()
        {
            var discoveryCount = 0;
            TestPluginCacheTests.SetupMockExtensions(() => { discoveryCount++; });

            var extensionManager = TestDiscoveryExtensionManager.Create();
            TestDiscoveryExtensionManager.Create();

            Assert.IsNotNull(extensionManager.Discoverers);
            Assert.IsTrue(extensionManager.Discoverers.Count() > 0);
            Assert.AreEqual(1, discoveryCount);
        }

        [TestMethod]
        public void GetDiscoveryExtensionManagerShouldReturnADiscoveryManagerWithExtensions()
        {
            var extensionManager =
                TestDiscoveryExtensionManager.GetDiscoveryExtensionManager(
                    typeof(TestDiscoveryExtensionManagerTests).GetTypeInfo().Assembly.Location);

            Assert.IsNotNull(extensionManager.Discoverers);
            Assert.IsTrue(extensionManager.Discoverers.Count() > 0);
        }

        #region LoadAndInitialize tests

        [TestMethod]
        public void LoadAndInitializeShouldInitializeAllExtensions()
        {
            TestPluginCacheTests.SetupMockExtensions();

            TestDiscoveryExtensionManager.LoadAndInitializeAllExtensions(false);

            var allDiscoverers = TestDiscoveryExtensionManager.Create().Discoverers;

            foreach (var discoverer in allDiscoverers)
            {
                Assert.IsTrue(discoverer.IsExtensionCreated);
            }
        }

        #endregion
    }

    [TestClass]
    public class TestDiscovererMetadataTests
    {
        [TestMethod]
        public void TestDiscovererMetadataCtorDoesNotThrowWhenFileExtensionsIsNull()
        {
            var metadata = new TestDiscovererMetadata(null, null);

            Assert.IsNull(metadata.FileExtension);
        }

        [TestMethod]
        public void TestDiscovererMetadataCtorDoesNotThrowWhenFileExtensionsIsEmpty()
        {
            var metadata = new TestDiscovererMetadata(new List<string> {}, null);

            Assert.IsNull(metadata.FileExtension);
        }

        [TestMethod]
        public void TestDiscovererMetadataCtorDoesNotThrowWhenDefaultUriIsNull()
        {
            var metadata = new TestDiscovererMetadata(new List<string> { }, null);

            Assert.IsNull(metadata.DefaultExecutorUri);
        }

        [TestMethod]
        public void TestDiscovererMetadataCtorDoesNotThrowWhenDefaultUriIsEmpty()
        {
            var metadata = new TestDiscovererMetadata(new List<string> { }, " ");

            Assert.IsNull(metadata.DefaultExecutorUri);
        }

        [TestMethod]
        public void TestDiscovererMetadataCtorSetsFileExtensions()
        {
            var extensions = new List<string> { "csv", "dll" };
            var metadata = new TestDiscovererMetadata(extensions, null);

            CollectionAssert.AreEqual(extensions, metadata.FileExtension.ToList());
        }

        [TestMethod]
        public void TestDiscovererMetadataCtorSetsDefaultUri()
        {
            var metadata = new TestDiscovererMetadata(null, "executor://helloworld");

            Assert.AreEqual("executor://helloworld/", metadata.DefaultExecutorUri.AbsoluteUri);
        }
    }
}