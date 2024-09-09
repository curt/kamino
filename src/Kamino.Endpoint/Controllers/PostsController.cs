using Kamino.Endpoint.Models;
using Kamino.Entities;
using Kamino.Models;
using Kamino.Services;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kamino.Endpoint.Controllers;

[Route("p")]
public class PostsController(IDbContextFactory<ApplicationContext> contextFactory) : ContextualController
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index()
    {
        IEnumerable<PostViewModel> model;

        using (var context = contextFactory.CreateDbContext())
        {
            var postsService = new PostsService(context);
            var posts = await postsService.GetPublicPostsAsync();

            model = posts.Select(post => new PostViewModel(post, GetEndpoint()));
        }

        return View("index.html", model);
    }

    [Route("{id:regex(^[[1-9A-Za-z]]{{22}}$)}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Get(string id)
    {
        var guid = Uuid7.FromId22String(id).ToGuid();

        return await Contextualize(() => GetHtml(guid), () => GetJson(guid));
    }

    private async Task<IActionResult> GetHtml(Guid guid)
    {
        using var context = contextFactory.CreateDbContext();

        var postsService = new PostsService(context);
        var post = await postsService.GetPublicPostByIdAsync(guid);
        var model = new PostViewModel(post, GetEndpoint());

        return View("get.html", model);
    }

    private async Task<IActionResult> GetJson(Guid id)
    {
        var factory = new PostActivityModelFactory(Request.GetEndpoint());
        var model = await GetModel(id, factory);

        return Json(Contextify(model));
    }

    private async Task<TModel> GetModel<TModel>(Guid id, ModelFactoryBase<Post, TModel> factory)
    {
        using var context = contextFactory.CreateDbContext();

        var postsService = new PostsService(context);
        var model = await postsService.GetSinglePublicPostByIdAsync(id, factory);

        return model;
    }

    private Uri GetEndpoint() => (new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1)).Uri;
}
