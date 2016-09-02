using Nancy;
using Nancy.Conventions;

public class StaticFileBootstrapper : DefaultNancyBootstrapper
{
    protected override void ConfigureConventions(NancyConventions conventions)
    {
        base.ConfigureConventions(conventions);

        conventions.StaticContentsConventions.Add(
            StaticContentConventionBuilder.AddDirectory("assets", "static/")
        );
    }
}