﻿using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LinqEditor.Core.CodeAnalysis.Services;

namespace LinqEditor.Core.CodeAnalysis.Installers
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IAsyncTemplateCodeAnalysis>()
                                        .ImplementedBy<AsyncTemplateCodeAnalysis>()
                                        .LifestyleScoped());
        }
    }
}
