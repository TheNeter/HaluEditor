using System;
using System.Collections.Generic;
using System.Linq;

namespace ngprojects.HaluEditor
{
    public class ServiceHost
    {
        private readonly List<IBaseServiceProvider> _Services = new List<IBaseServiceProvider>();

        public void Create(HaluEditorControl control)
        {
            var type = typeof(IBaseServiceProvider);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);
            foreach (var t in types)
            {
                IBaseServiceProvider bsp = (IBaseServiceProvider)Activator.CreateInstance(t);
                bsp.Host = this;
                bsp.Parent = control;

                _Services.Add(bsp);
            }
            FireLoadingDone();
        }

        public void FireLoadingDone()
        {
            LoadingDone();
        }

        public IBaseServiceProvider GetServiceFromType<T>()
        {
            foreach (var Service in _Services)
            {
                if (Service.GetType() == typeof(T))
                {
                    return Service;
                }
            }
            return null;
        }

        private void LoadingDone()
        {
            foreach (var s in _Services)
            {
                s.LoadingDoneEvent();
            }
        }
    }
}