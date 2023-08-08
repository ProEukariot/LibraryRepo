using LibraryApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace LibraryApp.ViewComponents
{
    public class BookListViewComponent : ViewComponent
    {
		public IViewComponentResult Invoke(BooksViewModel viewModel)
		{
			return View(viewModel);
        }
    }
}
