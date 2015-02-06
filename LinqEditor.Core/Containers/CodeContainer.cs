﻿using LinqEditor.Core.Generated;
using LinqEditor.Core.Models.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LinqEditor.Core.Containers
{
    public class CodeContainer : ExecutionContainer
    {
        public LoadAppDomainResult Initialize()
        {
            base.Initialize();
            return new LoadAppDomainResult();
        }

        public ExecuteResult Execute(byte[] assembly)
        {
            return Execute(assembly, null);
        }

        public ExecuteResult Execute(string path)
        {
            return Execute(null, path);
        }

        private ExecuteResult Execute(byte[] assembly, string path)
        {
            try
            {
                var assm = !string.IsNullOrEmpty(path) ? Assembly.LoadFile(path) : Assembly.Load(assembly);
                var programType = assm.GetType(string.Format("{0}.Program", assm.GetName().Name));

                var instance = Activator.CreateInstance(programType) as ICodeProgram;
                var output = instance.Execute();
                return new ExecuteResult
                {
                    CodeOutput = output
                };
            }
            catch (Exception e)
            {
                return new ExecuteResult
                {
                    Exception = e,
                    Success = false
                };
            }
        }
    }
}
