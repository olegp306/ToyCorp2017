<%@ WebHandler Language="C#" Class="GetCustomOptions" %>

using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using Newtonsoft.Json;

public class GetCustomOptions : IHttpHandler
{
    private Product _product;
    
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (string.IsNullOrEmpty(context.Request["productId"]))
        {
            context.Response.Write(JsonConvert.SerializeObject(new { error = "product not found" }));
            return;
        }

        var productId = context.Request["productId"].TryParseInt();
        var options = context.Request["selectedOptions"];
        
        if (productId == 0 || (_product = ProductService.GetProduct(productId)) == null)
        {
            context.Response.Write(JsonConvert.SerializeObject(new { error = "product not found" }));
            return;
        }

        var kvSelectedOptions = new List<KeyValuePair<int, string>>();

        if (options.IsNotEmpty())
        {
            foreach (var selectedOption in options.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries))
            {
                var option = selectedOption.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
                if (option.Length == 2)
                {
                    kvSelectedOptions.Add(new KeyValuePair<int, string>(option[0].TryParseInt(), HttpUtility.HtmlEncode(option[1])));
                }
            }
        }

        if (kvSelectedOptions.Count == 0)
        {
            context.Response.Write(JsonConvert.SerializeObject(new {attributesXml = string.Empty}));
            return;
        }

        var customOptions = CustomOptionsService.GetCustomOptionsByProductId(productId);
        var selectedOptions = new List<OptionItem>();
        
        //customOptions.ForEach(x => x.Options.ForEach(y => y.PriceBc = GetPrice(y.PriceBc)));

        var index = 0;
        foreach (var customOption in customOptions)
        {
            var kvOption = kvSelectedOptions.Find(x => x.Key == index);
            
            if (customOption.InputType == CustomOptionInputType.DropDownList || customOption.InputType == CustomOptionInputType.RadioButton)
            {
                selectedOptions.Add(!kvOption.Equals(default(KeyValuePair<int, string>))
                                        ? customOption.Options.WithId(kvOption.Value.TryParseInt())
                                        : null);
            }
            
            if (customOption.InputType == CustomOptionInputType.CheckBox)
            {
                selectedOptions.Add(!kvOption.Equals(default(KeyValuePair<int, string>)) || customOption.IsRequired
                                        ? customOption.Options[0]
                                        : null);
            }
            
            if (customOption.InputType == CustomOptionInputType.TextBoxMultiLine || customOption.InputType == CustomOptionInputType.TextBoxSingleLine)
            {
                customOption.Options[0].Title = kvOption.Value;
                selectedOptions.Add(!kvOption.Equals(default(KeyValuePair<int, string>)) || customOption.IsRequired
                                        ? customOption.Options[0]
                                        : null);
            }
            index++;
        }

        var attributesXml = CustomOptionsService.SerializeToXml(customOptions, selectedOptions) ?? string.Empty;

        context.Response.Write(JsonConvert.SerializeObject(new {attributesXml = HttpUtility.UrlEncode(attributesXml)}));
    }

    private float GetPrice(float price)
    {
        return CatalogService.CalculateProductPrice(price, _product.Discount, CustomerContext.CurrentCustomer.CustomerGroup, null);
    }

    public bool IsReusable
    {
        get { return false; }
    }
}