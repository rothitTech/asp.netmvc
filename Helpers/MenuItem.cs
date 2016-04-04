namespace EDS.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public static class MenuLinkExtension
    {
        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string itemText, string actionName, string controllerName, MvcHtmlString[] childElements = null)
        {
            var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
            string finalHtml;
            var linkBuilder = new TagBuilder("a");
            var liBuilder = new TagBuilder("li");

            if (childElements != null && childElements.Length > 0)
            {
                linkBuilder.MergeAttribute("href", "#");
                linkBuilder.AddCssClass("dropdown-toggle");
                linkBuilder.InnerHtml = itemText + " <b class=\"caret\"></b>";
                linkBuilder.MergeAttribute("data-toggle", "dropdown");
                var ulBuilder = new TagBuilder("ul");
                ulBuilder.AddCssClass("dropdown-menu");
                ulBuilder.MergeAttribute("role", "menu");
                foreach (var item in childElements)
                {
                    ulBuilder.InnerHtml += item.ToString() + "\n";
                }

                liBuilder.InnerHtml = linkBuilder.ToString() + "\n" + ulBuilder.ToString();
                liBuilder.AddCssClass("dropdown");
                if (controllerName == currentController)
                {
                    liBuilder.AddCssClass("active");
                }

                finalHtml = liBuilder.ToString() + ulBuilder.ToString();
            }
            else
            {
                UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
                linkBuilder.MergeAttribute("href", urlHelper.Action(actionName, controllerName));
                linkBuilder.SetInnerText(itemText);
                liBuilder.InnerHtml = linkBuilder.ToString();
                if (controllerName == currentController && actionName == currentAction)
                {
                    liBuilder.AddCssClass("active");
                }

                finalHtml = liBuilder.ToString();
            }

            return new MvcHtmlString(finalHtml);
        }
    }
    //public class Helpers
    //{
    //    public static List<SelectListItem> GetDropDownList<T>(
    //           string text, string value, string selected) where T : class
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();
    //        list.Add(new SelectListItem { Text = "-Please select-", Value = string.Empty });
    //        IQueryable<T> result = Db.Repository<T>();
    //        var lisData = (from items in result
    //                       select items).AsEnumerable().Select(m => new SelectListItem
    //                       {
    //                           Text = (string)m.GetType().GetProperty(text).GetValue(m),
    //                           Value = (string)m.GetType().GetProperty(value).GetValue(m),
    //                           Selected = (selected != "") ? ((string)
    //                             m.GetType().GetProperty(value).GetValue(m) ==
    //                             selected ? true : false) : false,
    //                       }).ToList();
    //        list.AddRange(lisData);
    //        return list;
    //    }
    //}



}