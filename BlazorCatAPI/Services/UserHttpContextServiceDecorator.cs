namespace BlazorCatAPI.Services;

public class UserHttpContextServiceDecorator
{
    private readonly RequestDelegate next;

    public UserHttpContextServiceDecorator(RequestDelegate next)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context, UserHttpContextService service)
    {
        service.SetUser(context.User);
        await next(context);
    }
}
