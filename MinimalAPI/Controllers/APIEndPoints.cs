namespace MinimalAPI.Controllers
{
    public static class APIEndPoints
    {
        public static void InitAPIEndPoints(this RouteGroupBuilder app)
        {
            app.PersonsAPIs();
            app.UserAPIs();
        }
    }
}
