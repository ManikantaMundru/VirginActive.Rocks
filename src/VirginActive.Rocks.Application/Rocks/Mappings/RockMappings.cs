using VirginActive.Rocks.Application.Rocks.Models;
using VirginActive.Rocks.Domain.Entities;

namespace VirginActive.Rocks.Application.Rocks.Mappings
{
    public static class RockMappings
    {
        public static RockDto ToDto(this Rock rock)
        {
            return new RockDto(
                rock.Id,
                rock.MemberId,
                rock.Title,
                rock.Category,
                rock.DueDate,
                rock.Note,
                rock.Status,
                rock.CreatedAtUtc);
        }
    }
}
