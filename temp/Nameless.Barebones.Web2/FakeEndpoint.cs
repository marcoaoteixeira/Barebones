using FastEndpoints;

namespace Nameless.Barebones.Web2;

public record FakeResult {}

public class FakeEndpoint : Endpoint<FakeResult> {
    public override void Configure() {
        Get("/fakeendpoint");
    }

    public override Task HandleAsync(FakeResult req, CancellationToken ct) {
        return SendResultAsync(Results.Ok(new FakeResult()));
    }
}
