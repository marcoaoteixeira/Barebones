using Microsoft.FluentUI.AspNetCore.Components;

namespace Nameless.Barebones.Infrastructure.Design.Icons;
public static class BarebonesIcons {
    public static Icon GetIcon(string name, IconSize size = IconSize.Size20)
        => name switch {
            nameof(Google) => new Google(size),
            nameof(Instagram) => new Instagram(size),
            nameof(Github) => new Github(size),
            nameof(StackOverflow) => new StackOverflow(size),
            _ => throw new ArgumentOutOfRangeException(nameof(name), $"Icon '{name}' not found.")
        };
}
