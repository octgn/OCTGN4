using HtmlTags;
using HtmlTags.Reflection;
using Nancy.Validation;
using Nancy.ViewEngines.Razor;
using System;
using System.Linq.Expressions;

namespace Octgn.UI
{
    public static class HtmlTagHelpers
    {
        public static HtmlTag Textbox<T>(this HtmlHelpers<T> html, Expression<Func<T, object>> valueSelector)
        {
            var name = valueSelector.GetName();
            return new TextboxTag(name, valueSelector.Compile().Invoke(html.Model)?.ToString()).Id(name);
        }

        public static HiddenTag HiddenTag<T>(this HtmlHelpers<T> html, Expression<Func<T, object>> valueSelector)
        {
            var name = valueSelector.GetName();
            var result = new HiddenTag();
            result.Name(name).Id(name);
            return result;
        }

        public static HtmlTag ValidationText<T>(this HtmlHelpers<T> html, Expression<Func<T, object>> valueSelector)
        {
            var name = valueSelector.GetName();
            return html.ValidationText(name);
        }
        public static HtmlTag ValidationText<T>(this HtmlHelpers<T> html, string name)
        {
            return HtmlTag.Empty();
            var result = html.RenderContext.Context.ModelValidationResult;
            if (result.IsValid) return HtmlTag.Empty();
            if (!result.Errors.ContainsKey(name)) return HtmlTag.Empty();
            var error = result.Errors[name];
            if (error.Count == 0) return HtmlTag.Empty();
            var tag = new DivTag();
            tag.Text(error[0]);
            return tag.AddClass("error");
        }

        public static IHtmlString Raw(this HtmlTag tag)
        {
            return new NonEncodedHtmlString(tag.ToString());
        }
    }
}