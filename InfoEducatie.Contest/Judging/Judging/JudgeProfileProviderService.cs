using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InfoEducatie.Contest.Judging.Judges;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Contest.Judging.Judging;

[Service]
public class JudgeProfileProviderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private HttpContext HttpContext => _httpContextAccessor.HttpContext;
    private readonly IRepository<JudgeEntity> _judgesRepo;

    private UserWithRoles _user;

    protected virtual UserWithRoles UserFromClaims =>
        _user ??= new UserWithRoles
        {
            Id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = HttpContext.User.FindFirstValue(ClaimTypes.Email),
            Roles = HttpContext.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList()
        };

    public JudgeProfileProviderService(IHttpContextAccessor httpContextAccessor, IRepository<JudgeEntity> judgesRepo)
    {
        _httpContextAccessor = httpContextAccessor;
        _judgesRepo = judgesRepo;
    }

    public async Task<JudgeEntity> GetProfile(bool withCategory = false, bool includeUser = false)
    {
        if (withCategory)
            _judgesRepo.ChainQueryable(q => q.Include(j => j.Category));
        if (includeUser)
            _judgesRepo.ChainQueryable(q => q.Include(j => j.User));

        var query = _judgesRepo.Query;

        if (HttpContext.Request.Cookies.TryGetValue(ImpersonateCookieKey, out var value))
        {
            if (!UserFromClaims.HasRole("God"))
                throw new KnownException("Only Gods can do this!");
            query = query.Where(j => j.Id == value);
        }
        else
        {
            var userId = UserFromClaims.Id;
            query = query.Where(j => j.User.Id == userId);
        }

        return await query.FirstOrDefaultAsync() ??
               throw new KnownException("Your account doesn't have a judge profile associated.");
    }

    public async Task Impersonate(string judgeId)
    {
        if (!UserFromClaims.HasRole("God"))
            throw new KnownException("Only Gods can do this!");

        if (!await _judgesRepo.Any(j => j.Id == judgeId))
            throw new KnownException("There is no judge with this id.");

        var cookieOpt = new CookieOptions
        {
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(1),
            IsEssential = true,
            HttpOnly = true,
            Secure = false,
        };
        HttpContext.Response.Cookies.Append(ImpersonateCookieKey, judgeId, cookieOpt);
    }

    public void EndImpersonation()
    {
        if (!UserFromClaims.HasRole("God"))
            throw new KnownException("Only Gods can do this!");
        HttpContext.Response.Cookies.Delete(ImpersonateCookieKey);
    }

    private const string ImpersonateCookieKey = "X-IMPERSONATE-JUDGE-ID";
}