using Microsoft.AspNetCore.Components;

namespace PaginationExample;

public static class NavigationManagerExtensions
{
    public static string PaginationUri(this NavigationManager navigation, string path, int page, int size) =>
	    navigation.GetUriWithQueryParameters(path, new Dictionary<string, object?>
	    {
		    { "page", page },
		    { "size", size },
	    });
}