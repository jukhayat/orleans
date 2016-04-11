﻿using System;
using System.Threading.Tasks;
using Orleans.Providers.Streams.AzureQueue;
using Orleans.Runtime.Configuration;
using Orleans.TestingHost;
using UnitTests.Tester;
using Xunit;

namespace Tester.StreamingTests
{
    public class AQClientStreamTests : TestClusterPerTest
    {
        private const string AQStreamProviderName = "AzureQueueProvider";
        private const string StreamNamespace = "AQSubscriptionMultiplicityTestsNamespace";

        private ClientStreamTestRunner runner;

        public override TestCluster CreateTestCluster()
        {
            var options = new TestClusterOptions(2);
            options.ClusterConfiguration.AddMemoryStorageProvider("PubSubStore");
            options.ClusterConfiguration.AddAzureQueueStreamProvider(AQStreamProviderName);
            options.ClusterConfiguration.Globals.ClientDropTimeout = TimeSpan.FromSeconds(5);

            options.ClientConfiguration.AddAzureQueueStreamProvider(AQStreamProviderName);
            return new TestCluster(options);
        }

        public AQClientStreamTests()
        {
            runner = new ClientStreamTestRunner(this.HostedCluster);
        }

        public override void Dispose()
        {
            var deploymentId = HostedCluster.DeploymentId;
            base.Dispose();
            AzureQueueStreamProviderUtils.DeleteAllUsedAzureQueues(AQStreamProviderName, deploymentId,
                StorageTestConstants.DataConnectionString).Wait();
        }

        [Fact, TestCategory("Functional"), TestCategory("Azure"), TestCategory("Storage"), TestCategory("Streaming")]
        public async Task AQStreamProducerOnDroppedClientTest()
        {
            logger.Info("************************ AQStreamProducerOnDroppedClientTest *********************************");
            await runner.StreamProducerOnDroppedClientTest(AQStreamProviderName, StreamNamespace);
        }
    }
}
