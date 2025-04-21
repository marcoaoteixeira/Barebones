using FastEndpoints;

namespace Nameless.Barebones.Web.Endpoints;

public record DummyResult {
    public string Message { get; init; } = "Hello, World!";
}

public class DummyEndpoint : Endpoint<DummyResult> {
    public override void Configure() {
        Get("/dummy");
        AllowAnonymous();
    }

    public override Task HandleAsync(DummyResult req, CancellationToken ct)
        => SendResultAsync(Results.Ok(new DummyResult()));
}
