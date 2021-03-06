﻿using Mantle.Aws;
using Mantle.Aws.Interfaces;
using Ninject.Modules;

namespace Mantle.PhotoGallery.Web.Mantle.Profiles.Default
{
    public class AwsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAwsRegionEndpoints>()
                .To<AwsRegionEndpoints>()
                .InTransientScope();
        }
    }
}