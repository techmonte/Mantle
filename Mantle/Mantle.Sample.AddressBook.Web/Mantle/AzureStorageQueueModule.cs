﻿using Mantle.Messaging;
using Mantle.Messaging.Azure;
using Ninject.Modules;

namespace Mantle.Sample.AddressBook.Web.Mantle
{
    public class AzureStorageQueueModule : NinjectModule
    {
        public override void Load()
        {
            LoadPublisherEndpoints();
            LoadSubscriberEndpoints();
        }

        private void LoadPublisherEndpoints()
        {
            // TODO: Setup publisher endpoints.

            Bind<IPublisherEndpoint>()
                .To<AzureStorageQueuePublisherEndpoint>()
                .InSingletonScope()
                .OnActivation(c => c.Setup("PersonQueue", "mantletestqueue"));
        }

        private void LoadSubscriberEndpoints()
        {
            // TODO: Setup subscriber endpoints.
        }
    }
}