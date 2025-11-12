using Microsoft.AspNetCore.Routing;

namespace Evently.Modules.Events.Presentation.Events;

public static class EventEndpoints
{
    public static void MapEndPoints(this IEndpointRouteBuilder app)
    {
        CreateEvent.MapEndPoint(app);
        GetEvent.MapEndPoint(app);
    }
}
