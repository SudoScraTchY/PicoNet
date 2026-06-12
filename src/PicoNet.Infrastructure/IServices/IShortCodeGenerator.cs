using PicoNet.Domain.ValueObjects;

namespace PicoNet.Infrastructure.IServices;

public interface IShortCodeGenerator
{
    ShortCode Generate();
    ShortCode GenerateFromId(long id);
    long DecodeToId(ShortCode code);
}