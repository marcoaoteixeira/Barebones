namespace Nameless.Barebones.Web.Configs;

internal static class SecurityConfig {
    internal static WebApplication UseSecurityServices(this WebApplication self) {
        // Configure the HTTP request pipeline.
        if (!self.Environment.IsDevelopment()) {
            self.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want
            // to change this for production scenarios.
            // See https://aka.ms/aspnetcore-hsts.
            self.UseHsts();
        }

        self.UseHttpsRedirection()
            .UseAntiforgery()
            .UseAuthentication()
            .UseAuthorization();

        return self;
    }
}
