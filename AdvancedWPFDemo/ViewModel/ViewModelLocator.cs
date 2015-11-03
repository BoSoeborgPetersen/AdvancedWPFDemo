using _02350AdvancedDemo.UndoRedo;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var container = new UnityContainer();
            var locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Insert dependencies for design-time data.
            }
            else
            {
                container.RegisterType<DialogViews>();
                //container.RegisterType<UndoRedoController>();
            }

            container.RegisterType<MainViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}
