using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace RouletteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {
        IRouletteService rouletteService;
        IHttpContextAccessor httpContextAccessor;
        public RouletteController(IRouletteService rouletteService, IHttpContextAccessor httpContextAccessor)
        {
            this.rouletteService = rouletteService;
            this.httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        [AllowAnonymous]
        public string Get()
        {
            return "Welcome to roulette bet game!";
        }
        [AllowAnonymous]
        [HttpGet("List")]
        public async Task<Response> List()
        {
            return await rouletteService.ListRoulettes();
        }
        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<string> Create()
        {
            return await rouletteService.CreateRoulette();
        }
        [HttpPost("Open")]
        [AllowAnonymous]
        public async Task<string> Open(string id)
        {
            return await rouletteService.OpenRoulette(id);
        }
        [HttpPost("Close")]
        [AllowAnonymous]
        public async Task<Response> Close(string id)
        {
            return await rouletteService.CloseRoulette(id);
        }
        [HttpPost]
        [Authorize]
        public async Task<Response> Bet([FromBody] RouletteBet rouletteBet)
        {
            return await rouletteService.BetToRoulette(rouletteBet.ToBet(Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)));
        }
    }
}
