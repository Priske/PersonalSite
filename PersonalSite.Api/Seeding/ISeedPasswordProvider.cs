
using QuickFuzzr;

namespace PersonalSite.Api.Seeding;

public interface ISeedPasswordProvider
{
    FuzzrOf<string> Password { get; }
}