namespace Kamino.Shared.Repo;

public static class ProfilesQueryExtensions
{
    public static IQueryable<Profile> WhereUriMatch(this IQueryable<Profile> profiles, Uri uri)
    {
        return profiles.Where(profile => profile.Uri == uri);
    }
}
