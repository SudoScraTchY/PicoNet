using HashidsNet;
using PicoNet.Domain.ValueObjects;
using PicoNet.Infrastructure.IServices;

namespace PicoNet.Infrastructure.Service;

public class ShortCodeGenerator : IShortCodeGenerator
{
    private readonly Hashids _hashids;
    
    public ShortCodeGenerator(string salt)
    {
        _hashids = new Hashids(salt, minHashLength: 6);
    }
    
    public ShortCode Generate()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var random = Random.Shared.Next(1000, 9999);
        var hash = _hashids.EncodeLong(ticks, random);
        return new ShortCode(hash[..8]);
    }
    
    public ShortCode GenerateFromId(long id)
    {
        return new ShortCode(_hashids.EncodeLong(id));
    }
    
    public long DecodeToId(ShortCode code)
    {
        var decoded = _hashids.DecodeLong(code.Value);
        return decoded.Length > 0 ? decoded[0] : 0;
    }
}