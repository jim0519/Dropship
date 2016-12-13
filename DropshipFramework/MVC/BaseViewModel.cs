using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DropshipFramework.MVC
{
    public class BaseViewModel
    {
        public BaseViewModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {

        }

        /// <summary>
        /// Use this property to store any custom value for your models. 
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
    }

    /// <summary>
    /// Base entity view model
    /// </summary>
    public partial class BaseEntityViewModel : BaseViewModel
    {
        public virtual int ID { get; set; }
    }
}
