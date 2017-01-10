//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using System.Drawing;

namespace AdvantShop.Modules
{
    public class ModulesRenderer
    {
        #region PictureModules

        public static void ProcessPhoto(Image image)
        {
            foreach (var cls in AttachedModules.GetModules<IProcessPhoto>())
            {
                MethodInfo method = cls.GetMethod("DoProcessPhoto");
                if (method != null)
                {
                    var classInstance = (IProcessPhoto)Activator.CreateInstance(cls, null);
                    method.Invoke(classInstance, new object[] { image });
                }
            }
        }

        #endregion

        #region OrderModules

        public static void OrderAdded(int orderId)
        {
            IOrder order = null;

            var modules = AttachedModules.GetModules<IOrderChanged>();

            if (modules.Count > 0)
                order = OrderService.GetOrder(orderId);

            foreach (var cls in modules)
            {
                MethodInfo method = cls.GetMethod("DoOrderAdded");
                if (method != null)
                {
                    var classInstance = (IOrderChanged)Activator.CreateInstance(cls, null);
                    method.Invoke(classInstance, new object[] { order });
                }
            }
        }
        
        public static void OrderUpdated(int orderId)
        {
            IOrder order = null;

            var modules = AttachedModules.GetModules<IOrderChanged>();

            if (modules.Count > 0)
                order = OrderService.GetOrder(orderId);

            foreach (var cls in modules)
            {
                MethodInfo method = cls.GetMethod("DoOrderUpdated");
                if (method != null)
                {
                    var classInstance = (IOrderChanged)Activator.CreateInstance(cls, null);
                    if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId))
                    {
                        method.Invoke(classInstance, new object[] { order });
                    }
                }
            }
        }

        public static void OrderDeleted(int orderId)
        {
            foreach (var cls in AttachedModules.GetModules<IOrderChanged>())
            {
                MethodInfo method = cls.GetMethod("DoOrderDeleted");
                if (method != null)
                {
                    var classInstance = (IOrderChanged)Activator.CreateInstance(cls, null);
                    if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId))
                    {
                        method.Invoke(classInstance, new object[] { orderId });
                    }
                }
            }
        }

        #endregion

        #region DetailsModules

        public static string RenderDetailsModulesToRightColumn()
        {
            var result = string.Empty;
            foreach (var cls in AttachedModules.GetModules<IModuleDetails>())
            {
                MethodInfo method = cls.GetMethod("RenderToRightColumn");
                if (method != null)
                {
                    var classInstance = (IModuleDetails)Activator.CreateInstance(cls, null);
                    result += method.Invoke(classInstance, null);
                }
            }
            return result;
        }

        public static void RenderDetailsModulesToProductInformation()
        {
            foreach (var cls in AttachedModules.GetModules<IModuleDetails>())
            {
                MethodInfo method = cls.GetMethod("RenderToProductInformation");
                if (method != null)
                {
                    var classInstance = (IModuleDetails)Activator.CreateInstance(cls, null);
                    if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId))
                    {
                        method.Invoke(classInstance, null);
                    }
                }
            }
        }

        public static void RenderDetailsModulesToBottom()
        {
            foreach (var cls in AttachedModules.GetModules<IModuleDetails>())
            {
                MethodInfo method = cls.GetMethod("RenderToBottom");
                if (method != null)
                {
                    var classInstance = (IModuleDetails)Activator.CreateInstance(cls, null);
                    if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId))
                    {
                        method.Invoke(classInstance, null);
                    }
                }
            }
        }

        #endregion

        #region NotificationModules

        public static void SendSms(string phoneNumber, string text)
        {
            foreach (var cls in AttachedModules.GetModules<IModuleSms>())
            {
                MethodInfo method = cls.GetMethod("SendSms");
                if (method != null)
                {
                    var classInstance = (IModuleSms)Activator.CreateInstance(cls, null);
                    method.Invoke(classInstance, new object[] { phoneNumber, text });
                }
            }
        }

        #endregion

        #region HtmlModules

        public static string RenderIntoHead()
        {
            var builder = new StringBuilder();
            foreach (var cls in AttachedModules.GetModules<IRenderIntoHtml>())
            {
                MethodInfo method = cls.GetMethod("DoRenderIntoHead");
                if (method != null)
                {
                    var classInstance = (IRenderIntoHtml)Activator.CreateInstance(cls, null);
                    builder.Append(method.Invoke(classInstance, null));
                }
            }
            return builder.ToString();
        }

        public static string RenderAfterBodyStart()
        {
            var builder = new StringBuilder();
            foreach (var cls in AttachedModules.GetModules<IRenderIntoHtml>())
            {
                MethodInfo method = cls.GetMethod("DoRenderAfterBodyStart");
                if (method != null)
                {
                    var classInstance = (IRenderIntoHtml)Activator.CreateInstance(cls, null);
                    if (classInstance.CheckAlive() && ModulesRepository.IsActiveModule(classInstance.ModuleStringId))
                    {
                        builder.Append(method.Invoke(classInstance, null));
                    }
                }
            }
            return builder.ToString();
        }

        public static string RenderBeforeBodyEnd()
        {
            var builder = new StringBuilder();
            foreach (var cls in AttachedModules.GetModules<IRenderIntoHtml>())
            {
                MethodInfo method = cls.GetMethod("DoRenderBeforeBodyEnd");
                if (method != null)
                {
                    var classInstance = (IRenderIntoHtml)Activator.CreateInstance(cls, null);
                    builder.Append(method.Invoke(classInstance, null));
                }
            }
            return builder.ToString();
        }

        public static string RenderIntoOrderConfirmationFinalStep(IOrder order)
        {
            var builder = new StringBuilder();
            foreach (var cls in AttachedModules.GetModules<IOrderRenderIntoHtml>())
            {
                MethodInfo methodOneParam = cls.GetMethod("DoRenderIntoFinalStep", new[] { typeof(IOrder) }, new[] { new ParameterModifier(1) });
                if (methodOneParam != null)
                {
                    var classInstance = (IOrderRenderIntoHtml)Activator.CreateInstance(cls, null);
                    builder.Append(methodOneParam.Invoke(classInstance, new object[] { order }));
                }
            }
            return builder.ToString();
        }

        #endregion
    }
}