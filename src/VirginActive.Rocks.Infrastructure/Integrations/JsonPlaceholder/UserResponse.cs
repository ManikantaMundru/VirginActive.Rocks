using System;
using System.Collections.Generic;
using System.Text;

namespace VirginActive.Rocks.Infrastructure.Integrations.JsonPlaceholder
{
    internal sealed record UserResponse(
        int Id,
        string Name,
        string Username,
        string Email,
        string? Phone,
        string? Website);
}
