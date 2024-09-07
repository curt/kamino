using Kamino.Entities;
using Kamino.Repo;
using Microsoft.EntityFrameworkCore;

namespace Kamino.Services;

public class PostsService(Context context) : IPostsService
{
    public async Task<IEnumerable<Post>> GetPublicPostsAsync()
    {
        var now = DateTime.UtcNow;

        var profiles = context.Profiles.WhereLocal();

        var posts = await context.Posts
            .Join(profiles, post => post.Author, profile => profile, (post, profile) => post)
            .WherePublished(now)
            .WhereNotTombstoned()
            .Include(post => post.Author)
            .Include(post => post.Places)
            .Include(post => post.Tags)
            .ToListAsync();

        return posts;
    }

    public async Task<Post> GetPublicPostByIdAsync(Guid id)
    {
        var now = DateTime.UtcNow;

        var profiles = context.Profiles.WhereLocal();

        var posts = await context.Posts
            .Join(profiles, post => post.Author, profile => profile, (post, profile) => post)
            .WhereIdMatch(id)
            .Include(post => post.Author)
            .Include(post => post.Places)
            .Include(post => post.Tags)
            .ToListAsync();

        return SinglePublicPost(posts, now);
    }

    private static Post SinglePublicPost(List<Post> posts, DateTime before)
    {
        if (posts.Count == 0)
        {
            throw new NotFoundException();
        }

        var published = posts.AsQueryable().WherePublished(before);

        if (!published.Any())
        {
            throw new ForbiddenException();
        }

        var notTombstoned = published.WhereNotTombstoned();

        if (!notTombstoned.Any())
        {
            throw new GoneException();
        }

        return notTombstoned.Single();
    }
}
