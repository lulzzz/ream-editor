﻿using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.Installer;
using LinqEditor.UI.WinForm;
using LinqEditor.UI.WinForm.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqEditor.Installers
{
    public class Installer : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();

            container.Install(FromAssembly.Named("LinqEditor.Core.CodeAnalysis"));
            container.Install(FromAssembly.Named("LinqEditor.Core.Templates"));
            container.Install(FromAssembly.Named("LinqEditor.Core.Backend"));
            container.Install(FromAssembly.Named("LinqEditor.Core.Schema"));

            // install forms
            container.Register(Component.For<MainForm>());
            container.Register(Component.For<ICodeEditorFactory>().AsFactory());
            container.Register(Component.For<IOutputPaneFactory>().AsFactory());

        }
    }
}
