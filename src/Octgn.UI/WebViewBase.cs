using Nancy.Localization;
using Nancy.ViewEngines.Razor;

namespace Octgn.UI
{
    public abstract class WebViewBase<T> : NancyRazorViewBase<T>
    {
        public IHtmlString RT(string name)
        {
            var val = this.Text["Text." + name];
            return new NonEncodedHtmlString(val as string);
        }
    }
}
