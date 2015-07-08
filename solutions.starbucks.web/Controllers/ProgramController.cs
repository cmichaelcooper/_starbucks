using solutions.starbucks.Interfaces;
using solutions.starbucks.Model;
using solutions.starbucks.web.Controllers.Masters;
using System.Web.Mvc;

namespace solutions.starbucks.web.Controllers
{

    public class ProgramController : GenericMasterController
    {

        protected static IProgramRepository _programRepository;
        //protected MemberAttributesRepository _memberRepository;
        //protected DashboardRepository _dashboardRepository;

        public ProgramController(IProgramRepository programRepository)
        {
            _programRepository = programRepository;
            //_memberRepository = new MemberAttributesRepository();
            //_dashboardRepository = new DashboardRepository();
        }

        //
        // GET: /Program/

        [Authorize]
        public ActionResult Program(string Season) 
        {
            ProgramModel model = new ProgramModel();

            model = _programRepository.GetProgramAttributes(CurrentPage);
            var ProgramBrand = _programRepository.GetProgramBrand(CurrentPage);
            model.TileContents = _programRepository.MapHeroTiles(CurrentPage, ProgramBrand);
            model.MarketingTiles = _programRepository.MapMarketing(CurrentPage);
            model.RecipeTiles = _programRepository.MapRecipes(CurrentPage);
            //model.ProgramBackground = _programRepository.GetProgramBackground(CurrentPage);
            //model.ProgramProductsLink = _programRepository.GetProductLink(CurrentPage);
            //model.RecipeCardUpload = _programRepository.GetRecipeCardUpload(CurrentPage);

            return View(model);
             
        }

    }
}
