using Microsoft.AspNetCore.Mvc;

namespace PokemonTournamentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonTournamentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/pokemon/tournament/statistics")]
        public IActionResult Get()
        {

        }
    }
}
