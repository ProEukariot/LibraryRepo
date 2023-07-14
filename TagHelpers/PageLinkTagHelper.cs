using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace LibraryApp.TagHelpers
{
	public class PageLinkTagHelper : TagHelper
	{
		[ViewContext]
		[HtmlAttributeNotBound]
		public ViewContext Context { get; set; } = null!;

		public int LinksCount { get; set; } = 3;

		public string PageAction { get; set; }
		public IPaginable<Book> ViewModel { get; set; }

		private readonly IUrlHelperFactory urlFactory;
#pragma warning disable
		public PageLinkTagHelper(IUrlHelperFactory urlFactory)
		{
			this.urlFactory = urlFactory;
		}
#pragma warning restore
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var urlHelper = urlFactory.GetUrlHelper(Context);

			TagBuilder ul = new("ul");
			ul.AddCssClass("pagination");
			ul.AddCssClass("mt-3");

			StringBuilder innerHtml = new("");

			innerHtml.Append(
				$"<li class='page-item'><a class='page-link' href='{urlHelper.ActionLink(PageAction, values: new { page = 1, pattern = Context.HttpContext.Request.Query["pattern"] })}'>First</a></li>"
				);

			if (ViewModel.HasPrevPage())
				innerHtml.Append(
									$"<li class='page-item'><a class='page-link' href='{urlHelper.ActionLink(PageAction, values: new { page = ViewModel.CurrPage - 1, pattern = Context.HttpContext.Request.Query["pattern"] })}'>Prev</a></li>"
					);

			int i = 0;
			do
			{
				var n = i + ViewModel.CurrPage;
				string disabled = i == 0 ? "disabled" : "";

				innerHtml.Append(
					$"<li class='page-item {disabled}'>" +
					$"	<a class='page-link' href='{urlHelper.ActionLink(PageAction, values: new { page = n, pattern = Context.HttpContext.Request.Query["pattern"] })}'>{n}</a>" +
					$"</li>"
					);
			} while (ViewModel.CurrPage + i < ViewModel.MaxPages && ++i < LinksCount);

			if (ViewModel.HasNextPage())
				innerHtml.Append(
				$"<li class='page-item'><a class='page-link' href='{urlHelper.ActionLink(PageAction, values: new { page = ViewModel.CurrPage + 1, pattern = Context.HttpContext.Request.Query["pattern"] })}'>Next</a></li>"
				);

			ul.InnerHtml.AppendHtml(innerHtml.ToString());

			output.TagName = "nav";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Content.AppendHtml(ul);
		}
	}
}
